using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using EsterCommon.BaseClasses;
using EsterCommon.Enums;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;

namespace Ester.Modules.Schedule.ViewModel
{
	public class Schedule
	{
		private ObservableCollection<DaySchedule> _daySchedules;
		private string _name;
		public int Id { get; set; }
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public ObservableCollection<DaySchedule> DaySchedules
		{
			get { return _daySchedules; }
			set { _daySchedules = value; }
		}

		public ScheduleTypes Type { get; set; }

		internal static Schedule FromScheduleClass(ScheduleClass value)
		{
			return value == null ? null : new Schedule()
			{
				Id = value.Id,
				Name = value.Name,
				Type = value.Type,
				DaySchedules = new ObservableCollection<DaySchedule>(
					value.Days.Select(n =>
						new DaySchedule()
						{
							Day = (byte)value.Days.ToList().IndexOf(n),
							Spans = new ObservableCollection<ValueTimeRange>(n.Select(m => new ValueTimeRange { Start = m.Start, Length = m.Length, Value = m.Value ?? value.Type.DefaultValue()}).ToList()),
							Time = TimeSpan.Zero,
							TooltipVisibility = Visibility.Collapsed,
							Value = value.Type.DefaultValue()
						}
					)
				)
			};
		}
	}

	public class DaySchedule : NotificationObject
	{
		private Visibility _tooltipVisibility;
		private double _left;
		private TimeSpan _time;
		private object _value;
		private ObservableCollection<ValueTimeRange> _spans;
		public byte Day { get; set; }
		public object Value
		{
			get { return _value; }
			set { _value = value; RaisePropertyChanged("Value"); }
		}

		public TimeSpan Time
		{
			get { return _time; }
			set { _time = value; RaisePropertyChanged("Time"); }
		}

		public Visibility TooltipVisibility
		{
			get { return _tooltipVisibility; }
			set { _tooltipVisibility = value; RaisePropertyChanged("TooltipVisibility"); }
		}

		public ObservableCollection<ValueTimeRange> Spans
		{
			get { return _spans; }
			set { _spans = value; RaisePropertyChanged("Spans"); }
		}

