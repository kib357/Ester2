using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ester.Model.UserControls
{
	public class PlanObjectItem : ContentControl
	{
		public static readonly DependencyProperty LeftProperty;
		public static readonly DependencyProperty TopProperty;
		public static readonly RoutedEvent SelectedEvent;
		public static readonly RoutedEvent UnselectedEvent;
		public static readonly DependencyProperty IsSelectedProperty;

		public PlanObjectItem()
		{
		}

		static PlanObjectItem()
		{
			LeftProperty = DependencyProperty.Register("LeftProperty", typeof(Double), typeof(PlanObjectItem));
			TopProperty = DependencyProperty.Register("TopProperty", typeof(Double), typeof(PlanObjectItem));
			SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(PlanObjectItem));
			UnselectedEvent = Selector.SelectedEvent.AddOwner(typeof(PlanObjectItem));
			IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(PlanObjectItem));
		}

		public Double X
		{
			get { return (Double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		public Double Y
		{
			get { return (Double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		internal PlanObjectsSelector ParentPlantObject
		{
			get { return ItemsControl.ItemsControlFromItemContainer(this) as PlanObjectsSelector; }
		}

		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (ParentPlantObject != null)
				ParentPlantObject.NotifyItemClicked(this);
			e.Handled = true;
		}

		public event RoutedEventHandler Selected
		{
			add { AddHandler(SelectedEvent, value); }
			remove { RemoveHandler(SelectedEvent, value); }
		}

		public event RoutedEventHandler Unselected
		{
			add { AddHandler(UnselectedEvent, value); }
			remove { RemoveHandler(UnselectedEvent, value); }
		}

		public bool IsSelected
		{
			get
			{
				return (bool)GetValue(IsSelectedProperty);
			}
			set
			{
				SetValue(IsSelectedProperty, value);
				RaiseEvent(new RoutedEventArgs(IsSelected ? Selector.SelectedEvent : Selector.UnselectedEvent));
			}
		}
	}
}
