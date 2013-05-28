using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using Ester.Model.Services;
using Ester.View;
using Ester.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace Ester
{
	class Bootstrapper : UnityBootstrapper
	{
		private Splash _splashScreen;

		protected override DependencyObject CreateShell()
		{
			Container.RegisterInstance(Container.Resolve<StartView>());
			var shell = new Shell();
			Container.RegisterInstance(shell);
			return shell;
		}

		protected override async void InitializeShell()
		{
			base.InitializeShell();

			App.Current.MainWindow = (Window)Shell;
			App.Current.MainWindow.Loaded += MainWindowLoaded;
			var shellViewModel = Container.Resolve<ShellViewModel>();
			App.Current.MainWindow.DataContext = shellViewModel;
			await Task.Delay(150);
			App.Current.MainWindow.Show();

			var serverInfo = Container.Resolve<IServerInfo>();
			ServicePointManager.FindServicePoint(new Uri(serverInfo.CommonServerAddress)).ConnectionLimit = 100;
		}

		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			_splashScreen.Close();
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			CommonInstances.UnityContainer = Container;

			Container.RegisterInstance<ISessionInfo>(new SessionInfo());

			var serverInfo = new ServerInfo();
			serverInfo.Initialize();
			Container.RegisterInstance<IServerInfo>(serverInfo);

			var cardReader = new AccessCardReader(serverInfo.AccessCardReaderPort);
			Container.RegisterInstance(typeof(IAccessCardReader), cardReader);

			Container.RegisterInstance(Container.Resolve<DataTransport>());
			Container.RegisterInstance<IDataTransport>(Container.Resolve<DataTransport>());

			//Container.RegisterInstance<IPeopleRepository>("peopleRepository", Container.Resolve<PeopleRepository>());
			//Container.RegisterInstance<IPeopleRepository>("guestsRepository", Container.Resolve<GuestsRepository>());

			//Container.RegisterInstance(Container.Resolve<PeopleRepository>());
			Container.RegisterInstance(Container.Resolve<PlanObjectsRepository>());
			Container.RegisterInstance(Container.Resolve<SchedulesRepository>());

			//Container.RegisterType<IDocumentScanner, AbbyyDocumentScanner>();


			Container.RegisterInstance(Container.Resolve<EsterBootstrapper>());

		}

		protected override IModuleCatalog CreateModuleCatalog()
		{
			return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
		}

		protected override void ConfigureModuleCatalog()
		{
			_splashScreen = new Splash();
			_splashScreen.Show();

			base.ConfigureModuleCatalog();
		}
	}
}
