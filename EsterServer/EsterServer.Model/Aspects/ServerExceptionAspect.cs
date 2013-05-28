//using System;
//using System.Net;
//using System.ServiceModel.Web;
//using EsterCommon.Exceptions;
//using EsterServer.Model.Interfaces;
//using NLog;
//using PostSharp.Aspects;

//namespace EsterServer.Model.Aspects
//{
//    [Serializable]
//    public class ServerExceptionAspectAttribute: OnMethodBoundaryAspect
//    {
//        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

//        public ServerExceptionAspectAttribute()
//        {
//            AspectPriority = 0;
//        }

//        public override void OnEntry(MethodExecutionArgs args)
//        {
//            var sender = args.Instance as IDataProvider;
//            if (WebOperationContext.Current == null || sender == null || sender.State == ServerModuleStates.Started)
//                return;
//            args.FlowBehavior = FlowBehavior.Return;
//            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
//            if (sender.State == ServerModuleStates.Fault)
//                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Fault.";
//            else
//                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Not initialized";
//        }

//        public override void OnException(MethodExecutionArgs args)
//        {
//            NLogger.Debug(args.Exception.Message);
//            if (WebOperationContext.Current == null) return;

//            if (args.Exception is BadRequestException) 
//                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
//            else
//                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
//            WebOperationContext.Current.OutgoingResponse.StatusDescription = args.Exception.Message; 
//        }
//    }
//}
