using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace EsterServer.Modules.BacNetServer
{
    /// <summary>
    ///   Класс загрузки файлов содержащих ResourceDictionary модуля просмотра секций(разделов) программы диспетчерезации "EnigneMon"
    ///   © ООО СК "Астория", 2011
    /// </summary>
    public static class ResourceDictionaries
    {
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ResourceDictionary GetDictionary(string relativePath = @"Resources\Dictionaries")
        {
            var mergedDictionary = new ResourceDictionary();
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, relativePath);

            if (Directory.Exists(path))
                foreach (string fileName in Directory.GetFiles(path).Where(f => f.EndsWith(".xaml")))
                    using (var fs = new FileStream(fileName, FileMode.Open))
                    {
                        try
                        {
                            var xr = new XamlReader();
                            
                            var tmp = (ResourceDictionary)XamlReader.Load(fs);
                            foreach (string key in tmp.Keys)
                                if (tmp[key] is Canvas)
                                {
                                    mergedDictionary.Add(key, tmp[key]);
                                }

                        }
                        catch (Exception)
                        {
                            //_logger.Error(ex.Message);
                        }
                    }
            else
                Directory.CreateDirectory(path);

            return mergedDictionary;
        }
    }
}
