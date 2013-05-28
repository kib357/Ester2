using System;
﻿using System.Collections.ObjectModel;
﻿using System.Threading.Tasks;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using EsterCommon.BaseClasses;
﻿using Microsoft.Practices.Prism;

namespace Ester.Model.Repositories
{
    public class PeopleRepository : Repository
    {
        public override string Title
        {
            get { return "список сотрудников и гостей"; }
        }

        private IDataTransport _dataTransport;

        public PeopleRepository(IDataTransport dataTransport)
        {
            _dataTransport = dataTransport;

            People = new ObservableCollection<Person>();           
        }

        public override void LoadData()
        {
			OnDataReceivedEvent();
           // Task.Run(() => GetPeople());
        }

        public async override Task UpdateData()
        {
			OnDataReceivedEvent();
           // await Task.Run(() => GetPeople());
        }

        private void GetPeople()
        {
            ObservableCollection<Person> peoples;
            ObservableCollection<Person> guests;

            peoples = _dataTransport.GetRequest<ObservableCollection<Person>>(Urls.People, true, 30000);
            guests = _dataTransport.GetRequest<ObservableCollection<Person>>(Urls.Guests, true, 30000);

            if (peoples == null || guests == null)
                return;
            var allPeople = peoples.AddRange(guests);
            People = StaticPeopleList = new ObservableCollection<Person>(allPeople);

            OnDataReceivedEvent();
        }

        public ObservableCollection<Person> People { get; private set; }
        public static ObservableCollection<Person> StaticPeopleList { get; private set; }
    }
}