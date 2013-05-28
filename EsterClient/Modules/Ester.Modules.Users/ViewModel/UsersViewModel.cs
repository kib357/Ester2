using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Ester.Modules.Users.Model;



namespace Ester.Modules.Users.ViewModel
{
    public delegate void ShowAdImportEventHandler();
    public delegate void HideAdImportEventHandler();


    public class UsersViewModel : NotificationObject
    {
        private IEventAggregator _eventAggregator;
        private AdUsersRepository _usersRepository;
        private IDataTransport _dataTrasport;

        public DelegateCommand AddUserCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand OkAuthCommand { get; set; }
        public DelegateCommand CancleAuthCommand { get; set; }
        public DelegateCommand AuthCommand { get; set; }
        public DelegateCommand<object> ShowAdImportCommand { get; set; }
        
        private ObservableCollection<Domain> _domains; 
        public ObservableCollection<Domain> Domains
        {
            get { return _domains; }
            set { _domains = value; RaisePropertyChanged("Domains"); }
        }

        public event ShowAdImportEventHandler ShowAdImportEvent;
        public event HideAdImportEventHandler HideAdImportEvent;

        public UsersViewModel(IEventAggregator eventAggregator, AdUsersRepository usersRepository, IDataTransport dataTransport)
        {
            _eventAggregator = eventAggregator;
            _usersRepository = usersRepository;
            _dataTrasport = dataTransport;

            _domains=new ObservableCollection<Domain>();
            RefreshUsers();


            RefreshCommand = new DelegateCommand(RefreshUsers);
            CancelCommand = new DelegateCommand(HideWindow);


            _eventAggregator.GetEvent<ShowAdUsersImportEvent>().Subscribe(ShowWindow);

        }

        private void OnDomainsListDistribution(ObservableCollection<string> domains)
        {
            foreach(string domain in domains)
                _usersRepository.Domains.Add(domain);
        }

        public void ShowWindow(object obj)
        {
            ShowAdImportEvent();
        }

        private void HideWindow()
        {
            HideAdImportEvent();
        }


        private void RefreshUsers()
        {
            foreach (var domain in _usersRepository.Domains)
            {
                Domains.Add(new Domain(domain));
            }
        }

        private void GetDomains(string server, string login, string password)
        {
            //string uri = "/Users/GetForest/" + server + "/" + login + "/" + password;
            //try
            //{
            //    _dataTrasport.GetRequestAsync<ObservableCollection<string>>((result)=>
            //                                                               {
                                                                              
            //                                                               },
            //                                                               uri, true, 500);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        private void GetDomainUsers(string domain, string login, string password)
        {
            //string uri = "/Users/GetDomainUsers/" + domain + "/" + login + "/" + password ;
            //try
            //{
            //    _dataTrasport.GetRequestAsync<ObservableCollection<string>>((result)=>
            //                                                               {
                                                                               
            //                                                               },
            //                                                               uri, true, 5);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

        }

    }

   

   

}
