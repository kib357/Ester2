using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.BaseClasses;
using EsterServer.Modules.Users.Wrappers;

namespace EsterServer.Modules.Users.Interfaces
{
    [ServiceContract]
    public interface IUsersModule
    {
        [WebGet(UriTemplate = "/AD/GetForest/{server}/{login}/{password}", ResponseFormat = WebMessageFormat.Json)]
        List<DomainTree> GetForest(string server, string login, string password);

        [WebGet(UriTemplate = "/AD/GetUsers/{domain}/{login}/{password}", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetDomainUsers(string domain, string login, string password);

        [WebInvoke(Method = "PUT", UriTemplate = "/AD/AddUser", ResponseFormat = WebMessageFormat.Json)]
        void AddDomainUsers(User user);
    }
}
