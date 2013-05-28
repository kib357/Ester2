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
using Ester.Modules.Users.ViewModel;
using Microsoft.Practices.Prism.Regions;

namespace Ester.Modules.Users.View
{
    /// <summary>
    /// Interaction logic for PeopleAdImportView.xaml
    /// </summary>
    public partial class ImportAdUsersView : UserControl
    {
        private IRegionManager _regionManager;


        public Visibility DomainAuthVisibility { get; set; }
        public Visibility DomainTreeVisibility { get; set; }

        public ImportAdUsersView(RegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dc = e.NewValue as UsersViewModel;
            if (dc == null) return;

            dc.ShowAdImportEvent += ShowView;
            dc.HideAdImportEvent += HideView;

            DomainTreeVisibility = Visibility.Hidden;
            
        }


     private void ShowView()
        {
            _regionManager.Regions[RegionNames.PopupRegion].Activate(this);
        }

        private void HideView()
        {
            _regionManager.Regions[RegionNames.PopupRegion].Deactivate(this);
        }

        


    }
}
