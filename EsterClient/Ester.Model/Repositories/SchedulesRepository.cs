using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using EsterCommon.BaseClasses;
using EsterCommon.Data;
using EsterCommon.Enums;

namespace Ester.Model.Repositories
{
	public class SchedulesRepository : Repository
	{
		private readonly IDataTransport _dataTransport;

		public SchedulesRepository(IDataTransport dataTransport)
		{
			_dataTransport = dataTransport;
			Schedules = new ObservableCollection<ScheduleClass>();
		}

		public override string Title
		{
			get { return "расписания"; }
		}

		public override void LoadData()
		{
			Task.Run(() => GetSchedulesFromServer());
		}

		public async override Task UpdateData()
		{
			await Task.Run(() => GetSchedulesFromServer());
		}

		public Dictionary<Property, ScheduleClass> GetSchedulesForControlledObjects(List<Property> controlledObjects)
		{
			var res = new Dictionary<Property, ScheduleClass>();
			foreach (var controlledObject in controlledObjects)
			{
				foreach (var scheduleClass in Schedules)
				{
					if (scheduleClass.ControlledObjects.Contains(controlledObject))
					{
						if (!res.ContainsKey(controlledObject))
							res.Add(controlledObject, scheduleClass);
						break;
					}
				}
				if (!res.ContainsKey(controlledObject))
					res.Add(controlledObject, new ScheduleClass(true));
			}
			return res;
		}

		public async Task<ScheduleClass> SaveSchedule(ScheduleClass schedule, bool create = false)
		{
			return await Task.Run(() =>
									  {
										  const string saveScheduleUrl = @"/Schedules/";
										  ScheduleClass savedSchedule = null;
										  try
										  {
											  savedSchedule = _dataTransport.PostRequest<ScheduleClass>(schedule,
																										saveScheduleUrl +
																										(schedule.Id == 0 || create
																											 ? ""
																											 : schedule.Id.ToString()), true);
										  }
										  catch (Exception)
										  {
											  return savedSchedule;
										  }
										  return savedSchedule;
									  });
		}

		public void SubmitChanges()
		{
			throw new NotImplementedException();
		}

		private async void GetSchedulesFromServer()
		{
			var schedules = await _dataTransport.GetRequestAsync<List<ScheduleClass>>(Urls.Schedules, true);
			Schedules = MakeFullWeeks(schedules);
			SKUDSchedules = new ObservableCollection<ScheduleClass>(Schedules.Where(s => s.Type == ScheduleTypes.SKUD));
			SKUDSchedules.Insert(0, new ScheduleClass(true));
			VentilationSchedules = new ObservableCollection<ScheduleClass>(Schedules.Where(s => s.Type == ScheduleTypes.Ventilation));
			VentilationSchedules.Insert(0, new ScheduleClass(true));
			LightSchedules = new ObservableCollection<ScheduleClass>(Schedules.Where(s => s.Type == ScheduleTypes.Light));
			LightSchedules.Insert(0, new ScheduleClass(true));
			HeatSchedules = new ObservableCollection<ScheduleClass>(Schedules.Where(s => s.Type == ScheduleTypes.Heat));
			HeatSchedules.Insert(0, new ScheduleClass(true));
			ACSchedules = new ObservableCollection<ScheduleClass>(Schedules.Where(s => s.Type == ScheduleTypes.AC));
			ACSchedules.Insert(0, new ScheduleClass(true));

			OnDataReceivedEvent();
		}

		private ObservableCollection<ScheduleClass> MakeFullWeeks(List<ScheduleClass> collection)
		{
			foreach (var schedule in collection)
			{
				for (int i = 0; i < 7; i++)
					schedule.Days[i] = GetFullDay(schedule.Days[i]);

				for (var i = 0; i < 7; i++)
					if (schedule.Days[i] != null)
						schedule.Days[i] = new List<ValueTimeRange>(schedule.Days[i].OrderBy(s => s.Start));
					else
						schedule.Days[i] = new List<ValueTimeRange>();
			}
			return new ObservableCollection<ScheduleClass>(collection);
		}

		private List<ValueTimeRange> GetFullDay(List<ValueTimeRange> day)
		{
			var startDay = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, 0, 0, 0);
			var endDay = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, 23, 59, 59);

			var res = new List<ValueTimeRange>();
			if (day.Count == 0)
			{
				res.Add(new ValueTimeRange(startDay, null)
							{
								Length = TimeSpan.FromMinutes(1439)
							});
				return res;
			}
			var currentElement = 0;
			if (day[0].Start != startDay)
				res.Add(new ValueTimeRange(startDay, null));
			res.AddRange(day);
			foreach (var valueTimeRange in res.OrderBy(s => s.Start))
			{
				if (res.Count - 1 > currentElement)
					valueTimeRange.Length = res[currentElement + 1].Start - valueTimeRange.Start;
				else
					valueTimeRange.Length = endDay - valueTimeRange.Start;
				currentElement++;
			}
			return res;
		}

		public ObservableCollection<ScheduleClass> Schedules { get; set; }

		public ObservableCollection<ScheduleClass> SKUDSchedules { get; set; }
		public ObservableCollection<ScheduleClass> VentilationSchedules { get; set; }
		public ObservableCollection<ScheduleClass> LightSchedules { get; set; }
		public ObservableCollection<ScheduleClass> HeatSchedules { get; set; }
		public ObservableCollection<ScheduleClass> ACSchedules { get; set; }
	}
}
