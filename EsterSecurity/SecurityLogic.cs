using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;
using EsterCommon.Security;

namespace EsterSecurity
{
	public class SecurityLogic
	{
		private static string GetPasswordHash(string password, string pwdSalt)
		{
#pragma warning disable 612,618
			var saltHash = FormsAuthentication.HashPasswordForStoringInConfigFile(pwdSalt, "MD5");
			var passHash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");

			return FormsAuthentication.HashPasswordForStoringInConfigFile(saltHash+passHash, "MD5");
#pragma warning restore 612,618
		}

		private static string GenerateNewApikey()
		{
			return Guid.NewGuid().ToString();
		}

		private static string GenerateNewApikey(SecurityContext context, User user)
		{
			var newApikey = Guid.NewGuid().ToString();
			using (context)
			{
				user.Apikey = newApikey;
				user.LastActivity = DateTime.Now;
				context.SaveChanges();
			}
			return newApikey;
		}

		public static string AuthenticateUser(string username, string password)
		{
			return GenerateNewApikey();
			using (var context = new SecurityContext())
			{
				var currUser = context.Users.FirstOrDefault(u => u.Username == username);
				if (currUser != null)
				{
					var pwdHash = GetPasswordHash(password, currUser.PwdSalt);
					if (pwdHash == currUser.PwdHash)
					{
						return GenerateNewApikey(context, currUser);
					}
				}
			}
			return null;
		}
		public static string AuthenticateUser(int? sid)
		{
			return GenerateNewApikey();
			using (var context = new SecurityContext())
			{
				var currUser = context.Users.FirstOrDefault(u => u.Sid == sid);
				if (currUser != null)
				{
					return GenerateNewApikey(context, currUser);
				}
			}
			return null;
		}
		public static string AuthenticateUser(WindowsIdentity windowsIdentity)
		{
			return GenerateNewApikey();
			using (var context = new PrincipalContext(ContextType.Domain, "ester.local"))
			{
				using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
				{
					foreach (var result in searcher.FindAll())
					{
						var de = result.GetUnderlyingObject() as DirectoryEntry;
						Console.WriteLine("First Name: " + de.Properties["givenName"].Value);
						Console.WriteLine("Last Name : " + de.Properties["sn"].Value);
						Console.WriteLine("SAM account name   : " + de.Properties["samAccountName"].Value);
						Console.WriteLine("User principal name: " + de.Properties["userPrincipalName"].Value);
						Console.WriteLine();
					}
				}
			}
		}
	}
}
