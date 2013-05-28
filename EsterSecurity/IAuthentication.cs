using System.ServiceModel;
using System.ServiceModel.Web;

namespace EsterSecurity
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMixedAuthService" in both code and config file together.
	[ServiceContract]
	public interface IAuthentication
	{
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "work")]
		string DoWork();

		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "login?user={username}&pwd={password}")]
		string Login(string username, string password);

		[OperationContract]
		string WindowsLogin();
	}
}
