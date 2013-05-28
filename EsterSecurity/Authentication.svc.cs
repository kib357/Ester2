using System.ServiceModel;
using System.ServiceModel.Activation;

namespace EsterSecurity
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Authentication" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Authentication.svc or Authentication.svc.cs at the Solution Explorer and start debugging.
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class Authentication : IAuthentication
	{
		public string Login(string username, string password)
		{
			return SecurityLogic.AuthenticateUser(username, password);
		}

		public string WindowsLogin()
		{
			return SecurityLogic.AuthenticateUser(ServiceSecurityContext.Current.WindowsIdentity);
		}

		public string DoWork()
		{
			return "I'm working!!";
		}
	}
}
