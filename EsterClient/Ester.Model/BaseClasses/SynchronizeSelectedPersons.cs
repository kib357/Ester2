//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using EsterCommon.BaseClasses;

namespace Ester.Model.BaseClasses
{
    /// <summary>
    /// Custom behavior that synchronizes the list in <see cref="ListBox.SelectedItems"/> with a collection.
    /// </summary>
    /// <remarks>
    /// This behavior uses a weak event handler to listen for changes on the synchronized collection.
    /// </remarks>
    public class SynchronizeSelectedPersons : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(
                "Selections", typeof(IList), typeof(SynchronizeSelectedPersons), new PropertyMetadata(null, OnSelectionsPropertyChanged));

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Dependency property")]
        public IList Selections
        {
            get { return (IList)this.GetValue(SelectionsProperty); }
            set { this.SetValue(SelectionsProperty, value); }
        }

        private static void OnSelectionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SynchronizeSelectedPersons;
            if (behavior == null) return;

            if (behavior._currentWeakHandler != null)
            {
                behavior._currentWeakHandler.Detach();
                behavior._currentWeakHandler = null;
            }

            if (e.NewValue != null)
            {
                var notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    behavior._currentWeakHandler =
                        new WeakEventHandler<SynchronizeSelectedPersons, object, NotifyCollectionChangedEventArgs>(
                            behavior,
                            (instance, sender, args) => instance.OnSelectionsCollectionChanged(sender, args),
                            (listener) => notifyCollectionChanged.CollectionChanged -= listener.OnEvent);
                    notifyCollectionChanged.CollectionChanged += behavior._currentWeakHandler.OnEvent;
                }

                behavior.UpdateSelectedItems();
            }
        }

        private bool _updating;
        private WeakEventHandler<SynchronizeSelectedPersons, object, NotifyCollectionChangedEventArgs> _currentWeakHandler;        

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnSelectedItemsChanged;
            UpdateSelectedItems();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged += OnSelectedItemsChanged;

            base.OnDetaching();
        }

        private void OnSelectedItemsChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelections(e);
        }

        private void UpdateSelections(SelectionChangedEventArgs e)
        {
            ExecuteIfNotUpdating(
                () =>
                    {
                        if (Selections == null) return;

                        if (IsEmptyPersonSelectedFirst())
                        {
                            ProcessEmptyPersonSelectedFirst();
                        }
                        else
                        {
                            ProcessNonEmptyPersonSelectedFirst(e);
                        }
                    });
        }

        private bool IsEmptyPersonSelectedFirst()
        {
            return AssociatedObject.SelectedItems.Count > 1
                   && GetEmptyPersonIndex(AssociatedObject.SelectedItems) == 0;
        }

        private int GetEmptyPersonIndex(IList listItems)
        {
            for (int i=0; i<listItems.Count; i++)
            {
                var currentItem = AssociatedObject.SelectedItems[i];
                if (currentItem.ToString() == new Person().ToString())
                {
                    return i;
                }
            }

            return -1;
        }

        private void ProcessNonEmptyPersonSelectedFirst(SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                if (IsEmptyPersonExistInList())
                {
                    AssociatedObject.SelectedItems.Remove(item);
                    continue;
                }

                Selections.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Selections.Remove(item);
            }
        }

        private bool IsEmptyPersonExistInList()
        {
            return GetEmptyPersonIndex(AssociatedObject.SelectedItems) > 0;
        }

        private void ProcessEmptyPersonSelectedFirst()
        {
            for (int i = AssociatedObject.SelectedItems.Count - 1; i > 0; i--)
                AssociatedObject.SelectedItems.RemoveAt(i);
        }

        private void OnSelectionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSelectedItems();
        }

        private void UpdateSelectedItems()
        {
            ExecuteIfNotUpdating(
                () =>
                    {
                        if (AssociatedObject != null)
                        {
                            AssociatedObject.SelectedItems.Clear();
                            foreach (var item in this.Selections ?? new object[0])
                            {
                                AssociatedObject.SelectedItems.Add(item);
                            }
                        }
                    });
        }

        private void ExecuteIfNotUpdating(Action execute)
        {
            if (_updating) return;
            try
            {
                _updating = true;
                execute();
            }
            finally
            {
                _updating = false;
            }
        }
    }
}
