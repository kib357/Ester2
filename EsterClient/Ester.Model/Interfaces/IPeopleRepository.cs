using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ester.Model.BaseClasses;
using EsterCommon.BaseClasses;

namespace Ester.Model.Interfaces
{
    
    public interface IPeopleRepository
    {        
        ObservableCollection<Person> People { get; set; }
        Task<ObservableCollection<Person>> GetPeople();        
    }
}