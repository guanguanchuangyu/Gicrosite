using System;
using System.Collections.Generic;
using System.Text;

namespace Gicrosite.EventBuses
{
    public abstract class EventDataBase : IEventData
    {
        /// <summary>
        /// 获取 事件编号
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime EventTime { get; set; }
        /// <summary>
        /// 事件源
        /// </summary>
        public object DataSource { get; set; }
        /// <summary>
        /// 初始化一个<see cref="EventDataBase"/>类型的新实例
        /// </summary>
        public EventDataBase()
        {
            Id = Guid.NewGuid();
            EventTime = DateTime.Now;
        }
    }
}
