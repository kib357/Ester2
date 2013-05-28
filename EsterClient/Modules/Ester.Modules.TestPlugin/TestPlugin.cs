using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EsterCommon.PlanObjectTypes.Abstract;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Ester.Modules.TestPlugin
{
	[Module(ModuleName = "TestPlugin")]
	[ModuleDependency("BuildingModule")]
	public class TestPlugin : IModule
	{
		private readonly IUnityContainer _container;

		public TestPlugin(IUnityContainer container)
		{
			_container = container;

			//var r = container.Resolve<IPlanObjectTypeResolver>
			//r.Add(50, typeof(MyTestPluginClass))
			var resource = new ResourceDictionary { Source = new Uri(@"pack://application:,,,/Ester.Modules.TestPlugin;component/Resources.xaml") };
		}

		public void Initialize()
		{

		}
	}


	public class MyTestPluginClass : Subsystem
	{
		
	}
}
