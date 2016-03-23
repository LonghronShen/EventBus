using System;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Sino.EventBus
{
	/// <summary>
	/// 对Abp EventBus的封装与扩展
	/// </summary>
	public class EventBusExtensions : IEventBusExtensions, ISingletonDependency
	{
		/// <summary>
		/// Abp自带EventBus，用来降级时使用
		/// </summary>
		protected IEventBus EventBus { get; set; }

		/// <summary>
		/// 事件发布
		/// </summary>
		protected IEventPublish EventPublish { get; set; }

		/// <summary>
		/// 事件订阅
		/// </summary>
		protected IEventSubscribe EventSubscribe { get; set; }

		/// <summary>
		/// 日志
		/// </summary>
		protected ILogger Log { get; set; }

		public EventBusExtensions(IEventBus eventBus, IEventPublish eventPublish, IEventSubscribe eventSubscribe)
		{
			if (eventBus == null)
				throw new ArgumentNullException("eventBus");
			if (eventPublish == null)
				throw new ArgumentNullException("eventPublish");
			if (eventSubscribe == null)
				throw new ArgumentNullException("EventSubscribe");

			Log = NullLogger.Instance;
			EventBus = eventBus;
			EventPublish = eventPublish;
			EventSubscribe = eventSubscribe;
		}

		public virtual void Start()
		{
			if (string.IsNullOrEmpty(EventBusConfigurationExtensions.ConnectionName))
			{
				throw new ArgumentNullException("ConnectionName");
			}

			EventPublish.Connect(EventBusConfigurationExtensions.ConnectionName);
			EventSubscribe.Connect(EventBusConfigurationExtensions.ConnectionName);
		}

		#region Trigger

		public void Trigger(Type eventType, IEventData eventData)
		{
			Trigger(eventType, null, eventData);
		}

		public void Trigger(Type eventType, object eventSource, IEventData eventData)
		{
			EventPublish.Trigger(eventType, eventSource, eventData);
		}

		public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
		{
			Trigger((object)null, eventData);
		}

		public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
		{
			Trigger(typeof(TEventData), eventSource, eventData);
		}

		public Task TriggerAsync(Type eventType, IEventData eventData)
		{
			return TriggerAsync(eventType, null, eventData);
		}

		public Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
		{
			return EventPublish.TriggerAsync(eventType, eventSource, eventData);
		}

		public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
		{
			return TriggerAsync((object)null, eventData);
		}

		public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
		{
			return EventPublish.TriggerAsync(eventSource, eventData);
		}

		#endregion

		#region UnAndRegister

		public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
		{
			return EventBus.Register(eventType, handlerFactory);
		}

		public IDisposable Register(Type eventType, IEventHandler handler)
		{
			return EventBus.Register(eventType, handler);
		}

		public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
		{
			return EventBus.Register<TEventData>(handlerFactory);
		}

		public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
		{
			return EventBus.Register(handler);
		}

		public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
		{
			return EventBus.Register(action);
		}

		public IDisposable Register<TEventData, THandler>()
			where TEventData : IEventData
			where THandler : IEventHandler<TEventData>, new()
		{
			return EventBus.Register<TEventData, THandler>();
		}

		public void Unregister(Type eventType, IEventHandlerFactory factory)
		{
			EventBus.Unregister(eventType, factory);
		}

		public void Unregister(Type eventType, IEventHandler handler)
		{
			EventBus.Unregister(eventType, handler);
		}

		public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
		{
			EventBus.Unregister<TEventData>(factory);
		}

		public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
		{
			EventBus.Unregister(handler);
		}

		public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
		{
			EventBus.Unregister(action);
		}

		public void UnregisterAll(Type eventType)
		{
			EventBus.UnregisterAll(eventType);
		}

		public void UnregisterAll<TEventData>() where TEventData : IEventData
		{
			EventBus.UnregisterAll<TEventData>();
		}

		#endregion
	}
}
