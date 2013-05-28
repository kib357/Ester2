using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace EsterServer.Modules.Data.Schedules
{
    [ServiceContract]
    interface ISchedulesService
    {
        [WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        Stream GetAllSchedules();

        [WebGet(UriTemplate = "/sync", ResponseFormat = WebMessageFormat.Json)]
        void SyncSchedules();

        [WebInvoke(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        Stream AddSchedule(Stream stream);

        [WebInvoke(UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream EditSchedule(string id, Stream stream);

        [WebInvoke(UriTemplate = "/SetObjects", ResponseFormat = WebMessageFormat.Json)]
        void SetControlledObjects(Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteSchedule(string id);

        [WebGet(UriTemplate = "/GetSchedule/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetScheduleById(string id);
    }
}
