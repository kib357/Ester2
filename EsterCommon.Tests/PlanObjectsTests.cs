using System;
using EsterCommon.Data;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EsterCommon.Tests
{
	[TestClass]
	public class PlanObjectsTests
	{
		[TestMethod]
		public void PutGeometryDataReturnSvgFormattedXElement()
		{
			var geometry = new PathData
				               {
					               GeometryType = "path",
					               Data = "M202.67,216.08V143h134.663l-0.003-94.08h64v167.16H202.67",
					               Fill = "#6CA9E0",
					               Stroke = "#52AC62"
				               };
			var element = geometry.ToSvg();
			Assert.AreEqual(element.Attribute("d").Value, "M202.67,216.08V143h134.663l-0.003-94.08h64v167.16H202.67");
			Assert.AreEqual(element.Attribute("fill").Value, "#6CA9E0");
			Assert.AreEqual(element.Attribute("stroke").Value, "#52AC62");
			Assert.AreEqual(element.Attribute("x").Value, "0");
			Assert.AreEqual(element.Attribute("y").Value, "0");
		}

		//[TestMethod]
		//public void PutBaseObjectsReturnPlanObject()
		//{
		//	var dbPlan = new PlanObject();
		//	dbPlan.PlanObjectType = new PlanObjectType();
		//	dbPlan.PlanObjectType.IsContainer = true;
		//	var jsonPlan = new Room();
		//	jsonPlan.Id = 1;
		//	jsonPlan.Left = 34;
		//	jsonPlan.Top = 22;
		//	jsonPlan.Name = "Room 1";
		//	jsonPlan.ParentId = 0;
		//	jsonPlan.Height = 120;
		//	jsonPlan.Width = 100;
		//	jsonPlan.TypeId = (int)ObjectTypes.Room;
		//	var sensor = new TemperatureSensor();
		//	jsonPlan.Children.Add(sensor);
		//	jsonPlan.ExportToDbObject(dbPlan);
		//	Assert.AreEqual(dbPlan.PlanObjects.Count, 1);
		//}
	}
}
