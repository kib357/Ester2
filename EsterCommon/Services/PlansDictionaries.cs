using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.Data;

namespace EsterCommon.Services
{
    public static class PlansDictionaries
    {
        private static readonly string EsterConnectionString = ConfigurationManager.ConnectionStrings["Ester"].ConnectionString;

        private static Dictionary<int, string> _planObjectTypes; 
        public static Dictionary<int, string> PlanObjectTypes
        {
            get
            {
                using (var context = new PlansDc(EsterConnectionString))
                {
                    if (_planObjectTypes == null || _planObjectTypes.Count == 0)
                        _planObjectTypes = context.PlanObjectTypes.ToDictionary(k => k.Id, v => v.ClassName);
                    return _planObjectTypes;
                }
            }
        }
    }
}
