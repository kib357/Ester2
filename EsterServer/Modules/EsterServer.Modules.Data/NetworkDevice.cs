using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BacNetApi.Data;

namespace EsterServer.Modules.Data
{
	public class NetworkDevice
	{
		public string Title { get; set; }

		public uint Id { get; set; }

		public DateTime LastUpdated { get; set; }

		public string Status { get; set; }

		public List<PrimitiveObject> SubscribedObjects { get; set; }

		public List<string> Objects { get; set; }
	}
}
