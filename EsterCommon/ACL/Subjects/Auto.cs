using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsterCommon.ACL.Subjects
{
    public class Auto
    {
        [Key, Column(Order = 0)]
        public string Number { get; set; }
        [Key, Column(Order = 1)]
        public int Region { get; set; }
        public string Mark { get; set; }   //марка
        public string Model { get; set; }   //модель
        public string Color { get; set; }
        public byte[] Photo { get; set; }
    }
}
