using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;
using EsterCommon.ACL;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;
using EsterCommon.IntruderAlarm;

namespace EsterCommon.Data
{
    public class EsterContext : DbContext
    {
        public EsterContext() : base("name=Ester2")
        {}
        
        public DbSet<AclItem> Acls { get; set; }
        public DbSet<AclSubject> AclSubjects { get; set; }
        public DbSet<AclObject> AclObjects { get; set; }
        public DbSet<CardReader> CardReaders { get; set; }
        public DbSet<CardReaderGroup> CardReaderGroups { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonGroup> PersonGroups { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<IntruderAlarmArea> IntruderAlarmAreas { get; set; }
        public DbSet<IntruderAlarmAreaGroup> IntruderAlarmAreaGroups { get; set; }

        public override int SaveChanges()
        {            
            return base.SaveChanges();
        }
    }
}
