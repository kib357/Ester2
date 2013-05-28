using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace EsterServer
{
	public class APIKeyAuthorization : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			string key = GetAPIKey(operationContext);

			try
			{
				if (APIKeyRepository.IsValidAPIKey(key) || IsLoginRequest(operationContext) || IsUpdateRequest(operationContext))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				CreateErrorReply(operationContext, HttpStatusCode.InternalServerError, "Error while checking API key",
								ex.Message);
				return false;
			}
			

			// Send back an HTML reply
		    var context = HttpContext.Current;
		    if (context != null)
		        context.Items["SuppressAuthenticationKey"] = true;
			return false;
		}

		private bool IsUpdateRequest(OperationContext operationContext)
		{
			// Get the request message
			var request = operationContext.RequestContext.RequestMessage;

			var path = request.Properties.Via.AbsolutePath;

			if (path.ToLower().Contains("/updates"))
				return true;
			return false;
		}

		public bool IsLoginRequest(OperationContext operationContext)
		{
			// Get the request message
			var request = operationContext.RequestContext.RequestMessage;

			var path = request.Properties.Via.AbsolutePath;

			if (path.ToLower().Contains("/login"))
				return true;
			return false;
		}

		public string GetAPIKey(OperationContext operationContext)
		{
			// Get the request message
			var request = operationContext.RequestContext.RequestMessage;

			// Get the HTTP Request
			var requestProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

			// Get the query string
			NameValueCollection queryParams = HttpUtility.ParseQueryString(requestProp.QueryString);

			// Return the API key (if present, null if not)
			return queryParams[APIKEY];
		}

		private static void CreateErrorReply(OperationContext operationContext, HttpStatusCode scode, string smessage, string message)
		{
			// The error message is padded so that IE shows the response by default
			using (var sr = new StringReader("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + APIErrorHTML0 + scode.ToString() + APIErrorHTML1 + message + APIErrorHTML2))
			{
				XElement response = XElement.Load(sr);
				using (Message reply = Message.CreateMessage(MessageVersion.None, null, response))
				{
					var responseProp = new HttpResponseMessageProperty() { StatusCode = scode, StatusDescription = smessage };
					responseProp.Headers[HttpResponseHeader.ContentType] = "text/html";
					reply.Properties[HttpResponseMessageProperty.Name] = responseProp;
					operationContext.RequestContext.Reply(reply);
					// set the request context to null to terminate processing of this request
					operationContext.RequestContext = null;                    
				}
			}
		}

		const string APIKEY = "APIKey";
		const string APIErrorHTML0 = @"
<html>
<head>
    <title>Request Error</title>
    <style type=""text/css"">
        body
        {
            font-family: Verdana;
            font-size: large;
        }
    </style>
</head>
<body>
    <h1>
	";
	const string APIErrorHTML1 = @"
    </h1>
    <p>
";
		const string APIErrorHTML2 = @"
    </p>
</body>
</html>
";
	}
}