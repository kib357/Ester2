using System.IO;
using System.ServiceModel.Web;

namespace EsterServer.Modules.AccessControl
{
    interface IIntruderAlarmModule
    {
        [WebGet(UriTemplate = "/intruderalarm/areas", ResponseFormat = WebMessageFormat.Json)]
        Stream GetIntruderAlarmAreas();

        [WebInvoke(UriTemplate = "/intruderalarm/areas", ResponseFormat = WebMessageFormat.Json)]
        Stream AddIntruderAlarmArea(Stream stream);

        [WebInvoke(UriTemplate = "/intruderalarm/areas/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeIntruderAlarmArea(string id, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/intruderalarm/areas/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteIntruderAlarmArea(string id);

        [WebGet(UriTemplate = "/intruderalarm/areagroups", ResponseFormat = WebMessageFormat.Json)]
        Stream GetIntruderAlarmAreaGroups();

        [WebInvoke(UriTemplate = "/intruderalarm/areagroups", ResponseFormat = WebMessageFormat.Json)]
        Stream AddIntruderAlarmAreaGroup(Stream stream);

        [WebInvoke(UriTemplate = "/intruderalarm/areagroups/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeIntruderAlarmAreaGroup(string id, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/intruderalarm/areagroups/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteIntruderAlarmAreaGroup(string id);
    }
}
