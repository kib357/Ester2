using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace EsterServer.Modules.AccessControl
{
    [ServiceContract]
    public interface IAccessControlModule
    {
#region сотрудники
        [WebGet(UriTemplate = "/access/employees", ResponseFormat = WebMessageFormat.Json)]
        Stream GetEmployees();

        [WebInvoke(UriTemplate = "/access/employees", ResponseFormat = WebMessageFormat.Json)]
        Stream AddEmployee(Stream stream);

        [WebInvoke(UriTemplate = "/access/employees/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeEmployee(string id, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/access/employees/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteEmployee(string id);
#endregion

#region посетители
        [WebGet(UriTemplate = "/access/guests", ResponseFormat = WebMessageFormat.Json)]
        Stream GetGuests();

        [WebInvoke(UriTemplate = "/access/guests", ResponseFormat = WebMessageFormat.Json)]
        Stream AddGuest(Stream stream);

        [WebInvoke(UriTemplate = "/access/guests/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeGuest(string id, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/access/guests/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteGuest(string id);
#endregion

#region группы доступа
        [WebGet(UriTemplate = "/access/personGroups", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPersonGroups();

        [WebInvoke(UriTemplate = "/access/personGroups", ResponseFormat = WebMessageFormat.Json)]
        Stream AddPersonGroup(Stream stream);

        [WebInvoke(UriTemplate = "/access/personGroups/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangePersonGroup(string id, Stream stream);
#endregion

#region Считыватели карт
        [WebGet(UriTemplate = "/access/cardReaders", ResponseFormat = WebMessageFormat.Json)]
        Stream GetCardReaders();

        [WebInvoke(UriTemplate = "/access/cardReaders", ResponseFormat = WebMessageFormat.Json)]
        Stream AddCardReader(Stream stream);

        [WebInvoke(UriTemplate = "/access/cardReaders/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeCardReader(string id, Stream stream);

        [WebGet(UriTemplate = "/access/cardReaders/{id}/modes", ResponseFormat = WebMessageFormat.Json)]
        Stream GetCardReaderModes(string id);

        [WebInvoke(Method = "DELETE", UriTemplate = "/access/cardReaders/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteCardReader(string id);

        [WebGet(UriTemplate = "/access/cardReaderGroups", ResponseFormat = WebMessageFormat.Json)]
        Stream GetCardReaderGroups();

        [WebInvoke(UriTemplate = "/access/cardReaderGroups", ResponseFormat = WebMessageFormat.Json)]
        Stream AddCardReaderGroups(Stream stream);

        [WebInvoke(UriTemplate = "/access/cardReaderGroups/{id}", ResponseFormat = WebMessageFormat.Json)]
        Stream ChangeCardReaderGroup(string id, Stream stream);

        [WebInvoke(Method = "DELETE", UriTemplate = "/access/cardReaderGroups/{id}", ResponseFormat = WebMessageFormat.Json)]
        void DeleteCardReaderGroups(string id);
#endregion
    }
}
