using System.Linq;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using System.Collections.ObjectModel;
using EsterCommon.Services;

namespace EsterCommon.PlanObjectTypes
{
	public class Container : BaseObject, IContainerObject
	{
		private ObservableCollection<BaseObject> _children;
		
		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
			private set { _children = value; RaisePropertyChanged("Children"); }
		}

		public Container()
		{
			_children = new ObservableCollection<BaseObject>();
            var type = PlansDictionaries.PlanObjectTypes.FirstOrDefault(s => s.Value == "Container");
            TypeId = type.Key;
		    Type = "Контейнер";
		}
	}
}
