using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsterCommon.BaseClasses;
using EsterServer.Modules.Users.ActiveDirectory;
using EsterServer.Modules.Users.Wrappers;

namespace EsterServer.Modules.Users
{

    
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UsersModule
    {
        public List<DomainTree> GetForest(string server, string login, string password)
        {
            return WorkWithAD.GetDomainList(server, login, password);
        }

        public List<string> GetDomainUsers(string domain, string login, string password)
        {
            return WorkWithAD.GetUsers(domain, login, password);
        }

        public void AddDomainUsers(User user)
        {
            WorkWithAD.AddUser(user);
        }

        
    }
}
