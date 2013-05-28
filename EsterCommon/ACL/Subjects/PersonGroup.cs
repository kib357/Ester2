using System.Collections.Generic;

namespace EsterCommon.ACL.Subjects
{
    public class PersonGroup : AclSubject
    {
        public string Name { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
        public bool AvailableForGuests { get; set; }
    }
}