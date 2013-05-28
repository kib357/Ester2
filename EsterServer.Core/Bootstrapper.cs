using System.IO;
using System.Web.Hosting;
using System.Windows;
using EsterServer.Model.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace EsterServer.Core
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            var moduleManager = Container.Resolve<ModuleManager>();
            moduleManager.LoadModuleCompleted += OnModulesLoaded;
        }

        private void OnModulesLoaded(object sender, LoadModuleCompletedEventArgs e)
        {
            var providers = Container.ResolveAll<IDataProvider>();
            var restServices = Container.ResolveAll<IRestService>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            string path = string.Empty;
            if (HostingEnvironment.IsHosted && !Path.IsPathRooted(path))
                path = HostingEnvironment.MapPath("~/Modules");
            else
                path = @".\Modules";
            return new DirectoryModuleCatalog() { ModulePath = path };
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return null;
        }
    }
}
