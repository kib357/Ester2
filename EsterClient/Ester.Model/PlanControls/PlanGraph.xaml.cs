using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ester.Model.Events;
using Ester.Model.UserControls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Ester.Model.PlanControls
{
	/// <summary>
	/// Interaction logic for PlanGraph.xaml
	/// </summary>
	public partial class PlanGraph
	{
		const string HistoryUri = "/bacnet";
		private readonly Color[] _graphColours = new [] { Colors.Red, Colors.YellowGreen, Colors.Teal, Colors.BlueViolet, Colors.Chocolate, Colors.DeepPink, Colors.DarkBlue, Colors.Gold, Colors.OrangeRed };
		private readonly IEventAggregator _eventAggregator;
		private bool _dataLoaded;

		public ObservableCollection<GraphModel> Graphs { get; set; }
		public PlanGraph()
		{
			InitializeComponent();
			_eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
			Graphs = new ObservableCollection<GraphModel>();
			DataContext = Graphs;
		}

		private async void GetData()
		{
			var yAxis = new DateTimeAxis();
			byte c = 0;
			foreach (var address in AddressList)
			{
				var i = address.IndexOf('.');
				var deviceAdress = address.Substring(0, i);
				var objectAdress = address.Substring(i + 1, address.Length - i - 1);

				string requestUrl = string.Format("{0}/{1}/{2}/history",
					HistoryUri,
					deviceAdress,
					objectAdress
					);

				var a = await _dataTransport.GetRequestAsync<Dictionary<DateTime, string>>(requestUrl, true, 30000);

				if (a == null || a.Count == 0)
					continue;

				var model = new GraphModel
				{
					Address = address,
					Color = _graphColours[c%_graphColours.Count()],
					Name = UnitsList[c],
					Times = a.Keys.ToList(),
					Values = a.Values.Select(val => double.Parse(val.Replace('.', ','))).ToList()
				};

				var xDs = new EnumerableDataSource<DateTime>(model.Times);
				var yDs = new EnumerableDataSource<double>(model.Values);

				model.MinValue = model.Values.Min() - 1;
				model.MaxValue = model.Values.Max() + 1;

				//set the mappings
				xDs.SetXMapping(x => yAxis.ConvertToDouble(x));
				yDs.SetYMapping(y => y);

				model.PointDataSource = new CompositeDataSource(xDs, yDs);
				Graphs.Add(model);
				c++;
			}
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			if (_dataLoaded)
				return;

			GetData();
			_dataLoaded = true;
			Loaded -= OnLoad;
		}

		private void GraphOnMouseEnter(object sender, MouseEventArgs e)
		{
			_eventAggregator.GetEvent<ChangeZoomAllowedEvent>().Publish(false);
		}

		private void GraphOnMouseLeave(object sender, MouseEventArgs e)
		{
			_eventAggregator.GetEvent<ChangeZoomAllowedEvent>().Publish(true);
		}

		protected override void SetValues(KeyValuePair<string, string> sensor)
		{
			if(!_dataLoaded) return;
			if (Address.Contains(sensor.Key))
			{
				var graph = Graphs.FirstOrDefault(g => g.Address == sensor.Key);
				if (graph != null)
				{
					graph.Times.Add(DateTime.Now);
					graph.Values.Add(double.Parse(sensor.Value));
					
					var f = graph.PointDataSource.DataParts.First() as EnumerableDataSource<DateTime>;
					var l = graph.PointDataSource.DataParts.Last() as EnumerableDataSource<double>;

					if (f != null) f.RaiseDataChanged();
					if (l != null) l.RaiseDataChanged();
				}
			}
		}
	}
}
