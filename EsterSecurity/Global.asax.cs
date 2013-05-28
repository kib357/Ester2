using System;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace EsterSecurity
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			RegisterRoutes();
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(Authentication)));
		}
	}
}