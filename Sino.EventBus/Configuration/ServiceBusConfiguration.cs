using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;

namespace Sino.EventBus.Configuration
{
	public class ServiceBusConfiguration
	{
		/// <summary>
		/// 事件默认保留的时间
		/// </summary>
		public TimeSpan DefaultMessageTimeToLive { get; set; } = new TimeSpan(0, 5, 0);

		/// <summary>
		/// 事件总线能够容纳的数据最大容量
		/// </summary>
		public long MaxSizeInMegabytes { get; set; } = 5120;

		/// <summary>
		/// 连接字符串参数名
		/// </summary>
		public string ConnectionName { get; private set; }

		/// <summary>
		/// 连接字符串
		/// </summary>
		public string ConnectionString { get; private set; }

		protected NamespaceManager NamespaceManager { get; set; }

		public ServiceBusConfiguration(string connectionName)
		{
			ConnectionName = connectionName;
		}

		/// <summary>
		/// 获取命名空间管理对象
		/// </summary>
		protected NamespaceManager GetNamespaceManager()
		{
			if (NamespaceManager == null)
			{
				ConnectionString = CloudConfigurationManager.GetSetting(ConnectionName);
				NamespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);
			}
			return NamespaceManager;
		}

		/// <summary>
		/// 创建主题命名空间
		/// </summary>
		/// <param name="topicName">主题名称</param>
		public void CreateTopic(string topicName)
		{
			if (string.IsNullOrEmpty(topicName))
				throw new ArgumentNullException("topicName");

			GetNamespaceManager();

			TopicDescription td = new TopicDescription(topicName);
			td.MaxSizeInMegabytes = MaxSizeInMegabytes;
			td.DefaultMessageTimeToLive = DefaultMessageTimeToLive;

			if (!NamespaceManager.TopicExists(topicName))
			{
				NamespaceManager.CreateTopic(td);
			}
		}

		/// <summary>
		/// 创建订阅命名空间
		/// </summary>
		/// <param name="topicName">主题名称</param>
		/// <param name="subscriptionName">订阅名称</param>
		public void CreateSubscription(string topicName, string subscriptionName)
		{
			if (string.IsNullOrEmpty(topicName))
				throw new ArgumentNullException("topicName");
			if (string.IsNullOrEmpty(subscriptionName))
				throw new ArgumentNullException("subscriptionName");

			GetNamespaceManager();

			if (!NamespaceManager.SubscriptionExists(topicName, subscriptionName))
			{
				SubscriptionDescription sd = new SubscriptionDescription(topicName, subscriptionName);
				NamespaceManager.CreateSubscription(sd);
			}
		}

		/// <summary>
		/// 获取主题API对象
		/// </summary>
		/// <param name="topicName">主题名称</param>
		/// <remarks>
		/// 如果该主题不存在则会自动创建该主题
		/// </remarks>
		public TopicClient CreateTopicClient(string topicName)
		{
			if (string.IsNullOrEmpty(topicName))
				throw new ArgumentNullException("topicName");

			CreateTopic(topicName);

			return TopicClient.CreateFromConnectionString(ConnectionString, topicName);
		}

		/// <summary>
		/// 获取主题下订阅的API对象
		/// </summary>
		/// <param name="topicName">主题名称</param>
		/// <param name="subscriptionName">订阅名称</param>
		/// <remarks>
		/// 如果主题或订阅名称不存在则自动创建
		/// </remarks>
		public SubscriptionClient CreateSubscriptionClient(string topicName, string subscriptionName)
		{
			if (string.IsNullOrEmpty(topicName))
				throw new ArgumentNullException("topicName");
			if (string.IsNullOrEmpty(subscriptionName))
				throw new ArgumentNullException("subscriptionName");

			CreateTopic(topicName);
			CreateSubscription(topicName, subscriptionName);

			return SubscriptionClient.CreateFromConnectionString(ConnectionString, topicName, subscriptionName);
		}
	}
}
