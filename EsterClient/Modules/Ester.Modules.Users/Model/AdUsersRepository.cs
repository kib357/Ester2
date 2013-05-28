using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.BaseClasses;

namespace Ester.Modules.Users.Model
{
    public class AdUsersRepository
    {
        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<string> Domains { get; set; }

        public AdUsersRepository()
        {
            Users=new ObservableCollection<User>();
            Domains=new ObservableCollection<string>();
        }


    }
}
