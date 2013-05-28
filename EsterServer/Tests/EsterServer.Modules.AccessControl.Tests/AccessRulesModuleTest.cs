using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ester.Model.Services;
using EsterCommon.ACL;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;
using NUnit.Framework;
using Newtonsoft.Json;
using Assert = NUnit.Framework.Assert;

namespace EsterServer.Modules.AccessControl.Tests
{
    [TestFixture]
    public class AccessRulesModuleTest
    {
        [Test]
        public void AddSeveralAccessRules()
        {
            var accessControl = new AccessControlModule();

            var answer = accessControl.GetEmployees();
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(data);

            answer = accessControl.GetCardReaders();
            reader = new StreamReader(answer);
            data = reader.ReadToEnd();
            var cardReaders = JsonConvert.DeserializeObject<List<CardReader>>(data);

            var rnd = new Random();
            var accessRules = new AccessRulesModule();
            var jsonSerializer = new JsonConvertWrapper();
            foreach (var employee in employees)
            {
                var aclItem = new AclItem
                {
                    AclObjectID = cardReaders[rnd.Next(1000)].Id,
                    AclSubjectID = employee.Id,
                    ActionID = rnd.Next(1, 10),
                    Access = true
                };
                var serializedCardRearer = jsonSerializer.Serialize(aclItem);
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(serializedCardRearer);
                writer.Flush();
                stream.Position = 0;

                answer = accessRules.AddAccessRule(stream);
            }
        }

        [Test]
        public void GetAccesRuleTest()
        {
            var accessRules = new AccessRulesModule();
            var answer = accessRules.GetAccessRule("30742B63-52C4-E211-BE73-001C42AEDAA6", "1157", "7");
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<AclItem>(data);
            //Assert.AreEqual(res[0], emp);
        }

        [Test]
        public void ChangeAccesRuleTest()
        {
            var aclItem = new AclItem()
                {
                    AclObjectID = new Guid("30742B63-52C4-E211-BE73-001C42AEDAA6"),
                    AclSubjectID = 1157,
                    ActionID = 7,
                    Access = false
                };
            var accessRules = new AccessRulesModule();
            var jsonSerializer = new JsonConvertWrapper();
            var serializedAclItem = jsonSerializer.Serialize(aclItem);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(serializedAclItem);
            writer.Flush();
            stream.Position = 0;

            var answer = accessRules.ChangeAccessRule("30742B63-52C4-E211-BE73-001C42AEDAA6", "1157", "7", stream);
            var reader = new StreamReader(answer);
            var data = reader.ReadToEnd();
            var res = JsonConvert.DeserializeObject<int>(data);
            Assert.AreEqual(res, 1);
        }

        [Test]
        public void DeleteAccessRuleTest()
        {
            var accessRules = new AccessRulesModule();
            accessRules.DeleteAccessRule("30742B63-52C4-E211-BE73-001C42AEDAA6", "1157", "7");
        }
    }
}
