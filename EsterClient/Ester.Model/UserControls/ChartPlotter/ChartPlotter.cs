using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Ester.Model.UserControls.ChartPlotter
{
	public class ChartPlotter : Microsoft.Research.DynamicDataDisplay.ChartPlotter
	{
		private List<LineGraph> lineGraphLines = new List<LineGraph>();

		/// <summary>
		/// DependencyProperty for LineGraphs
		/// </summary>
		public static readonly DependencyProperty LineGraphsProperty = DependencyProperty.Register("LineGraphs", typeof(ObservableCollection<GraphModel>), typeof(ChartPlotter), new FrameworkPropertyMetadata(ChangeLineGraphs));

		/// <summary>
		/// Gets or sets the line graphs.
		/// </summary>
		/// <value>The line graphs.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "ok")]
		public ObservableCollection<GraphModel> LineGraphs
		{
			get
			{
				return (ObservableCollection<GraphModel>)GetValue(LineGraphsProperty);
			}
			set
			{
				SetValue(LineGraphsProperty, value);
				LineGraphs.CollectionChanged += OnLineGraphsCollectionChanged;
			}
		}

		/// <summary>
		/// Changes the line graphs.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="eventArgs">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ChangeLineGraphs(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
		{
			var chartPlotter = source as ChartPlotter;
			if (chartPlotter != null)
				chartPlotter.UpdateLineGraphs((ObservableCollection<GraphModel>)eventArgs.NewValue);
		}

		private void UpdateLineGraphs(ObservableCollection<GraphModel> lineGrphs)
		{
			LineGraphs = lineGrphs;
		}

		private void OnLineGraphsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					{
						foreach (GraphModel viewModel in e.NewItems)
						{
							LineGraph lineGraph = this.AddLineGraph(viewModel.PointDataSource, viewModel.Color, 1, viewModel.Name);
							lineGraph.Name = viewModel.Name;
							lineGraphLines.Add(lineGraph);
						}
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					{
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					{
						foreach (var oldItem in e.OldItems)
						{
							foreach (LineGraph line in lineGraphLines)
							{
								if (((GraphModel)oldItem).Name == line.Name)
								{
									Children.Remove(line);
									lineGraphLines.Remove(line);
								}
							}
						}
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					{
						bool bTemp = false;
						foreach (GraphModel viewModel in e.NewItems)
						{
							foreach (LineGraph line in lineGraphLines)
							{
								if (Children.Contains(line) && line.Name == viewModel.Name)
								{
									Children.Remove(line);
									lineGraphLines.Remove(line);
									bTemp = true;
									break;
								}
							}

							if (bTemp)
							{
								LineGraph lineGraph = this.AddLineGraph(viewModel.PointDataSource, viewModel.Color, 1, viewModel.Name);
								lineGraph.Name = viewModel.Name;
								lineGraphLines.Add(lineGraph);
								bTemp = false;
							}
						}
						break;
					}

				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					{
						Restrictions.Clear();
						Children.RemoveAll(typeof(LineGraph));
						lineGraphLines.Clear();
						break;
					}
			}

			double min = double.MaxValue, max = double.MinValue;
			foreach (var graphModel in LineGraphs)
			{
				if (graphModel.MaxValue > max) max = graphModel.MaxValue;
				if (graphModel.MinValue < min) min = graphModel.MinValue;
			}

			Restrictions.Clear();

			if (min != double.MaxValue && max != double.MinValue && max > min)
			{
				Restrictions.Add(new ViewportAxesRangeRestriction { YRange = new DisplayRange(min, max) });
			}
			//Viewport.FitToView();
			Viewport.Visible = new DataRect(new DateTimeAxis().ConvertToDouble(DateTime.Now.AddHours(-23)), 0, new DateTimeAxis().ConvertToDouble(DateTime.MinValue.AddHours(23.5)), 100);
		}
	}
}
