using System.Collections.Generic;
using EsterCommon.ACL;

namespace EsterCommon.AccessControl
{
    public class CardReader : AclObject
    {
        public int Mode { get; set; }
        public virtual ICollection<CardReaderGroup> CardReaderGroups { get; set; }        
    }
}