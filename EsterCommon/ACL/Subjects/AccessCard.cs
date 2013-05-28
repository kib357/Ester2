using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.ACL.Subjects
{
    public class AccessCard
    {
        [Key, Column(Order = 0)]
        public int SiteCode { get; set; }
        [Key, Column(Order = 1)]
        public int Number { get; set; }
        public AccessCardStates AccessCardState { get; set; }
        public DateTime? StartDate { get; set; }    //дата начала работы карты
        public DateTime? EndDate { get; set; }    //дата окончания работы карты
    }
}