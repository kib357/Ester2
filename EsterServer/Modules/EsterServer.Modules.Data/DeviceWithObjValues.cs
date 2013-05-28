using System.Collections.Generic;
using BACsharp;

namespace EsterServer.Modules.Data
{
	public class DeviceWithObjValues
	{
		public string DeviceId;
		public Dictionary<string, Dictionary<BacnetPropertyId, object>> ObjValues;
	}
}
