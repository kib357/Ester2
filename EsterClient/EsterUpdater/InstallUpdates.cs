using System.IO;

namespace EsterUpdater
{
    class InstallUpdates
    {
        readonly string _dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private readonly string _backupDir =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Backup";
        private readonly string _updateDir =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Update";

        public void Install()
        {
            if (Directory.Exists(_updateDir) && Directory.GetFiles(_updateDir).Length > 0)
            {
                CreateBackup();
                CopyDir(_updateDir, _dir);
                Directory.Delete(_updateDir, true);
            }
        }

        private void CreateBackup()
        {
            if (Directory.Exists(_backupDir))
                Directory.Delete(_backupDir, true);
            MoveOldToBackup(_dir, _backupDir);
        }

        private void MoveOldToBackup(string fromDir, string toDir)
        {
            Directory.CreateDirectory(toDir);
            foreach (string s1 in Directory.GetFiles(fromDir))
            {
                if (s1 != System.Reflection.Assembly.GetExecutingAssembly().Location && !s1.Contains("vshost"))
                {
                    string s2 = toDir + @"\" + Path.GetFileName(s1);
                    File.Move(s1, s2);
                }
            }
            foreach (string s in Directory.GetDirectories(fromDir))
            {
                if (s.ToLower() != _backupDir.ToLower() && s.ToLower() != _updateDir.ToLower() && s.ToLower() != _dir.ToLower() + @"\logs")
                    Directory.Move(s, toDir + "\\" + Path.GetFileName(s));
            }
        }

        private void CopyDir(string fromDir, string toDir, bool overWrite = true)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(fromDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(fromDir, toDir));

            //Copy all the files
            foreach (string newPath in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories))
                if (newPath != System.Reflection.Assembly.GetExecutingAssembly().Location && !newPath.Contains("vshost"))
                    File.Copy(newPath, newPath.Replace(fromDir, toDir), true);

            /*Directory.CreateDirectory(toDir);
            foreach (string s1 in Directory.GetFiles(fromDir))
            {
                if (s1 != System.Reflection.Assembly.GetExecutingAssembly().Location && s1.ToLower() != _updateDir.ToLower() + @"\engineupdater.exe")
                {
                    string s2 = toDir + @"\" + Path.GetFileName(s1);
                    File.Copy(s1, s2, overWrite);
                }
            }
            foreach (string s in Directory.GetDirectories(fromDir))
            {
                if (s != _backupDir)
                    CopyDir(s, toDir + @"\" + Path.GetFileName(s));
            }*/
        }
    }
}
