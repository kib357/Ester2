using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using EsterCommon.PlanObjectTypes;
using PlansDb;

namespace PlanEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ObservableCollection<Plan> Plans = new ObservableCollection<Plan>();

		public MainWindow()
		{
			InitializeComponent();

			//var db = new PlansDc();

			//var plans = db.PlanObjects.Where(o => o.Parent == null).ToList();

			//List<Plan> Plans = new List<Plan>();
			//foreach (var planObject in plans)
			//{
			//	Plans.Add((Plan)BaseObject.FromDbObject(planObject));
			//}

			//DataContext = Plans;
		}

		//private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
		//{
		//	if (e.Key == Key.F5)
		//	{
		//		var db = new PlansDc();

		//		var plan = db.PlanObjects.FirstOrDefault(w => w.Parent == null);

		//		Plan viewPlan = (Plan)BaseObject.FromDbObject(plan);
		//		DataContext = viewPlan;
		//	}
		//}

		private void LoadfromSvgClick(object sender, RoutedEventArgs e)
		{
			//Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			//dlg.DefaultExt = ".svg";
			//dlg.Filter = "SVG files (.svg)|*.svg";

			//Nullable<bool> result = dlg.ShowDialog();
			//if (result == true)
			//{
			//	// Open document 
			//	string filename = dlg.FileName;
			//	var document = XDocument.Load(filename);



			//}

			var db = new PlansDc();
			#region rooms

			var r = db.PlanObjects.Single(o => o.Id == 13);
			r.Geometry = XElement.Parse("<path fill=\"none\" stroke=\"#000000\" d=\"M121.666,142.587H57.333V48.917h145.333v33 M150,142.587h52.667v-28.669 M202.667,190.418v25.666H57.333v-73.667l64.333,0.169 M202.667,163.751v-21.164H150 M202.667,81.917v-33h134.667v27M202.667,113.917v28.669h134.667v-36.336 M337.33,75.75V48.92h64v167.16h-73.12 M202.67,190.42v25.66h91.58 M337.33,106.08v36.34H202.67v21.16 M289.73,273.115c-10.943-2.66-22.605-8.908-34.397-20.7c0-21,0-36.331,0-36.331h38.917 M328.208,216.084h25.125v36.331c0,0-13.468,14.125-33.604,19.89\"/>");

			var grass = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#52AC62\" stroke=\"#52AC62\" d=\"M0,40.654C2.667-2.635,38.667,0.032,38.667,0.032L436,0c0,0,52.667,8.033,52.667,38.032s0,284,0,284H342v-38.667l92,0.667V22.699H41.333l-1.333,250l220,0.667v48.667H0C0,322.032,0,82.699,0,40.654z\"/>"),
				Name = "Травка",
				Parent = r,
				TypeId = 5
			};
			var r1 = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#6CA9E0\" d=\"M337.333,142.417H202.667v-93.5h134.667V142.417z\"/>"),
				Name = "101",
				Parent = r
				,
				TypeId = 6
			};
			var r2 = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#6CA9E0\" d=\"M202.667,142.587H57.333V48.917h145.333V142.587z\"/>"),
				Name = "102",
				Parent = r,
				TypeId = 6
			};
			var r3 = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#6CA9E0\" d=\"M202.667,216.084H57.333v-73.498h145.333V216.084z\"/>"),
				Name = "103",
				Parent = r,
				TypeId = 6
			};
			var r4 = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#6CA9E0\" d=\"M202.67,216.08V143h134.663l-0.003-94.08h64v167.16H202.67\"/>"),
				Name = "104",
				Parent = r,
				TypeId = 6
			};
			var r5 = new PlanObject
			{
				Geometry =
					XElement.Parse(
						"<path fill=\"#6CA9E0\" d=\"M319.729,272.305l-29.998,0.81c0,0-15.23-1.99-34.397-20.7c0-17.165,0-36.331,0-36.331h98c0,0,0,22.479,0,36.331C337.063,268.188,319.729,272.305,319.729,272.305\"/>"),
				Name = "105",
				Parent = r,
				TypeId = 6
			};

			#endregion

			db.PlanObjects.InsertOnSubmit(r1);
			db.PlanObjects.InsertOnSubmit(r2);
			db.PlanObjects.InsertOnSubmit(r3);
			db.PlanObjects.InsertOnSubmit(r4);
			db.PlanObjects.InsertOnSubmit(r5);

			//db.SubmitChanges();
		}
	}
}
