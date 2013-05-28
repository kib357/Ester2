using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ester.Model.Services;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;
using NUnit.Framework;
using Newtonsoft.Json;
using Assert = NUnit.Framework.Assert;

namespace EsterServer.Modules.AccessControl.Tests
{
    [TestFixture]
    public class AccessControlModuleTests
    {
        [Test]
        public void AddEmployeeTest()
        {
            var emp = new Employee
                {
                    FullName = "Васин Василий Васильевич",
                    HireDate = new DateTime(2012, 10, 25),
                    Phone = "+7(3452)736291",
                    Position = "Уборщик",
                    Department = "Уборщики"
                };

            var accessControl = new AccessControlModule();
            var jsonSerializer = new JsonConvertWrapper();
            var serializedEmp = jsonSerializer.Serialize(emp);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(serializedEmp);
            writer.Flush();
            stream.Position = 0;

            var answer = accessControl.AddEmployee(stream);
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<int>(data);
            Assert.AreEqual(res, 1);
        }

        [Test]
        public void ChangeEmployeeTest()
        {
            var emp = new Employee
            {
                Id = 4,
                FullName = "Иванов Иван Иванович",
                HireDate = new DateTime(2012, 10, 25),
                Phone = "+7(3452)736291",
                Position = "Уборщик",
                Department = "Уборщики"
            };

            var accessControl = new AccessControlModule();
            var jsonSerializer = new JsonConvertWrapper();
            var serializedEmp = jsonSerializer.Serialize(emp);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(serializedEmp);
            writer.Flush();
            stream.Position = 0;

            var answer = accessControl.ChangeEmployee(emp.Id.ToString(),stream);
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<int>(data);
            Assert.AreEqual(res, 1);
        }

        [Test]
        public void GetEmployeesTest()
        {
            var emp = new Employee
            {
                Id = 4,
                FullName = "Иванов Иван Иванович",
                HireDate = new DateTime(2012, 10, 25),
                Phone = "+7(3452)736291",
                Position = "Уборщик",
                Department = "Уборщики"
            };

            var accessControl = new AccessControlModule();
            var answer = accessControl.GetEmployees();
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<List<Employee>>(data);
            Assert.AreEqual(res[0], emp);
        }

        [Test]
        public void DeleteEmployeesTest()
        {
            var accessControl = new AccessControlModule();
            accessControl.DeleteEmployee("4");
        }

        [Test]
        public void AddSeveralEmployeesTest()
        {
            var employees = new List<Employee>();
            var rnd = new Random();
            var bytes = new byte[30];
            for (int i = 0; i < 1000; i++)
            {
                var emp = new Employee();
                rnd.NextBytes(bytes);
                emp.FullName = Encoding.UTF8.GetString(bytes);
                emp.HireDate = DateTime.MinValue.AddDays(rnd.Next(1000000,3000000));
                emp.Phone = rnd.Next(111111, 9999999).ToString();
                rnd.NextBytes(bytes);
                emp.Position = Encoding.UTF8.GetString(bytes);
                rnd.NextBytes(bytes);
                emp.Department = Encoding.UTF8.GetString(bytes);
                employees.Add(emp);
            }

            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            var accessControl = new AccessControlModule();
            var jsonSerializer = new JsonConvertWrapper();
            foreach (var employee in employees)
            {
                var serializedEmp = jsonSerializer.Serialize(employee);
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(serializedEmp);
                writer.Flush();
                stream.Position = 0;
                var answer = accessControl.AddEmployee(stream);
            }
            sWatch.Stop();
            TimeSpan tSpan = sWatch.Elapsed;
        }

        [Test]
        public void GetAndDeleteEmployeesTest()
        {
            var accessControl = new AccessControlModule();
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            var answer = accessControl.GetEmployees();
            sWatch.Stop();
            TimeSpan tSpan = sWatch.Elapsed;
            sWatch = new Stopwatch();
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<List<Employee>>(data);
            sWatch.Start();
            foreach (var employee in res)
            {
                accessControl.DeleteEmployee(employee.Id.ToString());
            }
            sWatch.Stop();
            tSpan = sWatch.Elapsed;
        }

        [Test]
        public void GetEmployeesDbSetTest()
        {
            var accessControl = new AccessControlModule();
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            var answer = accessControl.GetEmployeesDbSet();
            sWatch.Stop();
            TimeSpan tSpan = sWatch.Elapsed;
        }

        [Test]
        public void AddSeveralCardReadersTest()
        {
            var cardReaders = new List<CardReader>();
            var rnd = new Random();
            var bytes = new byte[30];
            for (int i = 0; i < 1000; i++)
            {
                var cardReader = new CardReader();
                rnd.NextBytes(bytes);
                cardReader.Name = Encoding.UTF8.GetString(bytes);
                cardReaders.Add(cardReader);
            }

            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            var accessControl = new AccessControlModule();
            var jsonSerializer = new JsonConvertWrapper();
            foreach (var cardReader in cardReaders)
            {
                var serializedCardReader = jsonSerializer.Serialize(cardReader);
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(serializedCardReader);
                writer.Flush();
                stream.Position = 0;
                var answer = accessControl.AddCardReader(stream);
            }
            sWatch.Stop();
            TimeSpan tSpan = sWatch.Elapsed;
        }
    }
}
