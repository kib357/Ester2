using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using EsterCommon.BaseClasses;
using EsterCommon.Data;
using EsterCommon.Enums;

namespace EsterServer.Modules.Data.Schedules
{
	public partial class SchedulesService
	{
		public async void SyncSchedules()
		{
			while (_syncSchedules)
			{
				var masterControllerSchedules = GetSchedulesFromMasterController();
				if (masterControllerSchedules == null)
				{
					await Task.Delay(TimeSpan.FromMinutes(1));
					continue;
				}
				var dataBaseSchedules = GetSchedulesFromDataBase();
				SyncSchedulesFromDbToController(dataBaseSchedules, masterControllerSchedules);
				await Task.Delay(TimeSpan.FromMinutes(1));
			}
		}

		private void SyncSchedulesFromDbToController(List<ScheduleClass> dataBaseSchedules, List<ScheduleClass> masterControllerSchedules)
		{
			//удаление помеченных расписаний
			foreach (var dataBaseSchedule in dataBaseSchedules.Where(s => s.DeleteOnSync))
			{
				DeleteScheduleFromControllers(dataBaseSchedule);
				DeleteScheduleFromDb(dataBaseSchedule);
				masterControllerSchedules.RemoveAll(s => s.Id == dataBaseSchedule.Id);
			}
			dataBaseSchedules = GetSchedulesFromDataBase();

			foreach (var dataBaseSchedule in dataBaseSchedules)
			{
				var index = masterControllerSchedules.FindIndex(s => s.Id == dataBaseSchedule.Id);
				//Добавление на контроллеры отсутствующих расписаний
				if (index < 0)
					AddScheduleToControllers(dataBaseSchedule);
				else
				{
					//обновление помеченных расписаний на контроллерах
					if (dataBaseSchedule.OverrideController)
						EditScheduleOnControllers(dataBaseSchedule);
					//обновление WeeklySchedule расписаний на контроллерах и в базе с мастер контроллера
					else
					{
						var scheduleChanged = IsScheduleChanged(masterControllerSchedules[index], dataBaseSchedule);
						if (scheduleChanged)
						{
							dataBaseSchedule.Name = masterControllerSchedules[index].Name;
							dataBaseSchedule.Days = masterControllerSchedules[index].Days;
							EditScheduleOnControllers(dataBaseSchedule);
						}
					}
				}
			}

			//добавление новых расписаний из контроллеров
			foreach (var masterControllerSchedule in masterControllerSchedules)
			{
				if (dataBaseSchedules.All(s => s.Id != masterControllerSchedule.Id))
					AddScheduleToDataBase(masterControllerSchedule);
			}
		}

		private static bool IsScheduleChanged(ScheduleClass masterControllerSchedule, ScheduleClass dataBaseSchedule)
		{
			bool scheduleChanged = dataBaseSchedule.Name != masterControllerSchedule.Name;

			if (dataBaseSchedule.Days.Length != masterControllerSchedule.Days.Length)
				return true;

			for (int i = 0; i < dataBaseSchedule.Days.Length; i++)
			{
				if (dataBaseSchedule.Days[i].Count != masterControllerSchedule.Days[i].Count)
					return true;

				for (int j = 0; j < dataBaseSchedule.Days[i].Count; j++)
				{
					if (dataBaseSchedule.Days[i][j].Start != masterControllerSchedule.Days[i][j].Start ||
						dataBaseSchedule.Days[i][j].Value != masterControllerSchedule.Days[i][j].Value)
						scheduleChanged = true;
				}
			}
			return scheduleChanged;
		}

		#region DataBase

		private List<ScheduleClass> GetSchedulesFromDataBase()
		{
			var dataBaseSchedules = new List<ScheduleClass>();
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
			{
				foreach (var dbSchedule in context.Schedules)
				{
					var schedule = ConvertDbScheduleToScheduleClass(dbSchedule);
					dataBaseSchedules.Add(schedule);
				}
			}
			return dataBaseSchedules;
		}

		private void AddScheduleToDataBase(ScheduleClass controllerSchedule)
		{
			//добавление расписания в бд
			Schedule sch = ConvertScheduleClassToDbSchedule(controllerSchedule);
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
			{
				var dbSch = context.Schedules.FirstOrDefault(s => s.ObjectId == sch.ObjectId);
				if (dbSch != null)
					context.Schedules.DeleteOnSubmit(dbSch);
				context.Schedules.InsertOnSubmit(sch);
				context.SubmitChanges();
			}
		}

