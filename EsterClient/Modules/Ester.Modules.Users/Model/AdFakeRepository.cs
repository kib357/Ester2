using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.BaseClasses;
using Ester.Model.Enums;

namespace Ester.Modules.Users.Model
{
    public class AdFakeRepository: IUsersRepository
    {
        private List<User> _users=new List<User>(); 

        public IEnumerable<User> GetUsers()
        {
            _users = new List<User>()
                       {
                           new User()
                               {
                                   IsActiveDirectoryUser = true,
                                   Login = "Admin",
                                   Domain = "ASTORIA",
                                   PasswordHash = @"xzHB4nVtDGbeO3Tl2yWQXRPNRhW0tkyurCUhpfJpt2A=",
                                   Sid = "S-1-5-21-3332492352-1950398402-596331667-1001",
                                   PasswordSalt = @"4MzYH97ZwgrzDYeoePaa8b\/tgthgFlcuzpRpD+chtdpT6m6aB+unHNPeKw25Cdbo",
                                   Role = UserRoles.Admin
                               },
                            new User()
                            {
                                IsActiveDirectoryUser = true,
                                Login = "Operator",
                                Domain = "Astoria",
                                PasswordHash = @"xzHB4nVtDGbeO3Tl2yWQXRPNRhW0tkyurCUhpfJpt2A=",
                                Sid = "S-1-5-21-3332492352-1950398402-596331667-1002",
                                PasswordSalt = @"4MzYH97ZwgrzDYeoePaa8b\/tgthgFlcuzpRpD+chtdpT6m6aB+unHNPeKw25Cdbo",
                                Role = UserRoles.Operator
                            },
                            new User()
                                {
                                    IsActiveDirectoryUser = true,
                                    Login = "Artemiy",
                                    Domain = "Artemiy-PC",
                                    Sid = "S-1-5-21-3332492352-1950398402-596331667-1000",
                                    Role = UserRoles.Operator
                                }
                          
                       };
            return _users;
        }

        public void EditUser(User user)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        IEnumerable<User> IUsersRepository.GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
