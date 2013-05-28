using Newtonsoft.Json;
using RestSharp.Serializers;

namespace Ester.Model.Services
{
    public class JsonConvertWrapper : ISerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public string ContentType { get; set; }
    }
}
