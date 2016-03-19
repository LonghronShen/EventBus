using Abp.Dependency;
using Abp.Events.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件发送接口
	/// </summary>
	public interface IEventPublish : ISingletonDependency
	{
		/// <summary>
		/// 当出现异常后是否调用本地事件处理，默认为true
		/// </summary>
		bool UseLocalWhenException { get; set; }

		/// <summary>
		/// 开始连接并工作
		/// </summary>
		void Connect(string connectionName);

		/// <summary>
		/// 关闭连接
		/// </summary>
		void Close();

		void Trigger(Type eventType, object eventSource, IEventData eventData);


		Task TriggerAsync(Type eventType, object eventSource, IEventData eventData);


		Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;
	}
}
