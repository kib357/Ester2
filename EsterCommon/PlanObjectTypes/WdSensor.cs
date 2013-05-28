using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;

namespace EsterCommon.PlanObjectTypes
{
	public class WdSensor : Subsystem
	{
		private BoolPropertyValue _isLeaked;

		public BoolPropertyValue IsLeaked
		{
			get { return _isLeaked; }
			set { _isLeaked = value; RaisePropertyChanged("IsLeaked"); }
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			IsLeaked = ((WdSensor)newObject).IsLeaked;
		}
	}
}