using System;
using System.Collections.Generic;
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
using Ester.Model;
using Ester.Model.Enums;
using Ester.Modules.InternalErrors.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Ester.Modules.InternalErrors.View
{
    /// <summary>
    /// Interaction logic for InternalErrorsView.xaml
    /// </summary>
    public partial class InternalErrorsView : UserControl
    {
        private IRegionManager _regionManager;
        public InternalErrorsView(RegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dc = e.NewValue as InternalErrorsViewModel;
            if (dc == null) return;

            dc.ShowViewEvent += ShowView;
            dc.HideViewEvent += HideView;
        }

        private void ShowView()
        {
            _regionManager.Regions[RegionNames.ErrorsRegion].Activate(this);
        }

        private void HideView()
        {
            _regionManager.Regions[RegionNames.ErrorsRegion].Deactivate(this);
        }
    }
}
