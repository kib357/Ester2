using System.Collections.Generic;
using System.Collections.ObjectModel;
using EsterCommon.PlanObjectTypes.Abstract;

namespace EsterCommon.PlanObjectTypes.Interfaces
{
	public interface IContainerObject
	{
		ObservableCollection<BaseObject> Children { get; }
	}
}
