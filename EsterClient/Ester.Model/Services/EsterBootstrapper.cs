using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Ester.Model.Services
{
	public class EsterBootstrapper
	{
		private readonly List<Repository> _repositories = new List<Repository>();
		private readonly List<IEsterViewModel> _viewModels = new List<IEsterViewModel>();
		private readonly IUnityContainer _container;
		private readonly ISessionInfo _sessionInfo;
		private readonly IEventAggregator _eventAggregator;

		public event BootProgressChangedEventHandler BootProgressChangedEvent;

		private void OnBootProgressChangedEvent(string message)
		{
			var handler = BootProgressChangedEvent;
			if (handler != null)
				Application.Current.Dispatcher.BeginInvoke(new Action(() => handler(message)));
		}

		public EsterBootstrapper(IUnityContainer container, IEventAggregator eventAggregator, ISessionInfo sessionInfo)
		{
			_container = container;
			_sessionInfo = sessionInfo;
			_eventAggregator = eventAggregator;

			//_repositories.Add(container.Resolve<PeopleRepository>());
			//_repositories.Add(container.Resolve<PlansRepository>());
			_repositories.Add(container.Resolve<SchedulesRepository>());
			_repositories.Add(container.Resolve<PlanObjectsRepository>());

			//_viewModels.Add(container.Resolve<IEsterViewModel>("building"));
			//_viewModels.Add(container.Resolve<IEsterViewModel>("schedules"));
			////_viewModels.Add(container.Resolve<IEsterViewModel>("people"));
			//_viewModels.Add(container.Resolve<IEsterViewModel>("logs"));
		}

		public void BeginLoad(Guid apiKey)
		{
			_sessionInfo.ApiKey = apiKey;
			CheckForUpdates();
		}

		private void CheckForUpdates()
		{
			OnBootProgressChangedEvent("Проверка обновлений");
			var updates = _container.Resolve<IEsterViewModel>("updates");
			updates.ViewModelConfiguredEvent += OnUpdatesChecked;
			updates.Configure();
		}

		private void OnUpdatesChecked(IEsterViewModel sender)
		{
			if (sender.IsReady)
				StartRepositories();
			else
			{
				OnBootProgressChangedEvent("Для запуска программы необходимо установить обновления");
			}
		}

		private void StartRepositories()
		{
			OnBootProgressChangedEvent("Передача данных");
			foreach (var repository in _repositories.Where(r => !r.HasData))
			{
				repository.DataReceivedEvent += OnDataLoaded;
				repository.LoadData();
			}
		}

		private void OnDataLoaded(Repository sender)
		{
			if (sender.HasData)
				OnBootProgressChangedEvent("Загрузка завершена - " + sender.Title);
			if (_repositories.All(r => r.HasData))
				Application.Current.Dispatcher.BeginInvoke(new Action(ConfigureViewModels));
		}

		private void ConfigureViewModels()
		{
			OnBootProgressChangedEvent("Настройка приложения");
			_viewModels.AddRange(_container.ResolveAll<IEsterViewModel>());
			foreach (var esterViewModel in _viewModels)
			{
				esterViewModel.ViewModelConfiguredEvent += OnViewModelLoaded;
				esterViewModel.Configure();
			}
		}

		private void OnViewModelLoaded(IEsterViewModel sender)
		{
			if (sender.IsReady)
				OnBootProgressChangedEvent("Расширение готово - " + sender.Title);
			if (_viewModels.All(vm => vm.IsReady))
			{
				OnBootProgressChangedEvent("Открываемся...");
				_eventAggregator.GetEvent<ApplicationLoadedEvent>().Publish(null);
			}
		}
	}

	public delegate void BootProgressChangedEventHandler(string message);
}
