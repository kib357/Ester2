using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Routing;
using EsterServer.Core;
using EsterServer.Model;
//using EsterServer.Modules.Authentication;
//using EsterServer.Modules.Data.Notifications;
//using EsterServer.Modules.Data.Schedules;
//using EsterServer.Modules.Logs;
//using EsterServer.Modules.Plans;
//using EsterServer.Modules.Update;
//using EsterServer.Modules.Users;
//using EsterServer.Modules.Data;


namespace EsterServer
{
	public class Global : HttpApplication
	{
		//private readonly UnityContainer _container = IocContainer.Instance;

		void Application_Start(object sender, EventArgs e)
		{
		    var core = new Bootstrapper();
            core.Run();
		    //RegisterRoutes();
		    //_container.RegisterInstance(_container.Resolve<DataManager>());

		    //var configSource = new XmlConfigSource(
		    //    Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };
		    //string path;
		    //try
		    //{
		    //    path = configSource.Configs["ActiveDirectory"].Get("Path") ?? "";
		    //}
		    //catch
		    //{
		    //    path = "";
		    //}
		}

		void Application_End(object sender, EventArgs e)
		{

		}

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Context.Items.Contains("SuppressAuthenticationKey"))
            {
                Response.TrySkipIisCustomErrors = true;
                Response.ClearContent();
                Response.StatusCode = 401;
                Response.RedirectLocation = null;
                Response.Write("Invalid API key");
            }
        }

		private void RegisterRoutes()
		{
            //RouteTable.Routes.Add(new ServiceRoute("info", new WebServiceHostFactory(), typeof(ServerInfo)));
            //RouteTable.Routes.Add(new ServiceRoute("login", new WebServiceHostFactory(), typeof(AuthenticationModule)));
            //RouteTable.Routes.Add(new ServiceRoute("data", new WebServiceHostFactory(), typeof(DataService)));
            //RouteTable.Routes.Add(new ServiceRoute("logs", new WebServiceHostFactory(), typeof(LogModule)));
            //RouteTable.Routes.Add(new ServiceRoute("updates", new WebServiceHostFactory(), typeof(UpdateModule)));
            //RouteTable.Routes.Add(new ServiceRoute("notifications", new WebServiceHostFactory(), typeof(DataPusher)));
            //RouteTable.Routes.Add(new ServiceRoute("users", new WebServiceHostFactory(), typeof(UsersModule)));
            //RouteTable.Routes.Add(new ServiceRoute("values", new WebServiceHostFactory(), typeof(ValuesPusher)));
            //RouteTable.Routes.Add(new ServiceRoute("schedules", new WebServiceHostFactory(), typeof(SchedulesService)));
            //RouteTable.Routes.Add(new ServiceRoute("plans", new WebServiceHostFactory(), typeof(PlansModule)));
		}
	}
}
