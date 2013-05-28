using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsterServer.Modules.Users.Wrappers
{
        public class DomainTree
        {
            public string Name { get; set; }
            public IEnumerable<DomainTree> Subdomains { get; set; }
        }
}
