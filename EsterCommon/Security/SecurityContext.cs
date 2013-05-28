using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsterCommon.Security
{
	public class SecurityContext : DbContext
	{
		public SecurityContext() : base("name=Ester2")
        {}
        
        public DbSet<User> Users { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<PermInRole> PermInRoles { get; set; }
	}
}
