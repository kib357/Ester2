using System;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace Ester.Model.Repositories
{
    public delegate void DataReceivedEventHandler(Repository sender);
    public delegate void DataUpdatedEventHandler(Repository sender);

	public abstract class Repository : NotificationObject
    {
        public abstract string Title { get; }
        public bool HasData { get { return _hasData; } }
        protected bool _hasData;

        public event DataReceivedEventHandler DataReceivedEvent;
        public event DataUpdatedEventHandler DataUpdatedEvent;
        protected virtual void OnDataReceivedEvent()
        {
            if (_hasData)
            {
                var handler = DataUpdatedEvent;
                if (handler != null) handler(this);
            }
            else
            {
                _hasData = true;
                var handler = DataReceivedEvent;
                if (handler != null) handler(this);
            }
        }       

        public abstract void LoadData();
        public abstract Task UpdateData();            
    }
}