		public double Left
		{
			get { return _left; }
			set { _left = value; RaisePropertyChanged("Left"); }
		}
	}

// ReSharper disable ClassNeverInstantiated.Global
	public class SchedulesViewModel : NotificationObject, INavigationAware, IEsterViewModel
// ReSharper restore ClassNeverInstantiated.Global
	{
		public DelegateCommand<string> AddScheduleCommand { get; set; }
		public DelegateCommand SaveScheduleCommand { get; set; }
		public DelegateCommand DropScheduleCommand { get; set; }
		public DelegateCommand CancelCommand { get; set; }

		private readonly IEventAggregator _eventAggregator;
		private readonly IDataTransport _dataTransport;
		private SchedulesRepository _scheduleRepository;

		public ObservableCollection<ScheduleClass> Schedules
		{
			get { return _schedules; }
			set { _schedules = value; RaisePropertyChanged("Schedules"); }
		}

		public ScheduleTypes SelectedScheduleType
		{
			get { return _selectedScheduleType; }
			set { _selectedScheduleType = value; RaisePropertyChanged("SelectedScheduleType"); }
		}

		public SchedulesViewModel(IEventAggregator eventAggregator, IDataTransport dataTransport, IUnityContainer container)
		{
			Schedules = new ObservableCollection<ScheduleClass>();
			_eventAggregator = eventAggregator;
			_dataTransport = dataTransport;
			_scheduleRepository = container.Resolve<SchedulesRepository>();

			AddScheduleCommand = new DelegateCommand<string>(AddSchedule);
			SaveScheduleCommand = new DelegateCommand(SaveSchedule);
			DropScheduleCommand = new DelegateCommand(DropSchedule);
			CancelCommand = new DelegateCommand(Cancel);

			_selectedScheduleType = ScheduleTypes.Light;
		}

		private void Cancel()
		{
			if (CurrentSchedule != null)
			{
				if (CurrentSchedule.Id == 0)
				{
					CurrentSchedule = Schedule.FromScheduleClass(Schedules.FirstOrDefault());
				}
				else
				{
					CurrentSchedule = null;
					CurrentSchedule = Schedule.FromScheduleClass(SelectedSchedule);
				}
			}
		}

		public event ViewModelConfiguredEventHandler ViewModelConfiguredEvent;

		public void OnViewModelConfiguredEvent()
		{
			ViewModelConfiguredEventHandler handler = ViewModelConfiguredEvent;
			if (handler != null) handler(this);
		}

		public string Title { get { return "редактирование расписаний"; } }

		public bool IsReady { get; private set; }

		public void Configure()
		{
			Schedules = new ObservableCollection<ScheduleClass>(_scheduleRepository.Schedules.ToList());


			SelectedSchedule = Schedules.FirstOrDefault();

			IsReady = true;
			OnViewModelConfiguredEvent();
		}

		private async void DropSchedule()
		{
			if (CurrentSchedule != null && CurrentSchedule.Id != null)
			{
				IsBusy = true;
				try
				{
					const string dropScheduleUrl = @"/Schedules/";
					await _dataTransport.DeleteRequestAsync(dropScheduleUrl + CurrentSchedule.Id, true);
					Schedules.Remove(SelectedSchedule);
					SelectedSchedule = Schedules.FirstOrDefault();
				}
				catch
				{
					throw new Exception("Не удалось удалить расписание, попробуйте позже.");
				}
				finally
				{
					IsBusy = false;
				}
			}
		}

		private async void SaveSchedule()
		{
			if (CurrentSchedule == null)
				throw new Exception("Расписание не выбрано");

			if (string.IsNullOrWhiteSpace(CurrentSchedule.Name))
				throw new Exception("Не задано название расписания");

			try
			{
				IsBusy = true;
				var sc = Schedules.FirstOrDefault(s => s.Id == CurrentSchedule.Id) ?? new ScheduleClass();
				if (sc.Id == 0)
					sc.SetType(CurrentSchedule.Type);
				sc.Days = CurrentSchedule.DaySchedules.Select(s => new List<ValueTimeRange>(s.Spans)).ToArray();
				sc.Name = CurrentSchedule.Name;

				var savedSchedule = await _scheduleRepository.SaveSchedule(sc, CurrentSchedule.Id == 0);

				if (savedSchedule == null) throw new Exception("Не удалось сохранить расписание, попробуйте позже.");
				else
				{
					if (CurrentSchedule.Id != 0)
						Schedules.Remove(SelectedSchedule);

					Schedules.Add(savedSchedule);
					SelectedSchedule = savedSchedule;
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void AddSchedule(string name)
		{
			if (string.IsNullOrEmpty(name))
				return;

			CurrentSchedule = new Schedule
			{
				Name = name,
				Type = SelectedScheduleType,
				DaySchedules = new ObservableCollection<DaySchedule>(
					Enumerable.Range(0, 7).Select(r =>
						new DaySchedule
						{
							Day = (byte)r,
							Left = 0,
							TooltipVisibility = Visibility.Collapsed,
							Time = TimeSpan.Zero,
							Value = null,
							Spans = new ObservableCollection<ValueTimeRange>(
								Enumerable.Range(1, 1).Select(e =>
									new ValueTimeRange { Length = TimeSpan.FromMinutes(1439), Start = DateTime.MinValue, Value = SelectedScheduleType.DefaultValue() }
								)
							)
						}
					)
				)
			};
		}

		#region Properties

		public ScheduleClass SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				CurrentSchedule = Schedule.FromScheduleClass(value);
				RaisePropertyChanged("SelectedSchedule");
			}
		}

		private List<int> _dayTimeSpans = new List<int>(Enumerable.Range(1, 24));
		private ObservableCollection<ScheduleClass> _schedules;
		private bool _isBusy = false;

		public bool IsBusy
		{
			get { return _isBusy; }
			set { _isBusy = value; RaisePropertyChanged("IsBusy"); }
		}

		private Schedule _currentSchedule;
		private ScheduleTypes _selectedScheduleType;
		private ScheduleClass _selectedSchedule;

		public Schedule CurrentSchedule
		{
			get { return _currentSchedule; }
			set
			{
				_currentSchedule = value;
				RaisePropertyChanged("CurrentSchedule");
			}
		}

		public List<int> DayTimeSpans
		{
			get
			{
				return _dayTimeSpans;
			}
		}

		#endregion

		#region navigation

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			_eventAggregator.GetEvent<ToggleLeftPanelEvent>().Publish(true);
		}

		#endregion
	}
}
