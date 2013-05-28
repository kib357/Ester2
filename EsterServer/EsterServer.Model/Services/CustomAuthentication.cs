using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.Data;

namespace EsterServer.Model.Services
{
    public class CustomAuthentication
    {
        private readonly static string EsterConnectionString = ConfigurationManager.ConnectionStrings["Ester"].ConnectionString;

        public static bool ValidateUser(string username, string password)
        {
            using (var context = new PlansDc(EsterConnectionString))
            {
                if (context.Users.Any(u => u.Login == username && u.Password == password))
                    return true;
            }
            return false;
        }

        public static bool IsUserExist(string username)
        {
            using (var context = new PlansDc(EsterConnectionString))
            {
                if (context.Users.Any(u => u.Login == username))
                    return true;
            }
            return false;
        }
    }
}
