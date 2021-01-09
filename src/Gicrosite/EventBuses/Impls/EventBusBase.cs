using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gicrosite.EventBuses
{
    /// <summary>
    /// 事件总线基类
    /// </summary>
    public abstract class EventBusBase : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 定义线程安全集合
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<IEventHandler>> _handlers;

        public EventBusBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Implementation of IEventPublisher
        public void Publish<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Publish<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Publish(Type eventType, IEventData eventData, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public void Publish(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(Type eventType, IEventData eventData, bool wait = true)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            throw new NotImplementedException();
        } 
        #endregion

        #region Implementation of IEventSubscriber
        public void Subscribe<TEventData, TEventHandler>()
            where TEventData : IEventData
            where TEventHandler : IEventHandler, new()
        {
            
        }

        public void Subscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEventData>(IEventHandler eventHandler) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type eventType, IEventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAll(Type[] eventHandlerTypes)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEventData>(IEventHandler<TEventData> eventHandler) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type eventType, IEventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll<TEventData>() where TEventData : IEventData
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
