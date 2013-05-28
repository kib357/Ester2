using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Model.Services;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterCommon.Services;
using System.Linq;

namespace Ester.Model.Repositories
{
	// ReSharper disable ClassNeverInstantiated.Global
	public class PlanObjectsRepository : Repository
	// ReSharper restore ClassNeverInstantiated.Global
	{
		private readonly IDataTransport _dataTransport;

		public readonly Dictionary<int, BaseObject> PlanObjects;

		public ObservableCollection<BaseObject> RootObjects { get; set; }

		public override string Title { get { return "набор планов"; } }

		public PlanObjectsRepository(IDataTransport transport)
		{
			_dataTransport = transport;
			PlanObjects = new Dictionary<int, BaseObject>();

			ValuesLongPull.ValuesChangedEvent +=
				(updateInfo) =>
				{
					if (updateInfo.UpdateObjectTree || !updateInfo.ChangedValues.TrueForAll(cv => PlanObjects.ContainsKey(cv.Id)))
					{
						UpdateData();
						return;
					}

					foreach (var newObject in updateInfo.ChangedValues)
					{
						if (PlanObjects.ContainsKey(newObject.Id))
							PlanObjects[newObject.Id].Update(newObject);
					}
				};
		}

		public override void LoadData()
		{
			Task.Run(() => GetPlanObjects());
		}

		public async override Task UpdateData()
		{
			await Task.Run(() => GetPlanObjects());
		}

		public void SubmitProperties(BaseObject obj)
		{
			_dataTransport.PostRequest<bool>(obj, Urls.Data + "/" + obj.Id, true);
		}

		private void GetPlanObjects()
		{
			try
			{
				RootObjects = new ObservableCollection<BaseObject>(_dataTransport.GetRequest<List<BaseObject>>(Urls.Data, true, 100000, new PlanObjectConverter()));
				PlanObjects.Clear();
				FillPlanObjects(PlanObjects, RootObjects);

				if (!ValuesLongPull.IsStarted) ValuesLongPull.Start();

				OnDataReceivedEvent();
			}
			catch (Exception ex)
			{
				throw new Exception("Ошибка при получении планов.", ex);
			}
		}

		private void FillPlanObjects(Dictionary<int, BaseObject> planObjects, IEnumerable<BaseObject> rootObjects)
		{
			foreach (var rootObject in rootObjects)
			{
				planObjects.Add(rootObject.Id, rootObject);
				var containerObject = rootObject as IContainerObject;
				if (containerObject != null)
					FillPlanObjects(planObjects, containerObject.Children);
			}
		}
	}
}
