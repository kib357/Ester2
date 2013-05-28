using System;
using System.Collections.Generic;
using EsterCommon.BaseClasses;
using Microsoft.Practices.Prism.ViewModel;

namespace Ester.Model.BaseClasses
{
	public class TemplateObject : NotificationObject
	{
		private int _id;
		public int Id
		{
			get { return _id; }
			set { _id = value; RaisePropertyChanged("Id"); }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; RaisePropertyChanged("Name"); }
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; RaisePropertyChanged("IsSelected"); }
		}

		private List<string> _doorList;
		public List<string> DoorList
		{
			get { return _doorList; }
			set { _doorList = value; RaisePropertyChanged("DoorList"); }
		}

		public TemplateObject()
		{
			InitializeFields();
		}

		private void InitializeFields()
		{
			Id = 0;
			Name = "";
			IsSelected = false;
			DoorList = new List<string>();
		}

		public TemplateObject(Person person)
		{
			Id = person.Id;
			Name = person.LastName;
			IsSelected = false;
			DoorList = person.DoorList;
		}
	}
}
