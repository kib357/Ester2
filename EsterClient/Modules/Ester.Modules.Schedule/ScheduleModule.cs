using Ester.Modules.Schedule.View;
using Ester.Modules.Schedule.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Ester.Modules.Schedule
{
	public class ScheduleModule : IModule
	{
		private readonly IUnityContainer _container;

		public ScheduleModule(IUnityContainer container)
		{
			_container = container;
		}

		public void Initialize()
		{
			var viewModel = _container.Resolve<SchedulesViewModel>();
			var scheduleView = _container.Resolve<ScheduleView>();
			_container.RegisterInstance("ScheduleView", scheduleView);
			_container.RegisterType<object, ScheduleView>("ScheduleView");
			scheduleView.DataContext = viewModel;
		}
	}
}
