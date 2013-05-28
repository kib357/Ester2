using System;
using EsterCommon.ACL.Subjects;
using EsterCommon.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EsterCommon.Tests
{
    [TestClass]
    public class dbSetChangeTrackerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var db = new EsterContext())
            {
                var guest = new Guest();
                guest.FirstName = "Name";
                guest.LastName = "LastName";
                db.Guests.Add(guest);
                var changes = db.ChangeTracker.Entries<Guest>();
            }
        }
    }
}
