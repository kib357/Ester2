using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ester.Model.Interfaces
{
    //public interface IDataTransport
    //{
    //    void GetRequestAsync<T>(Action<T> callback, string relativeUri, bool withApiKey = false, int timeout = 0);
    //    T GetRequest<T>(string relativeUri, bool withApiKey = false, int timeout = 0);
    //    void SendRequest(string uri, bool withApiKey = false, int timeout = 0);
    //    void PostRequest(string relativeUri, object postData, bool withApiKey = false, int timeout = 0);
    //}

    public interface IDataTransport
    {
        Task<T> GetRequestAsync<T>(string relativeUri, bool withApiKey = false, int timeout = 100000);
		T GetRequest<T>(string relativeUri, bool withApiKey = false, int timeout = 100000, params JsonConverter[] jsonConverters);        
        T PostRequest<T>(dynamic data, string relativeUri, bool withApiKey = false, int timeout = 100000);
        Task<T> PostRequestAsync<T>(dynamic data, string relativeUri, bool withApiKey = false, int timeout = 100000);
        void DeleteRequest(string relativeUri, bool withApiKey = false, int timeout = 100000);
        Task DeleteRequestAsync(string relativeUri, bool withApiKey = false, int timeout = 100000);
    }
}
