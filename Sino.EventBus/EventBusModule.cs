using Abp;
using Abp.Modules;
using System.Reflection;

namespace Sino.EventBus
{
    /// <summary>
    /// 模块配置
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EventBusModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
