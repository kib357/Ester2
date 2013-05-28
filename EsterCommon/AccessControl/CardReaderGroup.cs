using System.Collections.Generic;
using EsterCommon.ACL;

namespace EsterCommon.AccessControl
{
    public class CardReaderGroup : AclObject
    {        
        public virtual ICollection<CardReader> CardReaders { get; set; }
    }
}