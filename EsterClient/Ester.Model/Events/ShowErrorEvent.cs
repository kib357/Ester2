using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Ester.Model.Events
{
    public class ShowErrorEvent : CompositePresentationEvent<Exception>
    {
    }
}
