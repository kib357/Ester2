using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.ACL;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;
using EsterCommon.IntruderAlarm;
using EsterCommon.Schedules;

namespace EsterServer.Model.Data
{
    public sealed class ServerContext : DbContext
    {
        public ServerContext()
            : base("name=Ester")
        {}

        public DbSet<AclItem> AclItems { get; set; }
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
        public DbSet<Schedule> Shedules { get; set; }

        public override int SaveChanges()
        {                        
            return base.SaveChanges();
        }
    }
}
