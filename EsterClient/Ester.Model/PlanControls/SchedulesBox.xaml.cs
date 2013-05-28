using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ester.Model.Events;
using Ester.Model.Extensions;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using EsterCommon.BaseClasses;
using EsterCommon.Data;
using EsterCommon.Enums;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Ester.Model.PlanControls
{
	/// <summary>
	/// Interaction logic for SchedulesBox.xaml
	/// </summary>
	public partial class SchedulesBox
	{
		private SchedulesRepository _schedulesRepository;

		public SchedulesBox()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this)) return;
			EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
			EventAggregator.GetEvent<ApplicationLoadedEvent>().Subscribe(OnSchedulesLoaded);
			_schedulesRepository = ServiceLocator.Current.GetInstance<IUnityContainer>().Resolve<SchedulesRepository>();
		}

		private bool _changedByServer = false;

		private void OnSchedulesLoaded(object obj)
		{
			switch (ScheduleType)
			{
				case ScheduleTypes.AC:
					Schedules = _schedulesRepository.ACSchedules;
					break;
				case ScheduleTypes.Heat:
					Schedules = _schedulesRepository.HeatSchedules;
					break;
				case ScheduleTypes.Light:
					Schedules = _schedulesRepository.LightSchedules;
					break;
				case ScheduleTypes.SKUD:
					Schedules = _schedulesRepository.SKUDSchedules;
					break;
				case ScheduleTypes.Ventilation:
					Schedules = _schedulesRepository.VentilationSchedules;
					break;
			}
			_changedByServer = true;
			if (AddressList.Any())
			{
				var currentSchedule = Schedules.FirstOrDefault(s => s.ControlledObjects.Contains(new Property()));
				CurrentSchedule = currentSchedule ?? Schedules[0];
			}
		}

		public static readonly DependencyProperty SchedulesProperty = DependencyProperty.Register(
	"Schedules", typeof(ObservableCollection<ScheduleClass>), typeof(SchedulesBox));

		public ObservableCollection<ScheduleClass> Schedules
		{
			get { return (ObservableCollection<ScheduleClass>)GetValue(SchedulesProperty); }
			set { SetValue(SchedulesProperty, value); }
		}

		public static readonly DependencyProperty ScheduleTypeProperty = DependencyProperty.Register(
	"ScheduleType", typeof(ScheduleTypes), typeof(SchedulesBox));

		public ScheduleTypes ScheduleType
		{
			get { return (ScheduleTypes)GetValue(ScheduleTypeProperty); }
			set { SetValue(ScheduleTypeProperty, value); }
		}

		public static readonly DependencyProperty CurrentScheduleProperty = DependencyProperty.Register(
"CurrentSchedule", typeof(ScheduleClass), typeof(SchedulesBox));

		public ScheduleClass CurrentSchedule
		{
			get { return (ScheduleClass)GetValue(CurrentScheduleProperty); }
			set { SetValue(CurrentScheduleProperty, value); }
		}

		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_changedByServer)
			{
				_changedByServer = false;
				return;
			}

			if (Schedules.IndexOf(CurrentSchedule) == 0)
			{
				foreach (var schedule in Schedules.Where(s => s.ControlledObjects.Any()).ToList())
				{
					//if (AddressList.Any())
					//{
					//	schedule.ControlledObjects.Remove(AddressList[0]);
					//	_schedulesRepository.SaveSchedule(schedule);
					//}
					//if (Address.Length == 2)
					//	TryPushValueToServerAsync(AddressList[1], false.ToString());
				}
			}
			else
			{
				//if (AddressList.Any())
				//{
				//	CurrentSchedule.ControlledObjects.Add(AddressList[0]);
				//	_schedulesRepository.SaveSchedule(CurrentSchedule);
				//}
				//if (Address.Length == 2)
				//	TryPushValueToServerAsync(AddressList[1], true.ToString());
			}
		}
	}
}
