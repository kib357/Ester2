using System.IO;
using System.ServiceModel.Web;

namespace EsterServer.Modules.AccessControl
{
    interface IAccessRulesModule
    {
        [WebGet(UriTemplate = "/acl/items?{objectId}&{subjectId}&{actionId} ", ResponseFormat = WebMessageFormat.Json)]
        Stream GetAccessRule(string objectId, string subjectId, string actionId);

        [WebInvoke(UriTemplate = "/acl/items", ResponseFormat = WebMessageFormat.Json)]
        Stream AddAccessRule(Stream stream);

        [WebInvoke(UriTemplate = "/acl/items/{objectId}/{subjectId}/{actionId}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeAccessRule(string objectId, string subjectId, string actionId, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/access/acl/{objectId}/{subjectId}/{actionId}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteAccessRule(string objectId, string subjectId, string actionId);
    }
}
