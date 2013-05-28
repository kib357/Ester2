using System.Collections.Generic;
using EsterCommon.ACL;

namespace EsterCommon.IntruderAlarm
{
    public class IntruderAlarmArea : AclObject
    {
        public virtual ICollection<IntruderAlarmAreaGroup> AreaGroups { get; set; } 
    }
}
