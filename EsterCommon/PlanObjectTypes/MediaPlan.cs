using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using System.Collections.ObjectModel;

namespace EsterCommon.PlanObjectTypes
{
	public class MediaPlan : BaseObject, IContainerObject
	{
		private readonly ObservableCollection<BaseObject> _children;
		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
		}

		public MediaPlan()
		{
			_children = new ObservableCollection<BaseObject>();
		}
	}
}
