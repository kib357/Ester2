using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EsterServer.Modules.Data.Tests
{
    [TestClass]
    public class FakeDataProviderTests
    {
        private int _valuesCount;
        private const string Address = "F;d(32,38,0.7);a(12,14,1)";

        [TestMethod]
        public void PutAddressReturnFakeObject()
        {
            var provider = new FakeDataProvider();            
            var fo = provider.ParseAddress(Address);
            Assert.AreEqual(fo.MinValue, 32);
        }

        [TestMethod]
        public void StartProviderWithOneAddressGetTwoValues()
        {
            var provider = new FakeDataProvider();            
            var config = new Dictionary<string, object>();
            config.Add("Addresses", new List<string> {Address});
            provider.DataRecievedEvent += OnDataReceivedEvent;
            provider.Initialize(config);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.AreEqual(_valuesCount, 2);
        }

        private void OnDataReceivedEvent(Dictionary<string, string> values)
        {
            if (values.ContainsKey(Address))
                _valuesCount++;
        }
    }
}
