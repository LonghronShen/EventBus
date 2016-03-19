using Abp.Dependency;

namespace Sino.EventBus
{
	/// <summary>
	/// 事件分发处理接口
	/// </summary>
	public interface IEventDispatch : ISingletonDependency
    {
		/// <summary>
		/// 分发处理事件
		/// </summary>
		/// <param name="typeName">类型全称</param>
		/// <param name="messageBody">消息主体</param>
		bool Dispatch(string typeName, string messageBody);
    }
}
