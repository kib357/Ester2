using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using Ester.Model.BaseClasses;

namespace Ester.Model.PlanControls
{
	/// <summary>
	/// Interaction logic for Radiator.xaml
	/// </summary>
	public partial class ThreeWayChoke
	{
		public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
	"RotationAngle", typeof(int), typeof(ThreeWayChoke), new PropertyMetadata(0));

		public int RotationAngle
		{
			get { return (int)GetValue(RotationAngleProperty); }
			set { SetValue(RotationAngleProperty, value); }
		}

		public static readonly DependencyProperty ChokeAngleProperty = DependencyProperty.Register(
	"ChokeAngle", typeof(int), typeof(ThreeWayChoke), new PropertyMetadata(0));

		public int ChokeAngle
		{
			get { return (int)GetValue(ChokeAngleProperty); }
			set { SetValue(ChokeAngleProperty, value); }
		}

		public static readonly DependencyProperty ScaleTransformProperty = DependencyProperty.Register(
	"ScaleTransform", typeof(int), typeof(ThreeWayChoke), new PropertyMetadata(0));

		public int ScaleTransform
		{
			get { return (int)GetValue(ScaleTransformProperty); }
			set { SetValue(ScaleTransformProperty, value); }
		}

		public ThreeWayChoke()
		{
			InitializeComponent();
		}

		protected override void SetValues(KeyValuePair<string, string> sensor)
		{
			double tmp;
			if (AddressList.Length > 0 && AddressList[0] == sensor.Key && double.TryParse(sensor.Value, out tmp))
				Rotate.Angle = (int)Math.Round((tmp * 0.9));
		}
	}
}
