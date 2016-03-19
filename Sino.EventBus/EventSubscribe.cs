using System.Threading.Tasks;
using Sino.EventBus.Configuration;
using Microsoft.ServiceBus.Messaging;
using System;
using Castle.Core.Logging;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件订阅实现
	/// </summary>
	public class EventSubscribe : IEventSubscribe
    {
		public bool IsSubscribeCanUse
		{
			get
			{
				if(SubscriptionClient == null || SubscriptionClient.IsClosed)
				{
					return false;
				}
				return true;
			}
		}

		public ILogger Log { get; set; }

		protected IEventDispatch EventDispatch { get; set; }

		protected SubscriptionClient SubscriptionClient { get; set; }

		public EventSubscribe(IEventDispatch eventDispatch)
		{
			EventDispatch = eventDispatch;
			Log = NullLogger.Instance;
		}

		async Task Receive(BrokeredMessage message)
		{
			Log.InfoFormat("收到ID为{0}的事件", message.MessageId);
			if (message.Properties.ContainsKey(Config.TypeNameProperty))
			{
				string typeName = message.Properties[Config.TypeNameProperty].ToString();
				string messageBody = message.GetBody<string>();
				if (EventDispatch.Dispatch(typeName, messageBody))
				{
					await message.CompleteAsync();
					Log.InfoFormat("事件ID为{0}的事件处理完毕", message.MessageId);
					return;
				}
			}
			await message.AbandonAsync();
			Log.InfoFormat("事件ID为{0}的事件处理失败", message.MessageId);
		}

		#region IEventSubscribe Impl

		public void Connect(string connectionName)
		{
			var config = new ServiceBusConfiguration(connectionName);
			SubscriptionClient = config.CreateSubscriptionClient(Config.TopicName, Guid.NewGuid().ToString());

			Start();
		}

		public void Stop()
		{
			if (IsSubscribeCanUse)
			{
				Log.Info("关闭订阅事件");
				SubscriptionClient.Close();
			}
		}

		public void Start()
		{
			if (IsSubscribeCanUse)
			{
				Log.Info("开始订阅事件");
				SubscriptionClient.OnMessageAsync(Receive);
			}
		}

		#endregion
	}
}
