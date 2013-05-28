using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ester.Model.Extensions
{
    public static class ObservableCollectionExtension
    {
        //public static void Sort<T>(this ObservableCollection<T> collection, Fun) where T : IComparable
        //{
        //    // Order by and put into a list.
        //    List<T> sorted = collection.OrderBy(x => x).ToList();
 
        //    // Loop the list and exchange items in the collection.
        //    for (int i = sorted.Count() - 1; i >= 0; i--)
        //    {
        //        collection.Insert(0, sorted[i]);
        //        collection.RemoveAt(collection.Count - 1);
        //    }
        //}

        public static void Sort<TSource,TKey>(this ObservableCollection<TSource> collection, Func<TSource,TKey> keySelector)
        {
            int index = -1;
            try
            {
                var sorted = collection.OrderBy(keySelector).ToList();

                for (int i = 0; i < sorted.Count(); i++)
                {
                    index = collection.IndexOf(sorted[i]);
                    if (index>=0) collection.Move(index, i);   
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }  
 
   
    }
}
