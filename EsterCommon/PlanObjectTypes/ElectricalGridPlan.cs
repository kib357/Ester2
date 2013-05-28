using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using System.Collections.ObjectModel;

namespace EsterCommon.PlanObjectTypes
{
	public class ElectricalGridPlan : BaseObject, IContainerObject
	{
		private readonly ObservableCollection<BaseObject> _children;
		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
		}

		public ElectricalGridPlan()
		{
			_children = new ObservableCollection<BaseObject>();
		}
	}
}
