using System;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Ester.Model.UserControls
{
	public class GraphModel : NotificationObject
	{
		/// <summary>
		/// Gets or sets the point data source.
		/// </summary>
		/// <value>The point data source.</value>
		public CompositeDataSource PointDataSource
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name of the line graph.</value>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color
		{
			get;
			set;
		}

		public double MinValue { get; set; }
		public double MaxValue { get; set; }

		public List<DateTime> Times { get; set; }
		public List<double> Values { get; set; }

		public string Address { get; set; }
	}
}
