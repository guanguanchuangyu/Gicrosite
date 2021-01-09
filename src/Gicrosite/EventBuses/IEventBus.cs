using System;
using System.Collections.Generic;
using System.Text;

namespace Gicrosite.EventBuses
{
    /// <summary>
    /// 定义事件总线接口
    /// </summary>
    public interface IEventBus:IEventPublisher,IEventSubscriber
    {

    }
}
