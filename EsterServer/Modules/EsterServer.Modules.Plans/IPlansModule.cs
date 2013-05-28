using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace EsterServer.Modules.Plans
{
	[ServiceContract]
	public interface IPlansModule
	{
		[WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
		Stream Get();

		[WebGet(UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json)]
		Stream GetItem(string id);

        //[WebGet(UriTemplate = "/properties/{id}", ResponseFormat = WebMessageFormat.Json)]
        //Stream GetItemProperties(string id);

        [WebGet(UriTemplate = "/typeproperties/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetTypeProperties(string id);

		[WebInvoke(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
		Stream Add(Stream stream);

		[WebInvoke(UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json)]
		Stream Edit(string id, Stream stream);

		[WebInvoke(Method = "DELETE", UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json)]
		void Delete(string id);

        [WebInvoke(UriTemplate = "/importsvg", ResponseFormat = WebMessageFormat.Json)]
        Stream ImportSvg(Stream stream);

        [WebGet(UriTemplate = "/planelements/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPlanElements(string id);

        [WebGet(UriTemplate = "/units", ResponseFormat = WebMessageFormat.Json)]
        Stream GetUnits();

        [WebGet(UriTemplate = "/alarmlevels", ResponseFormat = WebMessageFormat.Json)]
        Stream GetAlarmLevels();
	}
}
