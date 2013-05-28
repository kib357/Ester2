using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Ester.Model.Services;
using Ester.Modules.Schedule.ViewModel;
using EsterCommon.BaseClasses;

namespace Ester.Modules.Schedule.View
{
	/// <summary>
	/// Interaction logic for ScheduleView.xaml
	/// </summary>
	public partial class ScheduleView : UserControl
	{
		private TimeSpan _minSpanLength = new TimeSpan(0, 0, 40, 0);
		private TimeSpan _fullDaySpan = new TimeSpan(0, 23, 59, 0);

		public ScheduleView()
		{
			InitializeComponent();
		}

		private void TimeSpanMouseMove(object sender, MouseEventArgs e)
		{
			// получили родителя

			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//// получили контекст 
			//var daySchedule = dayGird.DataContext as DaySchedule;

			//if (daySchedule == null)
			//	return;

			//var hitPosition = e.GetPosition(dayGird);

			//// обновили данные в модельке представления
			//daySchedule.Left = hitPosition.X;
			//daySchedule.Time = TimeSpan.FromMinutes(Math.Round((hitPosition.X / dayGird.ActualWidth) * 1440));
			//daySchedule.Value = ((sender as FrameworkElement).DataContext as ValueTimeRange).Value;
		}

		private void TimeSpanDragDelta(object sender, DragDeltaEventArgs e)
		{
			//// получили родителя
			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//// получили контекст 
			//var daySchedule = dayGird.DataContext as DaySchedule;
			//var valueSpan = (sender as FrameworkElement).DataContext;

			//if (daySchedule == null)
			//	return;

			//int index = daySchedule.Spans.IndexOf(valueSpan as ValueTimeRange);

			//if (index < 1)
			//	return;

			//var duration = TimeSpan.FromMinutes(e.HorizontalChange);
			//var currentSpan = daySchedule.Spans[index];
			//var previousSpan = daySchedule.Spans[index - 1];

			//if (duration.TotalMinutes > 0)
			//{
			//	// если пытаемся ужать меньше минимума
			//	if (index < daySchedule.Spans.Count - 1 && currentSpan.Length - duration < _minSpanLength)
			//	{
			//		// пытаемся добавить к следующему

			//		// ширина будет минимальна
			//		currentSpan.Length = _minSpanLength;

			//		// начало будет отстоять от следующего на минимальную длительность
			//		currentSpan.Start = daySchedule.Spans[index + 1].Start - _minSpanLength;
			//		previousSpan.Length = currentSpan.Start - previousSpan.Start;
			//		return;
			//	}

			//	if (index == daySchedule.Spans.Count - 1)
			//	{
			//		if ((currentSpan.Start + TimeSpan.FromMinutes(1) + duration).Day > 1)
			//		{
			//			// начнется в конце дня и будет длинной в 1 минуту
			//			currentSpan.Start = DateTime.MinValue + _fullDaySpan;
			//			currentSpan.Length = TimeSpan.Zero;
			//			previousSpan.Length = currentSpan.Start - previousSpan.Start + TimeSpan.FromMinutes(1);
			//			return;
			//		}
			//	}
			//}

			//if (duration.TotalMinutes < 0 && previousSpan.Length + duration <= _minSpanLength)
			//{
			//	previousSpan.Length = _minSpanLength;
			//	currentSpan.Start = previousSpan.Start + previousSpan.Length;

			//	if (index == daySchedule.Spans.Count - 1)
			//		currentSpan.Length = DateTime.MinValue + _fullDaySpan - currentSpan.Start;
			//	else
			//		currentSpan.Length = daySchedule.Spans[index + 1].Start - currentSpan.Start;
			//	return;
			//}

			//if (duration.TotalMinutes < 0 && currentSpan.Start.TimeOfDay < duration.Duration())
			//{
			//	currentSpan.Start = DateTime.MinValue;

			//	if (index == daySchedule.Spans.Count - 1)
			//		currentSpan.Length = _fullDaySpan;
			//	else
			//	{
			//		currentSpan.Length = daySchedule.Spans[index + 1].Start - currentSpan.Start;
			//		previousSpan.Length = currentSpan.Start - previousSpan.Start;
			//	}
			//	return;
			//}

			//currentSpan.Start += duration;
			//currentSpan.Length -= duration;
			//previousSpan.Length += duration;
		}

