using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.Security
{
	public class Role
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ICollection<PermInRole> Permissions {get;set;} 
	}
}
