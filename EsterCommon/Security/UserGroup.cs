using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.Security
{
	public class UserGroup
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ICollection<User> Users {get;set;} 
		public ICollection<Role> Roles {get;set;} 
	}
}
