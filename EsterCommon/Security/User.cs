using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.Security
{
	public class User
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id {get;set;}
		public string Username {get;set;}
		public string PwdSalt {get;set;}
		public string PwdHash {get;set;}
		public string Apikey {get;set;}
		public int? Sid {get;set;}
		public DateTime LastActivity {get;set;}
		public ICollection<Role> Roles { get; set; } 

	}
}
