using Abp.Events.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sino.EventBus
{
    /// <summary>
    /// 事件总线扩展
    /// </summary>
    public interface IEventBusExtensions : IEventBus
    {
		void Start();
    }
}
