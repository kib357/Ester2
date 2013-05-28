using EsterServer.Model.Interfaces;
using Microsoft.Practices.Unity;

namespace EsterServer.Modules.TestPlugin
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class YaWebService : IYaWebService, IRestService
    {
        public YaWebService(IUnityContainer container)
        {
        }

        public int Number { get; set; }
        public string Get()
        {
            return "YaWebService. Number: " + Number;
        }
    }
}
