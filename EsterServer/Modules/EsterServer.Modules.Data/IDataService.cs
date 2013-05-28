using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace EsterServer.Modules.Data
{
    [ServiceContract]
    public interface IDataService
    {
        [WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        Stream Get();

		[WebGet(UriTemplate = "/devices", ResponseFormat = WebMessageFormat.Json)]
		Stream GetDevices();
		
		[WebInvoke(UriTemplate = "/{id}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
		Stream Edit(string id, Stream stream);

    }
}
