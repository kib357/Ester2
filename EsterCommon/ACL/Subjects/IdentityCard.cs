using System;

namespace EsterCommon.ACL.Subjects
{
    public class IdentityCard
    {
        public Guid Id { get; set; }
        public IdentityCardTypes CardType { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }
        public string IssuedBy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public byte[] Photo { get; set; }
    }
}