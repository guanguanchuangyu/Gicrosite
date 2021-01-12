using System;
using System.Collections.Generic;
using System.Text;

namespace Gicrosite.EventBuses
{
    public class EventBus: EventBusBase
    {
        private static EventBus eventBus;
        public static EventBus Instance
        {
            get {
                if (eventBus == null)
                {
                    eventBus = new EventBus();
                }
                return eventBus;
            }
        }
    }
}
