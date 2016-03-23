using System;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Sino.EventBus.Configuration;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Castle.Core.Logging;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件发送具体实现
	/// </summary>
	public class EventPublish : IEventPublish
	{
		protected IEventBus EventBus { get; private set; }
		protected TopicClient TopicClient { get; private set; }

		public bool UseLocalWhenException { get; set; }

		public ILogger Log { get; set; }

		public bool IsTopicCanUse
		{
			get
			{
				if (TopicClient == null || TopicClient.IsClosed)
				{
					return false;
				}
				return true;
			}
		}

		public EventPublish(IEventBus eventBus)
		{
			UseLocalWhenException = true;
			EventBus = eventBus;
			Log = NullLogger.Instance;
		}

		/// <summary>
		/// 序列化事件数据
		/// </summary>
		protected virtual string SerializeObject(object eventData, Type eventType)
		{
			if (eventData == null)
				throw new ArgumentNullException("eventData");

			return eventType == null ? JsonConvert.SerializeObject(eventData) : JsonConvert.SerializeObject(eventData, eventType, new JsonSerializerSettings());
		}

		protected virtual void SendMessage(Type eventType, IEventData eventData)
		{
			BrokeredMessage message = null;
			//避免序列化过多数据
			eventData.EventSource = null;

			try
			{
				string messageBody = SerializeObject(eventData, eventType);
				message = new BrokeredMessage(messageBody);
				message.Properties[Config.TypeNameProperty] = GetMessageBodyTypeName(eventType);

				Log.DebugFormat("开发发送消息ID为{0}的事件", message.MessageId);
				if (IsTopicCanUse)
				{
					TopicClient.Send(message);
					Log.DebugFormat("消息ID为{0}的事件发送完毕", message.MessageId);
					return;
				}

				Log.Warn("事件无法发送，与服务器的连接已关闭");
				if (UseLocalWhenException)
				{
					EventBus.Trigger(eventType, eventData);
				}
			}
			catch (Exception ex)
			{
				Log.Error("发送事件失败", ex);
				EventBus.Trigger(eventType, eventData);
			}
		}

		/// <summary>
		/// 获取消息类型名称，用于恢复类型
		/// </summary>
		protected virtual string GetMessageBodyTypeName(Type type)
		{
			return type.AssemblyQualifiedName;
		}

		public void Connect(string connectionName)
		{
			Log.Debug("事件总线开始连接");

			var config = new ServiceBusConfiguration(connectionName);
			TopicClient = config.CreateTopicClient(Config.TopicName);

			Log.Debug("事件总线连接完毕");
		}

		public void Close()
		{
			if (IsTopicCanUse)
			{
				TopicClient.Close();
				Log.Debug("事件总线已断开连接");
			}
		}

		public void Trigger(Type eventType, object eventSource, IEventData eventData)
		{
			SendMessage(eventType, eventData);
		}

		public Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
		{
			return Task.Factory.StartNew(() =>
			{
				Trigger(eventType, eventSource, eventData);
			});
		}

		public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
		{
			return TriggerAsync(typeof(TEventData), eventSource, eventData);
		}
	}
}
