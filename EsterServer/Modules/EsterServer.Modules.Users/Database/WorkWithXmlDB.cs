using System;
using System.Linq;
using System.Xml.Linq;
using EsterCommon.BaseClasses;

namespace EsterServer.Modules.Users.Database
{
    public static class WorkWithXmlDB
    {
        private static XDocument _doc;
        
        private const string FileName = "Users.xml";

        private static string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", FileName);
        public static string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                LoadDocument(value);
            }
        }

        static void LoadDocument(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path)) throw new Exception(String.Format("File \"{0}\" not exist", path));
                using (var sr = new System.IO.StreamReader(path))
                {
                    _doc = XDocument.Load(sr);
                } 
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
                       
        }

        static WorkWithXmlDB()
        {
            LoadDocument(Path);
        }
    
        public static void AddUser(User user)
        {
            try
            {
                var newUser = new XElement("User", new XAttribute("Login", user.Login), new XAttribute("Sid", user.Sid),
                                       new XAttribute("Role", user.Role), new XAttribute("Domain", user.Domain));
                _doc.Descendants("Users").Last().Add(newUser);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }       
        }


    }
}
