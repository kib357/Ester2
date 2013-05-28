//using System;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Web;
//using EsterCommon.Data;
//using NLog;
//using PostSharp.Aspects;

//namespace EsterServer.Model.Aspects
//{
//    [Serializable]
//    public sealed class LogAttribute : OnMethodBoundaryAspect
//    {
//        private readonly string _message;
//        private readonly bool _writeAllParams;
//        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

//        public LogAttribute(string message, bool writeAllParams = false)
//        {
//            AspectPriority = 1;
//            _message = message;
//            _writeAllParams = writeAllParams;
//        }

//        public string Message { get { return _message; } }

//        public override void OnExit(MethodExecutionArgs args)
//        {
//            string userName = string.Empty,
//                   deviceAddress = string.Empty,
//                   objectAddress = string.Empty,
//                   value = string.Empty,
//                   details = string.Empty;

//            Guid apiKey;
//            Guid.TryParse(HttpContext.Current.Request.Params["apikey"], out apiKey);
//            using (var classes = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
//            {
//                var user = classes.Users.FirstOrDefault(u => u.ApiKey == apiKey);
//                if (user != null) userName = user.Login;
//            }

//            var parameters = args.Method.GetParameters();
//            foreach (var parameterInfo in parameters)
//            {
//                if (parameterInfo.Name.Equals("deviceAddress", StringComparison.OrdinalIgnoreCase))
//                    deviceAddress = args.Arguments[parameterInfo.Position].ToString();
//                if (parameterInfo.Name.Equals("objectAddress", StringComparison.OrdinalIgnoreCase))
//                    objectAddress = args.Arguments[parameterInfo.Position].ToString();
//                if (parameterInfo.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
//                    value = args.Arguments[parameterInfo.Position].ToString();

//                if (_writeAllParams && !parameterInfo.Name.Equals("password", StringComparison.OrdinalIgnoreCase))
//                {
//                    if (details != string.Empty) details += ";";
//                    var argObject = args.Arguments[parameterInfo.Position];
//                    string argValue;
//                    if (argObject is Stream)
//                    {
//                        using (var reader = new StreamReader(argObject as Stream))
//                            argValue = reader.ReadToEnd();
//                    }
//                    else
//                        argValue = argObject.ToString();
//                    details += parameterInfo.Name + ":" + argValue;
//                }
//            }

//            var myEvent = new LogEventInfo(LogLevel.Info, NLogger.Name, Message);
//            myEvent.Properties.Add("userName", userName);
//            if (deviceAddress != string.Empty && objectAddress != string.Empty)
//                myEvent.Properties.Add("address", deviceAddress + "." + objectAddress);
//            myEvent.Properties.Add("value", value);
//            myEvent.Properties.Add("details", details);
//            NLogger.Log(myEvent);
//        }
//    }
//}
