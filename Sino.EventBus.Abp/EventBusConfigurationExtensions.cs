using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Events.Bus;
using Castle.MicroKernel.Registration;
using System;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件总线配置扩展
	/// </summary>
	public static class EventBusConfigurationExtensions
	{
		/// <summary>
		/// 服务总线连接字符串Name
		/// </summary>
		public static string ConnectionName { get; set; } = "Azure.ServiceBus";

		/// <summary>
		/// 启用第三方事件总线
		/// </summary>
		/// <param name="connectionName">连接字符串名称</param>
		public static void UseEventBus(this IEventBusConfiguration eventBusConfiguration, string connectionName)
		{
			if (eventBusConfiguration == null)
				throw new ArgumentNullException("eventBusConfiguration");
			if (string.IsNullOrEmpty(connectionName))
				throw new ArgumentNullException("connectionName");

			ConnectionName = connectionName;

			var eventBusExtensions = IocManager.Instance.Resolve<IEventBusExtensions>();

			//释放ABP自带的EventBus
			var releaseEventBus = IocManager.Instance.Resolve<IEventBus>();

			IocManager.Instance.IocContainer.Register(
				Component.For<IEventBus>().
				Instance(eventBusExtensions).
				IsDefault().
				Named("Sino.EventBus")
			);
			eventBusExtensions.Start();
		}
	}
}
