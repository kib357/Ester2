using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace EsterServer.Modules.BacNetServer
{
    [ServiceContract] 
    interface IBacNetServer
    {

		[WebGet(UriTemplate = "/plans", ResponseFormat = WebMessageFormat.Json)]
		Stream GetPlanObjects();

        [WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        Stream GetAllSensorsValues();

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}", ResponseFormat = WebMessageFormat.Json)]
        string GetPresentValueProperty(string deviceAddress, string objectAddress);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistory(string deviceAddress, string objectAddress);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history/{frequency}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistoryWithEqualPeriods(string deviceAddress, string objectAddress, string frequency);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history/smooth", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistoryWithCalculatedEqualPeriods(string deviceAddress, string objectAddress);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history/{startDate}/{endDate}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistoryFilteredByDate(string deviceAddress, string objectAddress, string startDate, string endDate);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history/{startDate}/{endDate}/{frequency}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistoryWithEqualPeriodsFilteredByDate(string deviceAddress, string objectAddress, string frequency, string startDate, string endDate);

        [WebGet(UriTemplate = "/{deviceAddress}/{objectAddress}/history/{startDate}/{endDate}/smooth", ResponseFormat = WebMessageFormat.Json)]
        Stream GetPresentValueHistoryWithCalculatedEqualPeriodsFilteredByDate(string deviceAddress, string objectAddress, string startDate, string endDate);

        [WebGet(UriTemplate = "{deviceAddress}/{objectAddress}?property={propertyName}", ResponseFormat = WebMessageFormat.Json)]
        string GetCustomPropertyValue(string deviceAddress, string objectAddress, string propertyName);

        [WebInvoke(UriTemplate = "/{deviceAddress}/{objectAddress}", ResponseFormat = WebMessageFormat.Json)]
        void SetPresentValueProperty(string deviceAddress, string objectAddress, Stream stream);

        [WebInvoke(UriTemplate = "/{deviceAddress}/{objectAddress}?property={propertyName}", ResponseFormat = WebMessageFormat.Json)]
        void SetCustomPropertyValue(string deviceAddress, string objectAddress, string propertyName, Stream stream);

        [WebInvoke(UriTemplate = "/SetSeveral", ResponseFormat = WebMessageFormat.Json)]
        void SetSeveral(Stream stream);

        [WebGet(UriTemplate = "/devices", ResponseFormat = WebMessageFormat.Json)]
        Stream GetDevices();
    }
}
