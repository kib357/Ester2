using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.ACL
{
    public abstract class AclObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool AvailableForGuests { get; set; }
        public ICollection<AclItem> AclItems { get; set; }
    }
}