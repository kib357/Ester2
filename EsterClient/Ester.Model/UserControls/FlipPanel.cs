using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Ester.Model.UserControls
{
	public class FlipsPanel : Panel
	{
		public static readonly DependencyProperty FrontVisibleProperty = DependencyProperty.Register("FrontVisible", typeof(bool), typeof(FlipPanel), new PropertyMetadata(true, OnFrontVisibleChanged));
		public static readonly DependencyProperty SpinTimeProperty = DependencyProperty.Register("SpinTime", typeof(double), typeof(FlipPanel), new PropertyMetadata(1.0));
		public static readonly DependencyProperty SpinAxisProperty = DependencyProperty.Register("SpinAxis", typeof(Orientation), typeof(FlipPanel), new PropertyMetadata(Orientation.Horizontal, OnSpinAxisChanged));

		private static readonly Vector3D AxisX = new Vector3D(1, 0, 0);
		private static readonly Vector3D AxisY = new Vector3D(0, 1, 0);
		private static readonly Vector3D AxisZ = new Vector3D(0, 0, 1);

		private static readonly Material VisualHostMaterial = new DiffuseMaterial(Brushes.White);
		private static readonly MeshGeometry3D Mesh = Plane.CreateXY(AxisZ, 1, 1);

		private readonly ModelVisual3D _model = new ModelVisual3D();

		private Viewport2DVisual3D _frontVisual3D;
		private Viewport2DVisual3D _backVisual3D;

		private UIElement _frontElement;
		private UIElement _backElement;

		private Viewport3D _viewPort;
		private ModelVisual3D _contentContainer;
		private readonly AxisAngleRotation3D _rotation = new AxisAngleRotation3D(AxisY, 0);
		private readonly TranslateTransform3D _translation = new TranslateTransform3D();
		private readonly ScaleTransform3D _scale = new ScaleTransform3D(1, 1, 1);

		static FlipsPanel()
		{
			//Mesh = new MeshGeometry3D();
			//var offset = Mesh.Positions.Count;
			//Mesh.Positions.Add(new Point3D(-0.5, 0.5, 0));
			//Mesh.Positions.Add(new Point3D(-0.5, -0.5, 0));
			//Mesh.Positions.Add(new Point3D(0.5, -0.5, 0));
			//Mesh.Positions.Add(new Point3D(0.5, 0.5, 0));

			//Mesh.Normals.Add(AxisZ);
			//Mesh.Normals.Add(AxisZ);
			//Mesh.Normals.Add(AxisZ);
			//Mesh.Normals.Add(AxisZ);

			//Mesh.TextureCoordinates.Add(new Point(0, 0));
			//Mesh.TextureCoordinates.Add(new Point(0, 1));
			//Mesh.TextureCoordinates.Add(new Point(1, 1));
			//Mesh.TextureCoordinates.Add(new Point(1, 0));

			//Mesh.TriangleIndices.Add(offset + 0);
			//Mesh.TriangleIndices.Add(offset + 1);
			//Mesh.TriangleIndices.Add(offset + 2);
			//Mesh.TriangleIndices.Add(offset + 0);
			//Mesh.TriangleIndices.Add(offset + 2);
			//Mesh.TriangleIndices.Add(offset + 3);

			VisualHostMaterial.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
		}

		public FlipsPanel()
		{
			InitializeComponent();
			SizeChanged += HandleSizeChanged;
			Loaded += (s, e) => InvalidateMeasure();
			_contentContainer.Children.Add(_model);
			SetupModel();
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			if (visualAdded != _viewPort)
				throw new InvalidOperationException("Add children using the Front and Back properties");
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		private void InitializeComponent()
		{
			_viewPort = new Viewport3D
			{
				Camera = new PerspectiveCamera()
				{
					FieldOfView = 90,
					Position = new Point3D(0, 0, 100),
					LookDirection = new Vector3D(0, 0, -1),
				},
				SnapsToDevicePixels = true
			};

			_viewPort.Children.Add(new ModelVisual3D { Content = new AmbientLight(Colors.DarkGray) });
			_viewPort.Children.Add(new ModelVisual3D { Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)) });

			var transform = new Transform3DGroup();
			transform.Children.Add(new RotateTransform3D(_rotation));
			transform.Children.Add(_translation);
			transform.Children.Add(_scale);
			_contentContainer = new ModelVisual3D { Transform = transform };
			_viewPort.Children.Add(_contentContainer);
			Children.Add(_viewPort);
		}

		private static void OnFrontVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FlipsPanel)d).Spin();
		}

		private static void OnSpinAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FlipsPanel)d).SetupModel();
		}

		public bool FrontVisible
		{
			get { return (bool)GetValue(FrontVisibleProperty); }
			set { SetValue(FrontVisibleProperty, value); }
		}

		public double SpinTime
		{
			get { return (double)GetValue(SpinTimeProperty); }
			set { SetValue(SpinTimeProperty, value); }
		}

		public Orientation SpinAxis
		{
			get { return (Orientation)GetValue(SpinAxisProperty); }
			set { SetValue(SpinAxisProperty, value); }
		}

		private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 0)
				//_scale.ScaleY = e.NewSize.Height / e.NewSize.Width;

			SetupModel();
		}

		private void SetupModel()
		{
			Visual front = null;
			Visual back = null;

			if (_frontVisual3D != null)
			{
				front = _frontVisual3D.Visual;
				_frontVisual3D.Visual = null;
			}

			if (_backVisual3D != null)
			{
				back = _backVisual3D.Visual;
				_backVisual3D.Visual = null;
			}

			_frontVisual3D = new Viewport2DVisual3D
			{
				Geometry = Mesh,
				Material = VisualHostMaterial,
			};

			_backVisual3D = new Viewport2DVisual3D
			{
				Geometry = Mesh,
				Material = VisualHostMaterial,
				Transform = new RotateTransform3D(new AxisAngleRotation3D(SpinAxis == Orientation.Vertical ? AxisY : AxisX, 180))
			};

			_rotation.Axis = SpinAxis == Orientation.Vertical ? AxisY : AxisX;

			if (front != null)
				_frontVisual3D.Visual = front;
			if (back != null)
				_backVisual3D.Visual = back;

			_model.Children.Clear();
			_model.Children.Add(_frontVisual3D);
			_model.Children.Add(_backVisual3D);

			InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size constraint)
		{
			_viewPort.Measure(constraint);

			if (_frontElement != null)
				_frontElement.Measure(constraint);

			if (_backElement != null)
				_backElement.Measure(constraint);

			var size = base.MeasureOverride(constraint);
			//return size;

			return new Size(size.Width, _frontElement.DesiredSize.Height);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_viewPort.Arrange(new Rect(new Point(), arrangeBounds));

			if (_frontElement != null)
				_frontElement.Arrange(new Rect(new Point(), arrangeBounds));

			if (_backElement != null)
				_backElement.Arrange(new Rect(new Point(), arrangeBounds));

			return arrangeBounds;
		}

		public void Spin()
		{
			Front.InvalidateVisual();
			Back.InvalidateVisual();
			var rotationAnimation = new DoubleAnimation(FrontVisible ? 0 : 180, new Duration(TimeSpan.FromSeconds(SpinTime)));
			_rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotationAnimation);

			var translationAnimation = new DoubleAnimationUsingKeyFrames();
			translationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-0.5, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(SpinTime / 2.0)), new SineEase()));
			translationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(SpinTime)), new SineEase { EasingMode = EasingMode.EaseIn }));
			_translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, translationAnimation);
		}

		public UIElement Front
		{
			get { return _frontElement; }
			set
			{
				_frontVisual3D.Visual = null;
				_frontVisual3D.Visual = value;
				_frontElement = value;
			}
		}

		public UIElement Back
		{
			get { return _backElement; }
			set
			{
				_backVisual3D.Visual = null;
				_backVisual3D.Visual = value;
				_backElement = value;
			}
		}
	}
}
