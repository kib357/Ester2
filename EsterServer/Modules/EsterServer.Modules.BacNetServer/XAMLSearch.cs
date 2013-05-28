using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EsterServer.Modules.BacNetServer
{
    /// <summary>
    /// Вспомогательные методы модуля просмотра секций(разделов) программы диспетчерезации "EnigneMon"
    /// © ООО СК "Астория", 2011
    /// </summary>
    public static class XAMLSearch
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   Поиск всех датчиков на канвасе
        /// </summary>
        /// <param name = "sensorList">Ссылка на массив датчиков в который будут внесены изменения</param>
        /// <param name = "canvas">Канвас для поиска</param>
        /*public static void GetAdressesInCanvas(List<string> sensorList, Canvas canvas)
        {
            foreach (SensorBase sensor in FindLogicalChildren<SensorBase>(canvas).Where(s => !String.IsNullOrEmpty(s.Address)))
            {
                sensorList.AddRange(sensor.AddressList.Where(n => n.Length > 0));

                //if (sensor.AddressList.Length == 0)
                    //Logger.Warn("Не верно указаны значения адресов для датчика " + sensor.Address);
            }
        }*/

        #region Поиск элементов в дереве XAML

        /// <summary>
        /// Поиск всех визуальных потомков элемента по типу <T>
        /// </summary>
        /// <typeparam name="T">Тип элементов для поиска</typeparam>
        /// <param name="depObj">Родительский элемент, от которого следует начинать поиск</param>
        /// <returns>Перечисление всех найденных объектов указанного типа</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    LogicalTreeHelper.GetChildren(depObj);
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Поиск всех визуальных родителей элемента по типу <T>
        /// </summary>
        /// <typeparam name="T">Тип элементов для поиска</typeparam>
        /// <param name="depObj">Элемент, от которого следует начинать поиск</param>
        /// <returns>Перечисление всех найденных объектов указанного типа</returns>
        public static IEnumerable<T> FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);

                if (parent != null && parent is T)
                {
                    yield return (T)parent;
                }

                foreach (T parentOfParent in FindVisualParent<T>(parent))
                {
                    yield return parentOfParent;
                }
            }
        }


        /// <summary>
        /// Поиск всех логических потомков элемента по типу <T>
        /// </summary>
        /// <typeparam name="T">Тип элементов для поиска</typeparam>
        /// <param name="depObj">Родительский элемент, от которого следует начинать поиск</param>
        /// <returns>Перечисление всех найденных объектов указанного типа</returns>
        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    if (child is DependencyObject)
                        foreach (T childOfChild in FindLogicalChildren<T>((DependencyObject)child))
                        {
                            yield return childOfChild;
                        }
                }
            }
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!String.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        #endregion
    }
}