		private void EditScheduleInDataBase(ScheduleClass controllerSchedule)
		{
			Schedule newDbSchedule = ConvertScheduleClassToDbSchedule(controllerSchedule);
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
			{
				var dbSchedule = context.Schedules.Single(s => s.ObjectId == newDbSchedule.ObjectId);
				dbSchedule.Title = newDbSchedule.Title;
				dbSchedule.OverrideController = newDbSchedule.OverrideController;
				dbSchedule.DeleteOnSync = newDbSchedule.DeleteOnSync;
				dbSchedule.SchedulesContents = newDbSchedule.SchedulesContents;
				dbSchedule.SchedulesControlledProperties = newDbSchedule.SchedulesControlledProperties;
				context.SubmitChanges();
			}
		}

		private void DeleteScheduleFromDb(ScheduleClass dataBaseSchedule)
		{
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
			{
				var dbSch = context.Schedules.Single(s => s.ObjectId == dataBaseSchedule.Id);
				context.Schedules.DeleteOnSubmit(dbSch);
				context.SubmitChanges();
			}
		}

		private ScheduleClass ConvertDbScheduleToScheduleClass(Schedule dbSchedule)
		{
			var schedule = new ScheduleClass
			{
				Name = dbSchedule.Title,
				Id = dbSchedule.ObjectId,
				OverrideController = dbSchedule.OverrideController ?? false,
				DeleteOnSync = dbSchedule.DeleteOnSync ?? false,
				ControlledObjects = dbSchedule.SchedulesControlledProperties.Select(c => c.Property).ToList()
			};

			var days = new List<List<ValueTimeRange>>();
			for (int i = 0; i < 7; i++)
			{
				var daySchedule = dbSchedule.SchedulesContents.Where(sc => sc.Day == i).
					Select(ds => new ValueTimeRange(ds.Time, ds.Value)).ToList();
				days.Add(daySchedule);
			}
			schedule.Days = days.ToArray();
			return schedule;
		}

		private Schedule ConvertScheduleClassToDbSchedule(ScheduleClass schedule)
		{
			var dbSchedule = new Schedule
			{
				Title = schedule.Name,
				ObjectId = schedule.Id,
				OverrideController = schedule.OverrideController,
				DeleteOnSync = schedule.DeleteOnSync
			};

			foreach (var controlledObject in schedule.ControlledObjects)
			{
				dbSchedule.SchedulesControlledProperties.Add(new SchedulesControlledProperty() { });
			}

			for (int index = 0; index < schedule.Days.Length; index++)
			{
				var day = schedule.Days[index];
				dbSchedule.SchedulesContents.AddRange(
					day.Select(s => new SchedulesContent { Day = index, Time = s.Start, Value = s.Value }).ToList());
			}
			return dbSchedule;
		}

		#endregion

		#region Controllers

		private List<ScheduleClass> GetSchedulesFromMasterController()
		{
			var masterDevice = _network.OnlineDevices.FirstOrDefault(d => d.Id == _masterController);
			if (masterDevice == null || masterDevice.ObjectList.Count == 0) return null;
			var schedules = new List<ScheduleClass>();
			List<string> controllerSchedules;
			lock (masterDevice.SyncRoot)
				controllerSchedules = masterDevice.ObjectList.Where(bacnetObject => bacnetObject.Contains("SCH")).ToList();
			foreach (var bacnetObject in controllerSchedules)
				schedules.Add(_bacnetSchedule.ReadSchedule(masterDevice.Id, Convert.ToInt32(bacnetObject.Replace("SCH", ""))));
			return schedules;
		}

		private void AddScheduleToControllers(ScheduleClass sch)
		{
			var controlledObjectsForEachController = GenerateControlledObjectForEachController(sch);
			if (!controlledObjectsForEachController.ContainsKey(_masterController))
				controlledObjectsForEachController.Add(_masterController, new List<string>());

			foreach (var controller in controlledObjectsForEachController)
			{
				var tmpSchedule = GetCurrentScheduleToWrite(sch, controller);
				_bacnetSchedule.CreateSchedule(controller.Key, tmpSchedule.Id, tmpSchedule.Name);
				_bacnetSchedule.WriteSchedule(controller.Key, tmpSchedule);
			}
			sch.OverrideController = false;
			EditScheduleInDataBase(sch);
		}

