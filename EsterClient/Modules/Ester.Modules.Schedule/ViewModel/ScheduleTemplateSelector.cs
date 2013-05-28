using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ester.Model.Services;
using EsterCommon.Enums;

namespace Ester.Modules.Schedule.ViewModel
{
	public class ScheduleTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var element = container as FrameworkElement;
			//if (element != null && item != null)
			//{
			//	// получили родителя
			//	var dayGird = XAMLSearch.FindVisualParent<UserControl>(container as UIElement).SingleOrDefault(g => g.Name == "ScheduleViewControl");

			//	// проверили
			//	if (dayGird == null)
			//		return null;
			//	var daySchedule = dayGird.DataContext as SchedulesViewModel;

			//	if (daySchedule == null)
			//		return null;
			//	switch (daySchedule.CurrentSchedule.Type)
			//	{
			//		case ScheduleTypes.SKUD:
			//			return element.FindResource("SKUDEditorTemplate") as DataTemplate;
			//		case ScheduleTypes.Light:
			//			return element.FindResource("LigthEditorTemplate") as DataTemplate;
			//		case ScheduleTypes.Heat:
			//			return element.FindResource("HeatEditorTemplate") as DataTemplate;
			//		case ScheduleTypes.Ventilation:
			//			return element.FindResource("VentilationEditorTemplate") as DataTemplate;
			//		case ScheduleTypes.AC:
			//			return element.FindResource("ACEditorTemplate") as DataTemplate;
			//		default:
			//			throw new ArgumentOutOfRangeException();
			//	}
			//}
			return null;
		}
	}
}
