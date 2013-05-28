using System;

namespace EsterCommon.ACL.Subjects
{
    public class Employee : Person
    {
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public DateTime? HireDate { get; set; }    //дата приема на работу
        public string EmployeeNumber { get; set; }   //табельный номер

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Employee) obj);
        }

        protected bool Equals(Employee other)
        {
            return string.Equals(FullName, other.FullName) && string.Equals(Department, other.Department) && string.Equals(Position, other.Position) && HireDate.Equals(other.HireDate) && string.Equals(EmployeeNumber, other.EmployeeNumber);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FullName != null ? FullName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Department != null ? Department.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Position != null ? Position.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HireDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (EmployeeNumber != null ? EmployeeNumber.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}