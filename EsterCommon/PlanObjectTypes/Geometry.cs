using System.Collections.ObjectModel;
using System.Linq;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterCommon.Services;

namespace EsterCommon.PlanObjectTypes
{
	public class Geometry : BaseObject
	{
		public Geometry()
		{
		    var type = PlansDictionaries.PlanObjectTypes.FirstOrDefault(s => s.Value == "Geometry");
		    TypeId = type.Key;
		    Type = "Геометрия";
		}
	}
}
