using System.Collections.Generic;
using System.Collections.ObjectModel;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;

namespace EsterCommon.PlanObjectTypes
{
	public class Room : BaseObject, IContainerObject
	{
		private ObservableCollection<BaseObject> _children;
		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
			private set { _children = value; RaisePropertyChanged("Children"); }
		}

		public Room()
		{
			_children = new ObservableCollection<BaseObject>();
		}
	}
}