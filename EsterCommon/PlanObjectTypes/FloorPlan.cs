using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;

namespace EsterCommon.PlanObjectTypes
{
	public class FloorPlan : BaseObject, IContainerObject
	{
		private readonly ObservableCollection<BaseObject> _children;
		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
		}

		public FloorPlan()
		{
			_children = new ObservableCollection<BaseObject>();
		}
	}
}
