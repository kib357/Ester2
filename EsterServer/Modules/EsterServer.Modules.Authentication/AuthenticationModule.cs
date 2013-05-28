using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Xml.Linq;
using EsterCommon.Exceptions;
using EsterServer.Model.Aspects;


namespace EsterServer.Modules.Authentication
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	[ServerExceptionAspect]
	public class AuthenticationModule : IAuthenticationModule
	{
		[Log("Попытка авторизации", true)]
		public string AuthenticateUser(string login, string password)
		{
			var response = string.Empty;

			var path = Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources");
			if (!Directory.Exists(path))
			{
				throw new Exception("У сервера нет доступа к базе данных");
			}
			XDocument doc;
			using (var sr = new StreamReader(path + @"\Users.xml"))
			{
				doc = XDocument.Load(sr);

			}
			if (doc.Root == null) throw new Exception("У сервера нет доступа к базе данных");
			var user = doc.Root.Descendants().FirstOrDefault(s =>
			{
				var xAttribute = s.Attribute("Login");
				return xAttribute != null && xAttribute.Value == HttpUtility.HtmlDecode(login);
			});

			if (user != null && user.Attribute("Salt") != null)
			{
				var passwordHash = user.Attribute("Password");
				var salt = user.Attribute("Salt");
				if (passwordHash != null && salt != null)
				{
					var hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(HttpUtility.HtmlDecode(password)), Encoding.UTF8.GetBytes(salt.Value));
					if (CompareByteArrays(hash, Convert.FromBase64String(passwordHash.Value)))
					{
						response = "bda11d91-7ade-4da1-855d-24adfe39d174";
					}
				}
			}
			else
			{
				throw new BadRequestException("Неверный логин или пароль");
			}

			return response;
		}

		private byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
		{
			HashAlgorithm algorithm = new SHA256Managed();

			var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

			for (int i = 0; i < plainText.Length; i++)
			{
				plainTextWithSaltBytes[i] = plainText[i];
			}

			for (int i = 0; i < salt.Length; i++)
			{
				plainTextWithSaltBytes[plainText.Length + i] = salt[i];
			}

			return algorithm.ComputeHash(plainTextWithSaltBytes);
		}

		private bool CompareByteArrays(byte[] array1, byte[] array2)
		{
			if (array1.Length != array2.Length)
				return false;

			for (int i = 0; i < array1.Length; i++)
				if (array1[i] != array2[i])
					return false;

			return true;
		}

		private string CreateSalt(int size)
		{
			var rng = new RNGCryptoServiceProvider();
			var buff = new byte[size];
			rng.GetBytes(buff);

			return Convert.ToBase64String(buff);
		}
	}
}
