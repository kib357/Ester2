using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;

namespace Ester.Model.UserControls
{
	public class PlanObjectsSelector : MultiSelector
	{
		public static bool CanSelect
		{
			get { return true; }
		}

		public PlanObjectsSelector()
		{
			CanSelectMultipleItems = true;
			ClipToBounds = true;

			var panel = new FrameworkElementFactory(typeof(Canvas));
			ItemsPanel = new ItemsPanelTemplate(panel);

			ItemContainerStyle = new Style();
			ItemContainerStyle.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("Left")));
			ItemContainerStyle.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Top")));
		}

		static PlanObjectsSelector()
		{
			SelectedItems = new List<BaseObject>();
			//SelectedRooms = new ObservableCollection<Room>();
		}

		internal void NotifyItemClicked(PlanObjectItem item)
		{
			if (!CanSelect)
				return;

			var dataItem = (BaseObject)ItemContainerGenerator.ItemFromContainer(item);

			if (!(dataItem is Room)) return;

			if (SelectedItems.Contains(dataItem))
				Unselect(item);
			else
				Select(item);
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var contentItem = element as FrameworkElement;

			if (contentItem == null)
				throw new NullReferenceException("Value cannot be null");

			var leftBinding = new Binding("X") {Source = contentItem};

			var topBinding = new Binding("Y") {Source = contentItem};

			contentItem.SetBinding(Canvas.LeftProperty, leftBinding);
			contentItem.SetBinding(Canvas.TopProperty, topBinding);

			base.PrepareContainerForItemOverride(element, item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PlanObjectItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PlanObjectItem;
		}

		//protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		//{
		//	base.OnMouseLeftButtonDown(e);
		//}

		//protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		//{
		//	base.OnMouseLeftButtonUp(e);
		//}

		private void Select(PlanObjectItem item)
		{
			if (!IsUpdatingSelectedItems)
			{
				BeginUpdateSelectedItems();

				SelectedItems.Add((BaseObject)ItemContainerGenerator.ItemFromContainer(item));
				item.IsSelected = true;

				/* ----!---- */
				SelectedRooms.Add((Room)ItemContainerGenerator.ItemFromContainer(item));

				EndUpdateSelectedItems();
			}
		}

		//private void Select(List<PlanObjectItem> items)
		//{
		//	if (!IsUpdatingSelectedItems)
		//	{
		//		BeginUpdateSelectedItems();

		//		items.ForEach(a =>
		//			{
		//				SelectedItems.Add((BaseObject)ItemContainerGenerator.ItemFromContainer(a));
		//				a.IsSelected = true;
		//			}
		//		);

		//		EndUpdateSelectedItems();
		//	}
		//}

		private void Unselect(PlanObjectItem item)
		{
			if (!IsUpdatingSelectedItems)
			{
				BeginUpdateSelectedItems();
				SelectedItems.Remove((BaseObject)ItemContainerGenerator.ItemFromContainer(item));
				item.IsSelected = false;
				/* ----!---- */
				SelectedRooms.Remove((Room)ItemContainerGenerator.ItemFromContainer(item));
				EndUpdateSelectedItems();
			}
		}

		//private void UnselectAllExceptThisItem(PlanObjectItem item)
		//{
		//	if (!IsUpdatingSelectedItems)
		//	{
		//		BeginUpdateSelectedItems();
		//		SelectedItems.Cast<object>().Where(a => item != ItemContainerGenerator.ContainerFromItem(a)).ToList().ForEach(a => SelectedItems.Remove((BaseObject)a));
		//		EndUpdateSelectedItems();
		//	}
		//}

		public static new List<BaseObject> SelectedItems { get; set; }

		public ObservableCollection<Room> SelectedRooms
		{
			get { return (ObservableCollection<Room>)GetValue(SelectedRoomsProperty); }
			set { SetValue(SelectedRoomsProperty, value); }
		}

		public static readonly DependencyProperty SelectedRoomsProperty =
			DependencyProperty.Register("SelectedRooms", typeof(ObservableCollection<Room>), typeof(PlanObjectsSelector), new UIPropertyMetadata(new ObservableCollection<Room>()));
	}
}
