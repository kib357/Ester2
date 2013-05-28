using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Ionic.Zip;
using Nini.Config;

namespace EsterServer.Modules.Update
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UpdateModule : IUpdateModule
    {
        private readonly XmlConfigSource _configSource = new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };
        private XmlConfigSource _clientConfigSource;
        readonly string _dir = Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources");

        public string CheckForUpdates(string clientVersion)
        {
            var response = string.Empty;

            try
            {
                if (!Directory.Exists(_dir + @"\Ester"))
                {
                    throw new Exception("Cannot find Ester folder on server");
                }

                _clientConfigSource = new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\Ester\Config.xml")) { AutoSave = true };
                int clientVersionNumber;
                if (int.TryParse(clientVersion, out clientVersionNumber))
                {
                    if (clientVersionNumber < GetLastVersionNumber())
                    {
                        response = _configSource.Configs["Update"].Get("UpdateFileUrl");
                    }
                }
            }
            catch (Exception exception)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = exception.Message;
            }

            return response;
        }

        private int GetLastVersionNumber()
        {
            string newHash = GetFolderHash(_dir + @"\Ester");
            string oldHash = _configSource.Configs["Update"].Get("FolderHash");

            if (newHash != oldHash)
            {
                int currentVersionOnServer = _configSource.Configs["Update"].GetInt("VersionOnServer");
                currentVersionOnServer++;
                _configSource.Configs["Update"].Set("VersionOnServer", currentVersionOnServer);

                _clientConfigSource.Configs["Update"].Set("NeedUpdate", "False");
                _clientConfigSource.Configs["Update"].Set("Version", currentVersionOnServer);

                _configSource.Configs["Update"].Set("FolderHash", GetFolderHash(_dir + @"\Ester"));

                if (File.Exists(_dir + @"\Updates.zip")) File.Delete(_dir + @"\Updates.zip");
                CreateUpdateFile();
            }
            return _configSource.Configs["Update"].GetInt("VersionOnServer");
        }

        private static string GetFolderHash(string dirName)
        {
            var res = new byte[0];
            foreach (var file in Directory.GetFiles(dirName, "*.*", SearchOption.AllDirectories).OrderBy(s => s))
            {
                if (res.Length == 0)
                    res = GetMD5Hash(file);
                else
                {
                    var tmp = GetMD5Hash(file);
                    for (int i = 0; i < res.Length; i++)
                    {
                        res[i] = (byte)(res[i] ^ tmp[i]);
                    }
                }
            }
            return BitConverter.ToString(res);
        }

        private static byte[] GetMD5Hash(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileName = Encoding.UTF8.GetBytes(Path.GetFileName(path));
                byte[] fileData = new byte[fs.Length + fileName.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                fileName.CopyTo(fileData, fs.Length);
                byte[] result = md5.ComputeHash(fileData);
                return result;
            }
        }

        private void CreateUpdateFile()
        {
            var zipFile = new ZipFile(_dir + @"\Updates.zip") { UseZip64WhenSaving = Zip64Option.Always };
            zipFile.AddDirectory(_dir + @"\Ester");
            zipFile.Save();
            zipFile.Dispose();
        }
    }
}
