using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;
using EsterServer.Model.Data;
using EsterServer.Model.Services;
using Microsoft.Practices.Prism.Events;
using Newtonsoft.Json;

namespace EsterServer.Modules.AccessControl
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AccessControlModule : IAccessControlModule
    {
        private readonly IEventAggregator _eventAggregator;

        public AccessControlModule()
        {}

        public AccessControlModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public Stream GetEmployees()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.Employees, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public List<Employee> GetEmployeesDbSet()
        {
            List<Employee> res;
            using (var context = new ServerContext())
            {
                res = context.Employees.AsEnumerable().ToList();
            }
            return res;
        }

        public Stream AddEmployee(Stream stream)
        {
            return AddOrChangeEmployee(stream);
        }

        public Stream ChangeEmployee(string id, Stream stream)
        {
            return AddOrChangeEmployee(stream);
        }

        public void DeleteEmployee(string id)
        {
            int intId;
            if(int.TryParse(id, out intId))
                using (var context = new ServerContext())
                {
                    var employee = context.Employees.Find(intId);
                    context.Entry(employee).State = EntityState.Deleted;
                    context.SaveChanges();
                }
        }

        public Stream GetGuests()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.Guests, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddGuest(Stream stream)
        {
            return AddOrChangeGuest(stream);
        }

        public Stream ChangeGuest(string id, Stream stream)
        {
            return AddOrChangeGuest(stream);
        }

        public void DeleteGuest(string id)
        {
            int intId;
                if (int.TryParse(id, out intId))
                    using (var context = new ServerContext())
                    {
                        var guest = context.Guests.Find(intId);
                        context.Entry(guest).State = EntityState.Deleted;
                        context.SaveChanges();
                    }
        }

        public Stream GetPersonGroups()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.PersonGroups, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddPersonGroup(Stream stream)
        {
            return AddOrChangePersonGroup(stream);
        }

        public Stream ChangePersonGroup(string id, Stream stream)
        {
            return AddOrChangePersonGroup(stream);
        }

        public Stream GetCardReaders()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.CardReaders, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddCardReader(Stream stream)
        {
            return AddOrChangeCardReader(stream);
        }

        public Stream ChangeCardReader(string id, Stream stream)
        {
            return AddOrChangeCardReader(stream);
        }

        public Stream GetCardReaderModes(string id)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCardReader(string id)
        {
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
                using (var context = new ServerContext())
                {
                    var cardReader = context.CardReaders.Find(guidId);
                    context.Entry(cardReader).State = EntityState.Deleted;
                    context.SaveChanges();
                }
        }

        public Stream GetCardReaderGroups()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.CardReaderGroups, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddCardReaderGroups(Stream stream)
        {
            return AddOrChangeCardReaderGroup(stream);
        }

        public Stream ChangeCardReaderGroup(string id, Stream stream)
        {
            return AddOrChangeCardReaderGroup(stream);
        }

        public void DeleteCardReaderGroups(string id)
        {
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
                using (var context = new ServerContext())
                {
                    var cardReaderGroup = context.CardReaderGroups.Find(guidId);
                    context.Entry(cardReaderGroup).State = EntityState.Deleted;
                    context.SaveChanges();
                }
        }

        private static Stream AddOrChangeEmployee(Stream stream)
        {
            var changedEmployee = SerializeHelper.GetObjectFromStream<Employee>(stream);
            int res;
            using (var context = new ServerContext())
            {
                Employee originalEmployee;
                if (changedEmployee.Id == 0)
                {
                    originalEmployee = context.Employees.Attach(new Employee());
                    context.Entry(originalEmployee).State = EntityState.Added;
                }
                else
                {
                    originalEmployee = context.Employees.Find(changedEmployee.Id);
                    context.Entry(originalEmployee).State = EntityState.Modified;
                }
                context.Entry(originalEmployee).CurrentValues.SetValues(changedEmployee);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private static Stream AddOrChangeGuest(Stream stream)
        {
            var changedGuest = SerializeHelper.GetObjectFromStream<Guest>(stream);
            int res;
            using (var context = new ServerContext())
            {
                Guest originalGuest;
                if (changedGuest.Id == 0)
                {
                    originalGuest = context.Guests.Attach(new Guest());
                    context.Entry(originalGuest).State = EntityState.Added;
                }
                else
                {
                    originalGuest = context.Guests.Find(changedGuest.Id);
                    context.Entry(originalGuest).State = EntityState.Modified;
                }
                context.Entry(originalGuest).CurrentValues.SetValues(changedGuest);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private static Stream AddOrChangePersonGroup(Stream stream)
        {
            var changedPersonGroup = SerializeHelper.GetObjectFromStream<PersonGroup>(stream);
            int res;
            using (var context = new ServerContext())
            {
                PersonGroup originalPersonGroup;
                if (changedPersonGroup.Id == 0)
                {
                    originalPersonGroup = context.PersonGroups.Attach(new PersonGroup());
                    context.Entry(originalPersonGroup).State = EntityState.Added;
                }
                else
                {
                    originalPersonGroup = context.PersonGroups.Find(changedPersonGroup.Id);
                    context.Entry(originalPersonGroup).State = EntityState.Modified;
                }
                context.Entry(originalPersonGroup).CurrentValues.SetValues(changedPersonGroup);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private static Stream AddOrChangeCardReader(Stream stream)
        {
            var changedCardReader = SerializeHelper.GetObjectFromStream<CardReader>(stream);
            int res;
            using (var context = new ServerContext())
            {
                CardReader originalCardReader;
                if (changedCardReader.Id == new Guid())
                {
                    originalCardReader = context.CardReaders.Attach(new CardReader());
                    context.Entry(originalCardReader).State = EntityState.Added;
                }
                else
                {
                    originalCardReader = context.CardReaders.Find(changedCardReader.Id);
                    context.Entry(originalCardReader).State = EntityState.Modified;
                }
                context.Entry(originalCardReader).CurrentValues.SetValues(changedCardReader);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private static Stream AddOrChangeCardReaderGroup(Stream stream)
        {
            var changedCardReaderGroup = SerializeHelper.GetObjectFromStream<CardReaderGroup>(stream);
            int res;
            using (var context = new ServerContext())
            {
                CardReaderGroup originalCardReaderGroup;
                if (changedCardReaderGroup.Id == new Guid())
                {
                    originalCardReaderGroup = context.CardReaderGroups.Attach(new CardReaderGroup());
                    context.Entry(originalCardReaderGroup).State = EntityState.Added;
                }
                else
                {
                    originalCardReaderGroup = context.CardReaderGroups.Find(changedCardReaderGroup.Id);
                    context.Entry(originalCardReaderGroup).State = EntityState.Modified;
                }
                context.Entry(originalCardReaderGroup).CurrentValues.SetValues(changedCardReaderGroup);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }
    }
}
