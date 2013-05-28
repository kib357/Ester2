using System;
using System.Data.Entity;
using System.IO;
using System.ServiceModel.Activation;
using EsterCommon.IntruderAlarm;
using EsterServer.Model.Data;
using EsterServer.Model.Services;
using Microsoft.Practices.Prism.Events;
using Newtonsoft.Json;

namespace EsterServer.Modules.AccessControl
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    class IntruderAlarmModule : IIntruderAlarmModule
    {
        private readonly IEventAggregator _eventAggregator;

        public IntruderAlarmModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public Stream GetIntruderAlarmAreas()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.IntruderAlarmAreas, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddIntruderAlarmArea(Stream stream)
        {
            return AddOrChangeIntruderAlarmArea(stream);
        }

        public Stream ChangeIntruderAlarmArea(string id, Stream stream)
        {
            return AddOrChangeIntruderAlarmArea(stream);
        }

        public void DeleteIntruderAlarmArea(string id)
        {
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
                using (var context = new ServerContext())
                {
                    var intruderAlarmArea = context.IntruderAlarmAreas.Find(guidId);
                    context.Entry(intruderAlarmArea).State = EntityState.Deleted;
                    context.SaveChanges();
                }
        }

        public Stream GetIntruderAlarmAreaGroups()
        {
            string responseBody;
            using (var context = new ServerContext())
            {
                responseBody = JsonConvert.SerializeObject(context.IntruderAlarmAreaGroups, Formatting.Indented);
            }
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddIntruderAlarmAreaGroup(Stream stream)
        {
            return AddOrChangeIntruderAlarmAreaGroup(stream);
        }

        public Stream ChangeIntruderAlarmAreaGroup(string id, Stream stream)
        {
            return AddOrChangeIntruderAlarmAreaGroup(stream);
        }

        public void DeleteIntruderAlarmAreaGroup(string id)
        {
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
                using (var context = new ServerContext())
                {
                    var intruderAlarmAreaGroup = context.IntruderAlarmAreaGroups.Find(guidId);
                    context.Entry(intruderAlarmAreaGroup).State = EntityState.Deleted;
                    context.SaveChanges();
                }
        }

        private static Stream AddOrChangeIntruderAlarmArea(Stream stream)
        {
            var chagedIntruderAlarmArea = SerializeHelper.GetObjectFromStream<IntruderAlarmArea>(stream);
            int res;
            using (var context = new ServerContext())
            {
                IntruderAlarmArea originalIntruderAlarmArea;
                if (chagedIntruderAlarmArea.Id == new Guid())
                {
                    originalIntruderAlarmArea = context.IntruderAlarmAreas.Attach(new IntruderAlarmArea());
                    context.Entry(originalIntruderAlarmArea).State = EntityState.Added;
                }
                else
                {
                    originalIntruderAlarmArea = context.IntruderAlarmAreas.Find(chagedIntruderAlarmArea.Id);
                    context.Entry(originalIntruderAlarmArea).State = EntityState.Modified;
                }
                context.Entry(originalIntruderAlarmArea).CurrentValues.SetValues(chagedIntruderAlarmArea);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        private static Stream AddOrChangeIntruderAlarmAreaGroup(Stream stream)
        {
            var chagedIntruderAlarmAreaGroup = SerializeHelper.GetObjectFromStream<IntruderAlarmAreaGroup>(stream);
            int res;
            using (var context = new ServerContext())
            {
                IntruderAlarmAreaGroup originalIntruderAlarmAreaGroup;
                if (chagedIntruderAlarmAreaGroup.Id == new Guid())
                {
                    originalIntruderAlarmAreaGroup = context.IntruderAlarmAreaGroups.Attach(new IntruderAlarmAreaGroup());
                    context.Entry(originalIntruderAlarmAreaGroup).State = EntityState.Added;
                }
                else
                {
                    originalIntruderAlarmAreaGroup = context.IntruderAlarmAreaGroups.Find(chagedIntruderAlarmAreaGroup.Id);
                    context.Entry(originalIntruderAlarmAreaGroup).State = EntityState.Modified;
                }
                context.Entry(originalIntruderAlarmAreaGroup).CurrentValues.SetValues(chagedIntruderAlarmAreaGroup);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }
    }
}
