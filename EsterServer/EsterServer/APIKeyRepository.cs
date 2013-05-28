using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using EsterCommon.Data;

namespace EsterServer
{
	public static class APIKeyRepository
	{
        private static readonly string EsterConnectionString = ConfigurationManager.ConnectionStrings["Ester"].ConnectionString;

		public static bool IsValidAPIKey(string key)
		{
            using (var classes = new PlansDc(EsterConnectionString))
			{
				Guid apiKey;
				if (Guid.TryParse(key, out apiKey) && classes.Users.Any(s => s.ApiKey == apiKey))
				{
					return true;
				}
			}
			return false;
		}
	}
}