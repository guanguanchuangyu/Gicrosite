using System;
using System.Collections.Generic;
using System.Text;

namespace Gicrosite.EventBuses
{
    public interface IEventData
    {
        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime EventTime { get; set; }
        /// <summary>
        /// 事件对象
        /// </summary>
        public object EventSource { get; set; }
    }
}
