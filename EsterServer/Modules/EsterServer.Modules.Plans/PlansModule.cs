using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.Xml.Linq;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterCommon.Data;
using EsterCommon.Events;
using EsterCommon.Exceptions;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;
using EsterCommon.Services;
using EsterServer.Model.Extensions;
using EsterServer.Model.Ioc;
using EsterServer.Model.Services;
using Microsoft.Practices.Prism.Events;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace EsterServer.Modules.Plans
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PlansModule : IPlansModule
    {
        private readonly string _esterConnectionString = ConfigurationManager.ConnectionStrings["Ester"].ConnectionString;
        private readonly IEventAggregator _eventAggregator;

        public PlansModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public Stream Get()
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                var plans = context.PlanObjects.Where(p => p.ParentId == null);

                var res = new List<IContainerObject>();
                foreach (var planObject in plans)
                {
                    res.Add(ServerExtensions.FromDbObject(planObject) as IContainerObject);
                }
                responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream GetItem(string id)
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                var plan = GetPlanObjectById(id, context);
                responseBody = JsonConvert.SerializeObject(ServerExtensions.FromDbObject(plan), Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream GetItemProperties(string id)
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                var res = new List<BaseObjectProperty>();
                var plan = GetPlanObjectById(id, context);
                var properties = context.PropertyTypes.Where(a => a.PlanObjectType == plan.PlanObjectType);
                foreach (var addressType in properties)
                {
                    var property =
                        context.Properties.FirstOrDefault(p => p.ObjectId == plan.Id && p.AddressTypeId == addressType.Id);
                    if (property != null)
                        res.Add(new BaseObjectProperty
                            {

                                Id = property.Id,
                                Path = property.Path,
                                TypeName = addressType.Title,
                                TypeId = addressType.Id
                            });
                    else
                        res.Add(new BaseObjectProperty { Id = 0, Path = "", TypeName = addressType.Title, TypeId = addressType.Id });
                }
                responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream GetTypeProperties(string id)
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                int intId;
                if (!int.TryParse(id, out intId))
                    throw new BadRequestException("Unknown identifier format");
                var res = context.PropertyTypes.Where(a => a.TypeId == intId).ToDictionary(p1 => p1.Id, p2 => p2.Title);
                responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream Add(Stream stream)
        {
            var planObject = SerializeHelper.GetObjectFromStream<BaseObject>(stream, new JsonConverter[] { new PlanObjectConverter(), new GeometryDataConverter() });
            var dbOject = SavePlanObjectToDb(null, planObject);
            string responseBody = JsonConvert.SerializeObject(dbOject, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream Edit(string id, Stream stream)
        {
            var planObject = SerializeHelper.GetObjectFromStream<BaseObject>(stream, new JsonConverter[] { new PlanObjectConverter(), new GeometryDataConverter() });
            var dbOject = SavePlanObjectToDb(id, planObject);
            string responseBody = JsonConvert.SerializeObject(dbOject, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private BaseObject SavePlanObjectToDb(string id, BaseObject planObject)
        {
            BaseObject res;
            var plan = new PlanObject();
            using (var context = new PlansDc(_esterConnectionString))
            {
                if (!string.IsNullOrEmpty(id))
                    plan = GetPlanObjectById(id, context);
                else
                    context.PlanObjects.InsertOnSubmit(plan);
                planObject.ExportToDbObject(plan, context);
                context.SubmitChanges();
                res = ServerExtensions.FromDbObject(plan);
            }
            if (_eventAggregator != null)
                _eventAggregator.GetEvent<PlansModifiedEvent>().Publish(planObject);
            return res;
        }

        public void Delete(string id)
        {
            using (var context = new PlansDc(_esterConnectionString))
            {
                var plan = GetPlanObjectById(id, context);
                DeleteChilds(plan, context);
                context.PlanObjects.DeleteOnSubmit(plan);
                context.SubmitChanges();
            }
        }

        private void DeleteChilds(PlanObject plan, PlansDc context)
        {
            foreach (var child in plan.PlanObjects)
            {
                DeleteChilds(child, context);
                context.PlanObjects.DeleteOnSubmit(child);
            }
        }

        private static PlanObject GetPlanObjectById(string id, PlansDc context)
        {
            int intId;
            if (!int.TryParse(id, out intId))
                throw new BadRequestException("Unknown identifier format");
            var plan = context.PlanObjects.FirstOrDefault(p => p.Id == intId);
            if (plan == null)
                throw new BadRequestException("Object not found");
            return plan;
        }

        public Stream ImportSvg(Stream stream)
        {
            string responseBody;
            try
            {
                var doc = XDocument.Load(stream);
                _importedObjects = new List<BaseObject>();
                if (doc.Root != null) FindGeometry(doc.Root.Elements(), null);
                responseBody = JsonConvert.SerializeObject(_importedObjects, Formatting.Indented);
            }
            catch (Exception)
            {
                throw new BadRequestException("Error while parsing XML");
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream GetPlanElements(string id)
        {
            throw new NotImplementedException();
        }

        public Stream GetUnits()
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                var units = context.Units.ToDictionary(u => u.Id, u => u.Name);
                responseBody = JsonConvert.SerializeObject(units, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream GetAlarmLevels()
        {
            string responseBody;
            using (var context = new PlansDc(_esterConnectionString))
            {
                var units = context.PropertyAlarmLevels.ToDictionary(u => u.Id, u => u.Name);
                responseBody = JsonConvert.SerializeObject(units, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private List<BaseObject> _importedObjects;
        private void FindGeometry(IEnumerable<XElement> elements, IContainerObject parent)
        {
            foreach (var xElement in elements)
            {
                var planElement = TryParseGeometry(xElement);
                if (planElement == null) continue;
                if (parent != null)
                    parent.Children.Add(planElement);
                else
                    _importedObjects.Add(planElement);
                FindGeometry(xElement.Elements(), planElement as IContainerObject);
            }
        }

        private BaseObject TryParseGeometry(XElement xElement)
        {
            BaseObject res = null;
            switch (xElement.Name.LocalName)
            {
                case "g":
                    res = new Container { Name = "group", IsContainer = true };
                    break;
                case "path":
                    res = new Geometry { Name = "path", Path = new PathData(xElement) };
                    break;
                case "rect":
                    res = new Geometry { Name = "rect", Path = new RectangleData(xElement) };
                    break;
                case "circle":
                    res = new Geometry { Name = "circle", Path = new CircleData(xElement) };
                    break;
                case "ellipse":
                    res = new Geometry { Name = "ellipse", Path = new EllipseData(xElement) };
                    break;
                case "line":
                    res = new Geometry { Name = "line", Path = new LineData(xElement) };
                    break;
                case "polyline":
                    break;
                case "polygon":
                    res = new Geometry { Name = "polygon", Path = new PolygonData(xElement) };
                    break;
            }
            if (res != null && xElement.Attribute("id") != null)
                res.Name = xElement.Attribute("id").Value;
            return res;
        }
    }
}