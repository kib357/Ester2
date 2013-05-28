using System;
using EsterCommon.Enums;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
    public class Document:NotificationObject, ICloneable
    {

        private int? _documentId;
        public int? DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; RaisePropertyChanged("DocumentId"); }
        }

        private DocumentTypes? _type;
        public DocumentTypes? Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged("Type"); }
        }

        private string _series;
        public string Series
        {
            get { return _series; }
            set { _series = value; RaisePropertyChanged("Series"); }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
            set { _number = value; RaisePropertyChanged("Number"); }  
        }

        private string _issued;
        public string Issued
        {
            get { return _issued; }
            set { _issued = value; RaisePropertyChanged("Issued"); }
        }

        private byte[] _photo;
        public byte[] Photo
        {
            get { return _photo; }
            set { _photo = value; RaisePropertyChanged("Photo"); }
        }

        private DateTime? _issueDate;
        public DateTime? IssueDate
        {
            get { return _issueDate; }
            set { _issueDate = value; RaisePropertyChanged("IssueDate"); }
        }

        private DateTime? _expireDate;
        public DateTime? ExpireDate
        {
            get { return _expireDate; }
            set { _expireDate = value; RaisePropertyChanged("ExpireDate"); }
        }

        public object Clone()
        {
            var doc = (Document) this.MemberwiseClone();
            if (Photo != null)
            {
                doc.Photo = new byte[Photo.Length];

                Photo.CopyTo(doc.Photo, 0);
            }
            return doc;
        }
    }
}
