using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EsterCommon.Enums;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
    public class Person : NotificationObject, ICloneable
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; RaisePropertyChanged("FirstName");}
        }

        private string _middleName;
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; RaisePropertyChanged("MiddleName");  }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set 
            {
                _lastName = value;
                RaisePropertyChanged("LastName"); 
            }
        }

        private bool? _isGuest;
        public bool? IsGuest
        {
            get { return _isGuest; }
            set
            {
                _isGuest = value;
                RaisePropertyChanged("IsGuest");
            }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { 
                _phone = value;
                RaisePropertyChanged("Phone");
            }
        }

        private string _department;
        public string Department
        {
            get { return _department; }
            set
            {
                _department = value;
                RaisePropertyChanged("Department");
            }
        }

        private string _position;
        public string Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged("Position");
            }
        }


        private ObservableCollection<AddressValue<CardStates>>  _accessCard;
        public ObservableCollection<AddressValue<CardStates>> AccessCard
        {
            get { return _accessCard; }
            set 
            { 
               _accessCard = value; 
                RaisePropertyChanged("AccessCard"); 
            }
        }

        private List<string> _doorList;
        public List<string> DoorList 
        {
            get { return _doorList; }
            set 
            { 
                _doorList = value;
                RaisePropertyChanged("DoorList");
            }
        }

        private Document _document;
        public Document Document
        {
            get { return _document; }
            set
            {
                _document = value;
                RaisePropertyChanged("Document");
            }
        }

        private Roles? _role;
        public Roles? Role
        {
            get { return _role; }
            set
            {
                _role = value;
                RaisePropertyChanged("Role");
            }
        }

        private bool _isInit;
        public bool IsInit
        {
            get { return _isInit; }
            set
            {
                _isInit = value;
                RaisePropertyChanged("IsInit");
            }
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private bool _isDisabled;
        public bool IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                _isDisabled = value;
                RaisePropertyChanged("IsDisabled");
            }
        }

        private DateTime? _birthdayDate;
        public DateTime? BirthdayDate
        {
            get { return _birthdayDate; }
            set { _birthdayDate = value;RaisePropertyChanged("BirthdayDate"); }
        }

        private byte[] _photo;
        public byte[] Photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                RaisePropertyChanged("Photo");
            }
        }

        private string _activeDirectorySID;
        public string ActiveDirectorySID
        {
            get { return _activeDirectorySID; }
            set
            {
                _activeDirectorySID = value; 
                RaisePropertyChanged("ActiveDirectorySID");
            }
        }


        public override string ToString()
        {
            string tmp = (IsGuest==true) ? "Новый гость" : "Новый сотрудник";
            return LastName + FirstName == "" ? tmp : LastName + " " + FirstName;
        }

        public string ToLongPersonString()
        {
            if (IsGuest==true)
            {
                string result = FirstName + " " + MiddleName + " " + LastName + " " + Phone + " " +
                                Department + " " + Position + " " + AccessCard + " ";
                if (Document != null) result += Document.Series + "" + Document.Number + " " + Document.Issued + " " + DocTypeToString();
                else result += "отсутствует документ";
                return result;
            }
            else
            {
                return FirstName + " " + MiddleName + " " + LastName + " " + Phone + " " +
                   Department + " " + Position + " " + AccessCard;
            }
        }

        public Person(PersonTypes type=PersonTypes.Employee, bool isInit=true)
        {
            InitializeFields();
            Id = -1;
            IsGuest = (type == PersonTypes.Guest);
            IsInit = isInit;
            LastName = IsGuest==true ? "Новый гость" : "Новый сотрудник"; 
            Document = new Document() { Type = DocumentTypes.Unknown };
        }

        public Person() : this(PersonTypes.Employee)
        {
           
        }

        private void InitializeFields()
        {
            Id = 0;
            FirstName = "";
            MiddleName = "";
            LastName = "";
            Phone = "";
            Department = "";
            Position = "";
            AccessCard = new ObservableCollection<AddressValue<CardStates>>();
            DoorList = new List<string>();
            Role = Roles.None;
            Document = null;
        }

        string DocTypeToString()
        {
            if (Document == null) return "документ отсутствует";
            switch (Document.Type)
            {
                case DocumentTypes.ForeignPassport:
                    return "загранпаcпорт";
                case DocumentTypes.DriverLicence:
                    return "водительское удостоверение";
                case DocumentTypes.RussianPassport:
                    return "паспорт российский рф";
                case DocumentTypes.ForeignRusPassport:
                    return "загранпаспорт рф";
                default:
                    return "неизвестный документ";
            }
        }


        public object Clone()
        {
            var clone = (Person)this.MemberwiseClone();
            if (Photo != null)
            {
                clone.Photo = new byte[Photo.Length];
                Photo.CopyTo(clone.Photo, 0);
            }
            if (DoorList!=null)
                clone.DoorList = new List<string>(DoorList);
            if (Document!=null)
                clone.Document = (Document) Document.Clone();
            if (AccessCard!=null)
                clone.AccessCard = new ObservableCollection<AddressValue<CardStates>>(AccessCard);

            return clone;
        }
    }
}
