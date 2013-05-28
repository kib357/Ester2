using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.PlanObjectTypes.Abstract;

namespace EsterCommon.Services
{
	public class UpdateInfo
	{
		public List<BaseObject> ChangedValues { get; set; }
		public bool UpdateObjectTree { get; set; }

		public UpdateInfo(bool updateTree = false)
		{
			ChangedValues = new List<BaseObject>();
		}
	}
}
