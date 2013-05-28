using System.Net.Http;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace EsterServer.Modules.Authentication
{
    [ServiceContract]
    public interface IAuthenticationModule
    {
        [WebGet(UriTemplate = "?user={login}&pass={password}", ResponseFormat = WebMessageFormat.Json)]
        string AuthenticateUser(string login, string password);
    }
}