using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
    public class AddressValue<T> : NotificationObject
    {
        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; RaisePropertyChanged("Address"); }
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        public AddressValue(string address, T value)
        {
            Address = address;
            Value = value;
        }
    }
}
