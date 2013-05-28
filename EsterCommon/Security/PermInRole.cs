using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.Security
{
	public class PermInRole
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public Permission Permission {get;set;}
		public Role Role {get;set;}
		public bool? AttrCreate {get;set;}
		public bool? AttrRead { get; set; }
		public bool? AttrUpdate { get; set; }
		public bool? AttrDelete { get; set; }
		public bool? AttrExecute { get; set; }
	}
}
