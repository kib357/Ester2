using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.BaseClasses;
using Microsoft.Practices.Prism.ViewModel;

namespace Ester.Modules.Users.Model
{
    public class Domain : NotificationObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; RaisePropertyChanged("Users"); }
        }

        public Domain(string name)
        {
            _name = name;
            _users = new ObservableCollection<User>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Domain;
            if (other == null) return false;
            return Name.ToLower() == other.Name.ToLower();
        }

        public override int GetHashCode()
        {
            return Name.ToLower().GetHashCode();
        }
    }

    class DomainComparer : IEqualityComparer<Domain>
    {
        public bool Equals(Domain x, Domain y)
        {
            return x.Name.ToLower() == y.Name.ToLower();
        }

        public int GetHashCode(Domain obj)
        {
            return (obj.Name.GetHashCode());
        }
    }
}
