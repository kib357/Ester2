using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace EsterServer.Modules.TestPlugin
{
    [ServiceContract]
    interface IYaWebService
    {
        [WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        string Get();
    }
}
