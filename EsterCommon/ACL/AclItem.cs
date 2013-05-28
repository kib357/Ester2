using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EsterCommon.Schedules;

namespace EsterCommon.ACL
{
    public class AclItem
    {
        [Key, Column(Order = 0)]
        public Guid AclObjectID { get; set; }
        [Key, Column(Order = 1)]
        public int AclSubjectID { get; set; }
        [Key, Column(Order = 2)]
        public int ActionID { get; set; }

        public AclObject AclObject { get; set; }
        public AclSubject AclSubject { get; set; }        
        public AclActions Action { get; set; }

        public bool Access { get; set; }
        public Schedule Schedule { get; set; }
    }
}