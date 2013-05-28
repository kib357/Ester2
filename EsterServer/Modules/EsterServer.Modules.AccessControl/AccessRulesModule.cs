using System;
using System.Data.Entity;
using System.IO;
using System.ServiceModel.Activation;
using EsterCommon.ACL;
using EsterCommon.Exceptions;
using EsterServer.Model.Data;
using EsterServer.Model.Services;
using Microsoft.Practices.Prism.Events;
using Newtonsoft.Json;

namespace EsterServer.Modules.AccessControl
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AccessRulesModule : IAccessRulesModule
    {
        private readonly IEventAggregator _eventAggregator;

        public AccessRulesModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public AccessRulesModule()
        {
        }

        public Stream GetAccessRule(string objectId, string subjectId, string actionId)
        {
            string responseBody;
            Guid guidObjectId;
            int intSubjectId, intActionId;
            if ((Guid.TryParse(objectId, out guidObjectId)) && (int.TryParse(subjectId, out intSubjectId)) &&
                (int.TryParse(actionId, out intActionId)))
                using (var context = new ServerContext())
                {
                    responseBody = JsonConvert.SerializeObject(context.AclItems.Find(guidObjectId, intSubjectId, intActionId),
                                                               Formatting.Indented);
                }
            else
                throw new BadRequestException("Check key parameters: AclObjectID, AclSubjectID, ActionID");
            return SerializeHelper.GetResponceFromString(responseBody);
        }

        public Stream AddAccessRule(Stream stream)
        {
            return AddOrChangeAccessRule(stream, true);
        }

        public Stream ChangeAccessRule(string objectId, string subjectId, string actionId, Stream stream)
        {
            return AddOrChangeAccessRule(stream, false);
        }

        public void DeleteAccessRule(string objectId, string subjectId, string actionId)
        {
            Guid guidObjectId;
            int intSubjectId, intActionId;
            if ((Guid.TryParse(objectId, out guidObjectId)) && (int.TryParse(subjectId, out intSubjectId)) &&
                (int.TryParse(actionId, out intActionId)))
                using (var context = new ServerContext())
                {
                    var cardReaderGroup = context.AclItems.Find(guidObjectId, intSubjectId, intActionId);
                    context.Entry(cardReaderGroup).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            else
                throw new BadRequestException("Check key parameters: AclObjectID, AclSubjectID, ActionID");
        }

        private static Stream AddOrChangeAccessRule(Stream stream, bool add)
        {
            var changedAccessRule = SerializeHelper.GetObjectFromStream<AclItem>(stream);
            int res;
            if ((changedAccessRule.AclObjectID == new Guid()) || (changedAccessRule.AclSubjectID == 0) ||
                (changedAccessRule.ActionID == 0))
            {
                throw new BadRequestException("Check key parameters: AclObjectID, AclSubjectID, ActionID");
            }

            using (var context = new ServerContext())
            {
                AclItem originalAccessRule;
                if (add)
                {
                    originalAccessRule = context.AclItems.Attach(new AclItem());
                    context.Entry(originalAccessRule).State = EntityState.Added;
                }
                else
                {
                    originalAccessRule = context.AclItems.Find(changedAccessRule.AclObjectID, changedAccessRule.AclSubjectID, changedAccessRule.ActionID);
                    if(originalAccessRule == null)
                        throw new BadRequestException("Can't find item with those parameters");
                    context.Entry(originalAccessRule).State = EntityState.Modified;
                }
                context.Entry(originalAccessRule).CurrentValues.SetValues(changedAccessRule);
                res = context.SaveChanges();
            }
            var responseBody = JsonConvert.SerializeObject(res, Formatting.Indented);
            return SerializeHelper.GetResponceFromString(responseBody);
        }
    }
}
