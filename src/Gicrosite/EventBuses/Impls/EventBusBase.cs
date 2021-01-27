using Gicrosite.Data;
using Gicrosite.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gicrosite.EventBuses
{
    /// <summary>
    /// 事件总线基类
    /// </summary>
    public abstract class EventBusBase : IEventBus
    {
        /// <summary>
        /// 定义线程安全集合
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<IEventHandler>> _handlers;

        public EventBusBase()
        {
            _handlers = new ConcurrentDictionary<Type, List<IEventHandler>>();
        }

        /// <summary>
        /// 通过反射，将事件源与事件处理绑定
        /// </summary>
        private void Register(Type[] eventHandlerTypes)
        {
            foreach (var type in eventHandlerTypes)
            {
                if (typeof(IEventHandler).IsAssignableFrom(type))//判断当前类型是否实现了IEventHandler接口
                {
                    Type handlerInterface = type.GetInterface("IEventHandler`1");//获取该类实现的泛型接口
                    if (handlerInterface != null)
                    {
                        Type eventDataType = handlerInterface.GetGenericArguments()[0];//获取泛型的第一个参数类型
                        if (_handlers.ContainsKey(eventDataType))
                        {
                            List<IEventHandler> events = GetOrCreateHandleres(eventDataType);
                            events.Add((IEventHandler)Activator.CreateInstance(type));//构建实例
                            _handlers[eventDataType] = events;
                        }
                        else
                        {
                            List<IEventHandler> events = GetOrCreateHandleres(eventDataType);
                            events.Add((IEventHandler)Activator.CreateInstance(type));
                            _handlers[eventDataType] = events;
                        }
                    }
                }
            }
        }

        #region Implementation of IEventPublisher
        public virtual void Publish<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            Publish<TEventData>(null, eventData, wait);
        }

        public virtual void Publish<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            Publish(typeof(TEventData), eventSource, eventData, wait);
        }

        public virtual void Publish(Type eventType, IEventData eventData, bool wait = true)
        {
            Publish(eventType, null, eventData, wait);
        }

        public virtual void Publish(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            eventData.EventSource = eventData.EventSource ?? eventSource;

            IDictionary<Type,IEventHandler[]> dict = GetHandlers(eventType);
            if (dict.Count == 0)
            {
                return;
            }
            foreach (KeyValuePair<Type, IEventHandler[]> typeItem in dict)
            {
                foreach (IEventHandler eventHandler in typeItem.Value)
                {
                    InvokeHandler(eventHandler, eventType, eventData, wait);
                }
            }
        }

        public virtual Task PublishAsync<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            return PublishAsync<TEventData>(null, eventData, wait);
        }

        public Task PublishAsync<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            return PublishAsync(typeof(TEventData), eventSource, eventData, wait);
        }

        public virtual Task PublishAsync(Type eventType, IEventData eventData, bool wait = true)
        {
            return PublishAsync(eventType, null, eventData, wait);
        }

        public virtual async Task PublishAsync(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            eventData.EventSource = eventSource;

            IDictionary<Type, IEventHandler[]> dict = GetHandlers(eventType);
            if (dict.Count == 0)
            {
                return;
            }
            foreach (var typeItem in dict)
            {
                foreach (IEventHandler eventHandler in typeItem.Value)
                {
                    await InvokeHandlerAsync(eventHandler, eventType, eventData, wait);
                }
            }
        }

        /// <summary>
        /// 重写以实现触发事件的执行
        /// </summary>
        /// <param name="eventHandler">事件处理器工厂</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="wait">是否等待结果返回</param>
        protected void InvokeHandler(IEventHandler eventHandler, Type eventType, IEventData eventData, bool wait = true)
        {
            IEventHandler handler = eventHandler;
            if (handler == null)
            {
                return;
            }
            if (wait)
            {
                Run(handler, eventType, eventData);
            }
            else
            {
                Task.Run(() =>
                {
                    Run(handler, eventType, eventData);
                });
            }
        }

        /// <summary>
        /// 重写以实现异步触发事件的执行
        /// </summary>
        /// <param name="factory">事件处理器工厂</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="wait">是否等待结果返回</param>
        /// <returns></returns>
        protected virtual Task InvokeHandlerAsync(IEventHandler eventHandler, Type eventType, IEventData eventData, bool wait = true)
        {
            IEventHandler handler = eventHandler;
            if (handler == null)
            {
                return Task.FromResult(0);
            }
            if (wait)
            {
                return RunAsync(eventHandler, handler, eventType, eventData);
            }
            Task.Run(async () =>
            {
                await RunAsync(eventHandler, handler, eventType, eventData);
            });
            return Task.FromResult(0);
        }

        private void Run(IEventHandler handler, Type eventType, IEventData eventData)
        {
            try
            {
                handler.Handle(eventData);
            }
            catch (Exception ex)
            {
                string msg = $"执行事件“{eventType.Name}”的处理器“{handler.GetType()}”时引发异常：{ex.Message}";
                throw ;
            }
        }

        private Task RunAsync(IEventHandler eventHandler, IEventHandler handler, Type eventType, IEventData eventData)
        {
            try
            {
                //ICancellationTokenProvider cancellationTokenProvider = _serviceProvider.GetService<ICancellationTokenProvider>();
                return handler.HandleAsync(eventData);
            }
            catch (Exception ex)
            {
                string msg = $"执行事件“{eventType.Name}”的处理器“{handler.GetType()}”时引发异常：{ex.Message}";
                throw;
            }
        }
        #endregion

        #region Implementation of IEventSubscriber
        public void Subscribe<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new()
        {
            Subscribe<TEventData>((IEventHandler)Activator.CreateInstance(typeof(TEventHandler)));
        }

        public void Subscribe<TEventData>(IEventHandler eventHandler) where TEventData : IEventData
        {
            Check.NotNull(eventHandler, nameof(eventHandler));

            Subscribe(typeof(TEventData), eventHandler);
        }

        public void Subscribe(Type eventType, IEventHandler eventHandler)
        {
            Check.NotNull(eventType, nameof(eventType));
            Check.NotNull(eventHandler, nameof(eventHandler));

            List<IEventHandler> events = GetOrCreateHandleres(eventType);
            if (!events.Any(x => x.GetType() == eventHandler.GetType()))
            {
                events.Add(eventHandler);
                _handlers[eventType] = events;
            }
        }

        public void SubscribeAll(Type[] eventHandlerTypes)
        {
            Check.NotNull(eventHandlerTypes, nameof(eventHandlerTypes));

            Register(eventHandlerTypes);
        }


        public void Unsubscribe<TEventData>(IEventHandler<TEventData> eventHandler) where TEventData : IEventData
        {
            Check.NotNull(eventHandler, nameof(eventHandler));

            Unsubscribe(typeof(TEventData), eventHandler);
        }

        public void Unsubscribe(Type eventType, IEventHandler eventHandler)
        {
            Check.NotNull(eventType, nameof(eventType));
            Check.NotNull(eventHandler, nameof(eventHandler));

            _handlers[eventType].Remove(eventHandler);
        }

        public void UnsubscribeAll<TEventData>() where TEventData : IEventData
        {
            UnsubscribeAll(typeof(TEventData));
        }

        public void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandleres(eventType).Locking(eventHanders =>
            {
                eventHanders.Clear();
            });
        }
        #endregion

        private List<IEventHandler> GetOrCreateHandleres(Type eventType)
        {
            return _handlers.GetOrAdd(eventType, type => new List<IEventHandler>());
        }

        /// <summary>
        /// 获取指定事件源的所有处理器
        /// </summary>
        /// <returns></returns>
        public IDictionary<Type, IEventHandler[]> GetHandlers(Type eventType)
        {
            return _handlers.Where(item => item.Key == eventType || item.Key.IsAssignableFrom(eventType))
                .ToDictionary(item => item.Key, item => item.Value.ToArray());
        }
    }
}
