using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EsterServer.Modules.Update
{
    [ServiceContract]
    public interface IUpdateModule
    {
        [WebGet(UriTemplate = "/?client-version={clientVersion}", ResponseFormat = WebMessageFormat.Json)]
        string CheckForUpdates(string clientVersion);
    }
}
