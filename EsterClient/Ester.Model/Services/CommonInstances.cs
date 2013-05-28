using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ester.Model.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace Ester.Model.Services
{
    public static class CommonInstances
    {
        public static IUnityContainer UnityContainer { get; set; }
    }
}
