using System.Collections.Generic;
using EsterCommon.ACL;
using EsterCommon.ACL.Subjects;
using EsterCommon.AccessControl;

namespace EsterServer.Model.Interfaces
{
    public enum ServerModuleStates
    {
        Stopped,
        Starting,
        Started,
        Paused,
        Fault
    }

    public interface IDataProvider
    {
        string Name { get; }
        ServerModuleStates State { get; set; }

        void Start();
        void Stop();
        void Restart();
        void Pause();
    }

    public interface IPrimitiveDataProvider
    {
        
    }    

    public interface IAccessControlProvider
    {
        void UpdateControllersFromDb();
        void UpdateDbFromControllers();
        void ApplyAcl(List<AclItem> aclItems);
        void ApplyCardReaders(List<CardReader> cardReaders);
        void ApplyPersonsData(List<Person> persons);
        void ApplyPersonGroupsData(List<PersonGroup> personGroups);

        event OnCardReaderChangedEventHandler OnCardReaderChangedEvent;
        event OnAclAppliedEventHandler OnAclAppliedEvent;
        event OnCardReaderAppliedEventHandler OnCardReaderAppliedEvent;
        event OnPersonsDataAppliedEventHandler OnPersonsDataAppliedEvent;
        event OnPersonGroupsDataAppliedEventHandler OnPersonGroupsDataAppliedEvent;
    }

    public delegate void OnCardReaderChangedEventHandler(CardReader reader);    
    public delegate void OnAclAppliedEventHandler(List<AclItem> item);
    public delegate void OnCardReaderAppliedEventHandler(List<CardReader> reader);
    public delegate void OnPersonsDataAppliedEventHandler(List<AclItem> item);
    public delegate void OnPersonGroupsDataAppliedEventHandler(List<AclItem> item);

    public interface ISchedulesProvider
    {
        
    }
}
