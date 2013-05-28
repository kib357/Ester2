using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BacNetApi;
using EsterCommon.BaseClasses;
using EsterCommon.Exceptions;
using EsterServer.Model.Aspects;
using EsterServer.Model.Data;
using EsterServer.Model.Ioc;
using Newtonsoft.Json;
using Nini.Config;

namespace EsterServer.Modules.BacNetServer.Schedules
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [UnityServiceBehavior]
    public partial class SchedulesService : ISchedulesService
    {
        private readonly XmlConfigSource _configSource = new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };
        private readonly uint _masterController;
        private readonly BacnetSchedule _bacnetSchedule;
	    private readonly BacNet _network;
	    private readonly bool _syncSchedules;

	    public SchedulesService()
        {
            _bacnetSchedule = new BacnetSchedule();
            _masterController = (uint)_configSource.Configs["BacNet"].GetInt("SchedulesMainController");
			_syncSchedules = _configSource.Configs["BacNet"].GetBoolean("SyncSchedules");
            _network = BacNetServer.Network;
            Task.Factory.StartNew(SyncSchedules, TaskCreationOptions.LongRunning);
        }

        #region Users request
        [ServerExceptionAspect]
        public Stream GetAllSchedules()
        {
	        var myResponseBody = JsonConvert.SerializeObject(GetSchedulesFromDataBase().Where(s => !s.DeleteOnSync));
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
        }

        [ServerExceptionAspect]
        public Stream AddSchedule(Stream stream)
        {
            var schedule = GetScheduleClassFromStream(stream);
	        schedule.Id = GetFreeIdForType(schedule.Type);
	        schedule.OverrideController = true;
	        AddScheduleToDataBase(schedule);
            var myResponseBody = JsonConvert.SerializeObject(schedule);
	        if (WebOperationContext.Current != null)
		        WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
	        return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
        }

        [ServerExceptionAspect]
        public Stream EditSchedule(string id, Stream stream)
        {
            var schedule = GetScheduleClassFromStream(stream);
			try
			{
				int intId = int.Parse(id);
				schedule.Id = intId;
				schedule.OverrideController = true;
				EditScheduleInDataBase(schedule);
			}
			catch
			{
				throw new BadRequestException();
			}
			var myResponseBody = JsonConvert.SerializeObject(schedule);
	        if (WebOperationContext.Current != null)
		        WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
	        return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
        }	    

	    [ServerExceptionAspect]
        public void DeleteSchedule(string id)
        {			
			try
			{
				int intId = int.Parse(id);
				using (var context = new EsterClassesDataContext())
				{
					var dbSchedule = context.Schedules.Single(s => s.ObjectId == intId);
					dbSchedule.DeleteOnSync = true;
					context.SubmitChanges();
				}				
			}
			catch
			{
				throw new BadRequestException();
			}
        }        

        [ServerExceptionAspect]
        public Stream GetScheduleById(string id)
        {
			try
			{
				int intId = int.Parse(id);
				using (var context = new EsterClassesDataContext())
				{
					var dbSchedule = context.Schedules.Single(s => s.ObjectId == intId);
					var schedule = ConvertDbScheduleToScheduleClass(dbSchedule);
					var myResponseBody = JsonConvert.SerializeObject(schedule);
					if (WebOperationContext.Current != null)
						WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
					return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
				}			
			}
			catch
			{
				throw new BadRequestException();
			}
        }

		[ServerExceptionAspect]
		public void SetControlledObjects(Stream stream)
		{
			throw new NotImplementedException();
		}

        #endregion                

        private static ScheduleClass GetScheduleClassFromStream(Stream stream)
        {
            var reader = new StreamReader(stream);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<ScheduleClass>(data);
            return res;
        }        
    }
}
