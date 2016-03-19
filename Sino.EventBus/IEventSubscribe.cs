using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sino.EventBus
{
    /// <summary>
    /// 事件订阅接口
    /// </summary>
    public interface IEventSubscribe : ISingletonDependency
    {
		/// <summary>
		/// 订阅是否可用
		/// </summary>
		bool IsSubscribeCanUse { get; }

		/// <summary>
		/// 开始连接并工作
		/// </summary>
		void Connect(string connectionName);
		
		/// <summary>
		/// 停止监听
		/// </summary>
		void Stop();

		/// <summary>
		/// 开始监听
		/// </summary>
		/// <remarks>
		/// 在调用Connect后会自动执行此方法，允许重复运行。
		/// </remarks>
		void Start();
    }
}
