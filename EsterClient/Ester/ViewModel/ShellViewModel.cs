using System;
using System.Windows;
using Ester.Model.Enums;
using Ester.Model.Events;
using Ester.Model.Services;
using Ester.View;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Ester.ViewModel
{
	public class ShellViewModel : NotificationObject, IRegionMemberLifetime, INavigationAware
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IRegionManager _regionManager;
		private readonly EsterBootstrapper _bootstrapper;
		private IUnityContainer _container;
		private RenderTargetBitmap _overlayImage;
		private string _ovarlayTag;

		public DelegateCommand AppCloseCommand { get; private set; }

		public RenderTargetBitmap OverlayImage
		{
			get { return _overlayImage; }
			set
			{
				if (Equals(value, _overlayImage)) return;
				_overlayImage = value;
				RaisePropertyChanged("OverlayImage");
				OverlayTag = value == null ? "Off" : "On";
			}
		}
		public string OverlayTag
		{
			get { return _ovarlayTag; }
			set
			{
				if (value == _ovarlayTag) return;
				_ovarlayTag = value;
				RaisePropertyChanged("OverlayTag");
			}
		}

		public ShellViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IUnityContainer container, EsterBootstrapper bootstrapper)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_bootstrapper = bootstrapper;
			_container = container;
			_regionManager.AddToRegion(RegionNames.MainRegion, container.Resolve<StartView>());

			AppCloseCommand = new DelegateCommand(ApplicationClose);
		}

		private void ApplicationClose()
		{
			_eventAggregator.GetEvent<AppShutdownEvent>().Publish(null);
		}

		public bool KeepAlive
		{
			get { return true; }
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			var shell = _container.Resolve<Shell>();
			var rtb = new RenderTargetBitmap((int)shell.ActualWidth, (int)shell.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(shell);
			OverlayImage = rtb;
			navigationContext.NavigationService.Navigating += NavigationServiceOnNavigating;
		}
		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			navigationContext.NavigationService.Navigating -= NavigationServiceOnNavigating;
		}

		private void NavigationServiceOnNavigating(object sender, RegionNavigationEventArgs regionNavigationEventArgs)
		{
			if (regionNavigationEventArgs.NavigationContext.Uri.OriginalString != ViewNames.StartView) return;
			var s = (FrameworkElement)_container.Resolve<object>(regionNavigationEventArgs.NavigationContext.NavigationService.Journal.CurrentEntry.Uri.OriginalString);
			var rtb = new RenderTargetBitmap((int)s.ActualWidth, (int)s.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(s);
			OverlayImage = rtb;
		}
	}
}
