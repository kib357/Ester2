using System.Collections.Generic;
using EsterCommon.ACL;

namespace EsterCommon.IntruderAlarm
{
    public class IntruderAlarmAreaGroup : AclObject
    {
        public virtual ICollection<IntruderAlarmArea> Areas { get; set; }
    }
}
