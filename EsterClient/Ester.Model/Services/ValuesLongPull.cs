﻿using System;
using System.Collections.Generic;
﻿using System.Threading;
﻿using System.Threading.Tasks;
﻿using Ester.Model.Events;
using Ester.Model.Interfaces;
﻿using EsterCommon.PlanObjectTypes;
﻿using EsterCommon.PlanObjectTypes.Abstract;
﻿using EsterCommon.Services;
﻿using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;


namespace Ester.Model.Services
{
	//лонг-пулл сервис изменения показания контроллеров



	public delegate void ValuesChangedHandler(UpdateInfo updateInfo);

	public static class ValuesLongPull
	{
		private static readonly IDataTransport DataTransport;
		private static readonly IEventAggregator EventAggregator;

		private const string PullValuesChangesQuery = "/Values/subscribe";

		public static event ValuesChangedHandler ValuesChangedEvent;
		private static void OnValuesChangedEvent(UpdateInfo updateInfo)
		{
			ValuesChangedHandler handler = ValuesChangedEvent;
			if (handler != null) handler(updateInfo);
		}

		public static bool IsStarted { get; private set; }

		static ValuesLongPull()
		{
			DataTransport = CommonInstances.UnityContainer.Resolve<IDataTransport>();
			EventAggregator = CommonInstances.UnityContainer.Resolve<IEventAggregator>();
		}

		public static void Start()
		{
			if (IsStarted) return;
			IsStarted = true;
			Task.Factory.StartNew(() => GetChanges(), TaskCreationOptions.LongRunning);
		}

		public static void Stop()
		{
			IsStarted = false;
		}

		private static void GetChanges(string query = null)
		{
			try
			{
				var result = DataTransport.GetRequest<UpdateInfo>(query ?? PullValuesChangesQuery, true, 120000, new PlanObjectConverter());
				if (result != null && (result.ChangedValues.Count > 0 || result.UpdateObjectTree))
					OnValuesChangedEvent(result);

				if (IsStarted) GetChanges();
			}
			catch (Exception ex)
			{
				EventAggregator.GetEvent<ShowErrorEvent>().Publish(new Exception("Ошибка при попытке получения значений датчиков.", ex));
				Thread.Sleep(60000);
				if (IsStarted) GetChanges(query);
			}
		}
	}
}
