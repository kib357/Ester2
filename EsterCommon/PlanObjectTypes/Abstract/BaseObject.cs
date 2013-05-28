using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes.Data;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;

namespace EsterCommon.PlanObjectTypes.Abstract
{
	public abstract class BaseObject : NotificationObject
	{
		private GeometryData _path;
		public int Id { get; set; }
		public int? ParentId { get; set; }
		public string Type { get; set; }
		public int TypeId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public double Left { get; set; }
		public double Top { get; set; }
		public double? Width { get; set; }
		public double? Height { get; set; }
		public bool IsContainer { get; set; }

		public BaseObjectProperty[] Properties { get; set; }

		public GeometryData Path
		{
			get { return _path; }
			set { _path = value; RaisePropertyChanged("Path"); }
		}

		public virtual void Update(BaseObject newObject)
		{
			Description = newObject.Description;
			Height = newObject.Height;
			Id = newObject.Id;
			Left = newObject.Left;
			Name = newObject.Name;
			ParentId = newObject.ParentId;
			Path = newObject.Path;
			Top = newObject.Top;
			Type = newObject.Type;
			TypeId = newObject.TypeId;
			Width = newObject.Width;
		}

		public virtual List<Tuple<PropertyTypes, string>> GetChanges(BaseObject newObj)
		{
			throw new NotImplementedException();
		}
	}
}
