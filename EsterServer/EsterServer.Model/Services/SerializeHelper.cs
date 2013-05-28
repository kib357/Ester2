using System.IO;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace EsterServer.Model.Services
{
    public class SerializeHelper
    {
        public static MemoryStream GetResponceFromString(string responceBody)
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(responceBody));
        }

        public static T GetObjectFromStream<T>(Stream stream, JsonConverter[] converters = null)
        {
            var reader = new StreamReader(stream);
            var data = reader.ReadToEnd();
            var res = converters != null ? JsonConvert.DeserializeObject<T>(data, converters) : JsonConvert.DeserializeObject<T>(data);
            return res;
        }
    }
}
