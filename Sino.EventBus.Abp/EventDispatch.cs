using Abp.Events.Bus;
using Castle.Core.Logging;
using Newtonsoft.Json;
using System;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件分发具体实现
	/// </summary>
	public class EventDispatch : IEventDispatch
	{
		protected IEventBus EventBus { get; set; }

		public ILogger Log { get; set; }

		public EventDispatch(IEventBus eventBus)
		{
			EventBus = eventBus;
			Log = NullLogger.Instance;
		}

		/// <summary>
		/// 反序列化得到事件对象
		/// </summary>
		/// <param name="eventType">事件类型</param>
		/// <param name="eventData">事件序列化数据</param>
		protected virtual IEventData DeserializeObject(Type eventType, string eventData)
		{
			if (string.IsNullOrEmpty(eventData))
				throw new ArgumentNullException("eventData");

			var eventObject = eventType == null ? JsonConvert.DeserializeObject(eventData) : JsonConvert.DeserializeObject(eventData, eventType, new JsonSerializerSettings());
			if (eventObject is IEventData)
			{
				return eventObject as IEventData;
			}
			Log.Error("反序列化事件失败");
			return null;
		}

		protected virtual Type GetTypeByTypeName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			Type eventType = null;

			try
			{
				eventType = Type.GetType(typeName);
			}
			catch (Exception ex)
			{
				Log.Error("根据类型名称获取类型失败", ex);
			}
			return eventType;
		}

		protected virtual bool Trigger(string typeName, string eventData)
		{
			Type eventType = GetTypeByTypeName(typeName);
			IEventData eventDataObj = DeserializeObject(eventType, eventData);
			if (eventDataObj == null)
			{
				return false;
			}

			if (eventType == null)
			{
				EventBus.Trigger(eventDataObj);
			}
			else
			{
				EventBus.Trigger(eventType, eventDataObj);
			}
			return true;
		}

		public bool Dispatch(string typeName, string messageBody)
		{
			return Trigger(typeName, messageBody);
		}
	}
}
