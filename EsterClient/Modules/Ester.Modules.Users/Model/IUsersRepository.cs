using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.BaseClasses;

namespace Ester.Modules.Users.Model
{
    public interface IUsersRepository
    {
        IEnumerable<User> GetUsers();
        void AddUser(User user);
        void EditUser(User user);
    }
}
