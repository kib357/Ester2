using System;
using System.Collections.Generic;
using EsterCommon.AccessControl;

namespace EsterCommon.ACL.Subjects
{
    public abstract class Person : AclSubject
    {                
        public virtual ICollection<PersonGroup> PersonGroups { get; set; }
        public virtual ICollection<AccessCard> AccessCards { get; set; }
        public virtual ICollection<IdentityCard> IdentityCards { get; set; }
        public virtual ICollection<Auto> Autos { get; set; }
        public byte[] Photo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? AddDate { get; set; }   //дата добавления
    }
}