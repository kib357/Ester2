using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Ester.Model.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Ester.Modules.InternalErrors.ViewModel
{
    public delegate void ShowViewEventHandler();
    public delegate void HideViewEventHandler();

    public class InternalErrorsViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        public event ShowViewEventHandler ShowViewEvent;
        public event HideViewEventHandler HideViewEvent;
        public List<Exception> ErrorList;

        public InternalErrorsViewModel(IEventAggregator eventAggregator)
        {
            ErrorList = new List<Exception>();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ShowErrorEvent>().Subscribe(OnShowError, ThreadOption.UIThread);
            HideViewCommand = new DelegateCommand(HideView);
        }

        public DelegateCommand HideViewCommand { get; private set; }
        private void HideView()
        {
            if (ErrorList.Count > 0)
                ErrorList.Remove(ErrorList.First());

            if (ErrorList.Count == 0)
            {
                if (HideViewEvent != null)
                    HideViewEvent();
            }
            else
                ShowView(ErrorList.First());
        }

        private void OnShowError(Exception ex)
        {
            if (ErrorList.Count == 0)
                ShowView(ex);
            ErrorList.Add(ex);
        }

        private void ShowView(Exception ex)
        {
            Title = ex.Message;
            Message = (ex.InnerException != null) ? ex.InnerException.Message : "";
            Background = new SolidColorBrush(Color.FromRgb(0xD1, 0x2C, 0x3A));
            if (ShowViewEvent != null)
                ShowViewEvent();
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    RaisePropertyChanged("Background");
                }
            }
        }
    }
}
