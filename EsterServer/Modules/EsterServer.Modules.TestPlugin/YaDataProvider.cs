using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterServer.Model.Interfaces;

namespace EsterServer.Modules.TestPlugin
{
    public class YaDataProvider : IDataProvider
    {
        public string Name { get { return "YaDataProvider"; } }
        public ServerModuleStates State { get; set; }
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
    }
}
