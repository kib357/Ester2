using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Container = EsterCommon.PlanObjectTypes.Container;

namespace Ester.Modules.Building.ViewModel
{
	public class BuildingViewModel : NotificationObject, IEsterViewModel
	{
		private readonly PlanObjectsRepository _planObjectsrepository;

		private bool _isReady;
		private Model.ViewModes _mode;
		private ObservableCollection<BaseObject> _plans;
		private ListCollectionView _subsystems;
		private Container _selectedContainer;
		private IRegionManager _regionManager;
		private ObservableCollection<BaseObject> SubsystemItems { get; set; }

		public DelegateCommand TestCommand { get; private set; }

		public DelegateCommand<BaseObject> SaveObjectCommand { get; private set; }

		public Container SelectedContainer
		{
			get { return _selectedContainer; }
			set
			{
				if (Equals(value, _selectedContainer)) return;
				_selectedContainer = value;
				RaisePropertyChanged("SelectedContainer");
			}
		}

		public ObservableCollection<BaseObject> Plans
		{
			get { return _plans; }
			private set { _plans = value; RaisePropertyChanged("Plans"); }
		}

		public ObservableCollection<Room> SelectedRooms { get; private set; }

		public ListCollectionView Subsystems
		{
			get { return _subsystems; }
			private set { _subsystems = value; RaisePropertyChanged("Subsystems"); }
		}

		public Model.ViewModes Mode
		{
			get { return _mode; }
			set { _mode = value; RaisePropertyChanged("Mode"); Subsystems.Refresh(); }
		}

		public BuildingViewModel(IUnityContainer container, IRegionManager regionManager)
		{
			_planObjectsrepository = container.Resolve<PlanObjectsRepository>();
			_regionManager = regionManager;
			TestCommand = new DelegateCommand(Test);
			SaveObjectCommand = new DelegateCommand<BaseObject>(SaveObject);

			SubsystemItems = new ObservableCollection<BaseObject>();
			SelectedRooms = new ObservableCollection<Room>();
		}

		private void SaveObject(BaseObject obj)
		{
			if (obj == null)
				return;
			_planObjectsrepository.SubmitProperties(obj);
		}

		private bool FilterSubsystemsByType(object o)
		{
			switch (Mode)
			{
				case Model.ViewModes.Overview:
					return true;
				case Model.ViewModes.Microclimat:
					return ((BaseObject)o).TypeId == (int)ObjectTypes.TemperatureSensor;
				case Model.ViewModes.Ventilation:
					return ((BaseObject)o).TypeId == (int)ObjectTypes.AC;
				default:
					return false;
			}
		}

		void SelectedRooms1CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (Room newRoom in e.NewItems)
						newRoom.Children.Where(c => c is Subsystem).ToList().ForEach(i => SubsystemItems.Add(i));
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Room oldRoom in e.OldItems)
						oldRoom.Children.Where(c => c is Subsystem).ToList().ForEach(i => SubsystemItems.Remove(i));
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (Room oldRoom in e.OldItems)
						oldRoom.Children.Where(c => c is Subsystem).ToList().ForEach(i => SubsystemItems.Remove(i));
					foreach (Room newRoom in e.NewItems)
						newRoom.Children.Where(c => c is Subsystem).ToList().ForEach(i => SubsystemItems.Add(i));
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Reset:
					SubsystemItems.Clear();
					Subsystems.Refresh();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Test()
		{
			_regionManager.RequestNavigate(RegionNames.MainRegion, new Uri(ViewNames.StartView, UriKind.Relative));
		}

		#region IEsterViewModel
		public event ViewModelConfiguredEventHandler ViewModelConfiguredEvent;

		public void OnViewModelConfiguredEvent()
		{
			ViewModelConfiguredEventHandler handler = ViewModelConfiguredEvent;
			if (handler != null) handler(this);
		}

		public bool IsReady
		{
			get { return _isReady; }
		}

		public string Title
		{
			get { return "Здание"; }
		}

		public void Configure()
		{
			Subsystems = (ListCollectionView)CollectionViewSource.GetDefaultView(SubsystemItems);

			SelectedRooms.CollectionChanged += SelectedRooms1CollectionChanged;
			Subsystems.Filter += FilterSubsystemsByType;

			Plans = _planObjectsrepository.RootObjects;
			SelectedContainer = Plans.FirstOrDefault() as Container;

			_planObjectsrepository.DataUpdatedEvent += sender =>
			{
				SelectedContainer = null;
				Application.Current.Dispatcher.Invoke(() => SelectedRooms.Clear());
				Plans = _planObjectsrepository.RootObjects;
				SelectedContainer = Plans.FirstOrDefault() as Container;
			};

			_isReady = true;
			OnViewModelConfiguredEvent();
		}

		#endregion
	}
}