		private void EditScheduleOnControllers(ScheduleClass dbSchedule)
		{
			AddScheduleToControllers(dbSchedule);
			var schdeuleDevices = _network.OnlineDevices.Where(d => d.ObjectList.Contains("SCH" + dbSchedule.Id));
			//foreach (var deviceWithSchedule in schdeuleDevices)
			//{
			//	if (!dbSchedule.Controllers.Contains(deviceWithSchedule.Id) && deviceWithSchedule.Id != _masterController)
			//		_bacnetSchedule.DeleteSchedule(deviceWithSchedule.Id, dbSchedule.Id);
			//}
			dbSchedule.OverrideController = false;
			EditScheduleInDataBase(dbSchedule);
		}

		private void DeleteScheduleFromControllers(ScheduleClass schedule)
		{
			//foreach (var controller in schedule.Controllers)
			//{
			//	_bacnetSchedule.DeleteSchedule(controller, schedule.Id);
			//}
			var controllersWithSchedule =
				_network.OnlineDevices.Where(d => d.ObjectList.Contains("SCH" + schedule.Id));
			foreach (var controller in controllersWithSchedule)
			{
				_bacnetSchedule.DeleteSchedule(controller.Id, schedule.Id);
			}
		}

		private Dictionary<uint, List<string>> GenerateControlledObjectForEachController(ScheduleClass sch)
		{
			var controlledObjectsForEachController = new Dictionary<uint, List<string>>();
			//foreach (var controlledObject in sch.ControlledObjects)
			//{
			//	var tmp = controlledObject.Split('.')[0];
			//	uint controller;
			//	if (uint.TryParse(tmp, out controller))
			//	{
			//		var obj = controlledObject.Split('.')[1];
			//		if (controlledObjectsForEachController.ContainsKey(controller))
			//			controlledObjectsForEachController[controller].Add(obj);
			//		else
			//			controlledObjectsForEachController.Add(controller, new List<string> { obj });
			//	}
			//}
			return controlledObjectsForEachController;
		}

		private ScheduleClass GetCurrentScheduleToWrite(ScheduleClass sch, KeyValuePair<uint, List<string>> controlledObjects)
		{
			var tmpSchedule = new ScheduleClass { Id = sch.Id, Name = sch.Name, Days = sch.Days, ControlledObjects = new List<Property>() };
			//foreach (var obj in controlledObjects.Value)
			//{
			//	tmpSchedule.ControlledObjects.Add(controlledObjects.Key + "." + obj);
			//}
			return tmpSchedule;
		}

		#endregion

		private int GetFreeIdForType(ScheduleTypes type)
		{
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
			{
				int minId = 0, maxId = 0;
				switch (type)
				{
					case ScheduleTypes.SKUD:
						minId = (int)ScheduleClass.MinScudTypeNumber;
						maxId = (int)ScheduleClass.MaxScudTypeNumber;
						break;
					case ScheduleTypes.Heat:
						minId = (int)ScheduleClass.MinHeatTypeNumber;
						maxId = (int)ScheduleClass.MaxHeatTypeNumber;
						break;
					case ScheduleTypes.Light:
						minId = (int)ScheduleClass.MinLightTypeNumber;
						maxId = (int)ScheduleClass.MaxLightTypeNumber;
						break;
					case ScheduleTypes.AC:
						minId = (int)ScheduleClass.MinACTypeNumber;
						maxId = (int)ScheduleClass.MaxACTypeNumber;
						break;
					case ScheduleTypes.Ventilation:
						minId = (int)ScheduleClass.MinVentialtionTypeNumber;
						maxId = (int)ScheduleClass.MaxVentialtionTypeNumber;
						break;
				}
				var schedules = context.Schedules.Where(s => s.ObjectId >= minId && s.ObjectId <= maxId).ToList();
				for (var i = minId; i <= maxId; i++)
					if (schedules.All(s => s.ObjectId != i))
						return i;
				return 0;
			}
		}
	}
}