		private void TimeSpanMouseEnter(object sender, MouseEventArgs e)
		{
			//// получили родителя
			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//var daySchedule = dayGird.DataContext as DaySchedule;

			//if (daySchedule == null)
			//	return;

			//daySchedule.TooltipVisibility = Visibility.Visible;
		}

		private void TimeSpanMouseLeave(object sender, MouseEventArgs e)
		{
			//// получили родителя
			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//var daySchedule = dayGird.DataContext as DaySchedule;

			//if (daySchedule == null)
			//	return;

			//daySchedule.TooltipVisibility = Visibility.Collapsed;
		}

		private void TimeSpanLeftMouseUp(object sender, MouseButtonEventArgs e)
		{
			//// получили родителя
			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//// получили контекст 
			//var daySchedule = dayGird.DataContext as DaySchedule;

			//if (daySchedule == null)
			//	return;

			//var valueSpan = ((sender as FrameworkElement).DataContext as ValueTimeRange);

			//if (valueSpan == null)
			//	return;

			//if (valueSpan.Length <= _minSpanLength + _minSpanLength)
			//	return;

			//int index = daySchedule.Spans.IndexOf(valueSpan);
			//if (index < 0)
			//	return;

			//var leftSpan = new ValueTimeRange
			//{
			//	Value = valueSpan.Value,
			//	Start = valueSpan.Start,
			//	Length = (DateTime.MinValue + daySchedule.Time) - valueSpan.Start
			//};

			//var rightSpan = new ValueTimeRange
			//{
			//	Value = valueSpan.Value,
			//	Start = valueSpan.Start + leftSpan.Length,
			//	Length = valueSpan.Length - leftSpan.Length
			//};

			//if (leftSpan.Length <= _minSpanLength)
			//{
			//	rightSpan.Start += _minSpanLength - leftSpan.Length;
			//	rightSpan.Length -= _minSpanLength - leftSpan.Length;
			//	leftSpan.Length = _minSpanLength;
			//}

			//if (rightSpan.Length <= _minSpanLength)
			//{
			//	leftSpan.Length -= _minSpanLength - rightSpan.Length;
			//	rightSpan.Start = leftSpan.Start + leftSpan.Length;
			//	rightSpan.Length = _minSpanLength;
			//}

			//daySchedule.Spans.RemoveAt(index);
			//daySchedule.Spans.Insert(index, rightSpan);
			//daySchedule.Spans.Insert(index, leftSpan);

			//daySchedule.TooltipVisibility = Visibility.Collapsed;
		}

		private void RemoveTimeValueSpanClick(object sender, RoutedEventArgs e)
		{
			//// получили родителя
			//var dayGird = XAMLSearch.FindVisualParent<Grid>(sender as UIElement).SingleOrDefault(g => g.Name == "ScheduleDayGrid");

			//// проверили
			//if (dayGird == null)
			//	return;

			//// получили контекст 
			//var daySchedule = dayGird.DataContext as DaySchedule;

			//if (daySchedule == null)
			//	return;

			//var valueSpan = ((sender as FrameworkElement).DataContext as ValueTimeRange);

			//if (valueSpan == null)
			//	return;
			//int index = daySchedule.Spans.IndexOf(valueSpan);
			//if (index == 0)
			//	return;


			//var currentSpan = daySchedule.Spans[index];
			//var prevSpan = daySchedule.Spans[index - 1];

			//prevSpan.Length += currentSpan.Length;
			//daySchedule.Spans.Remove(currentSpan);
		}
	}
}
