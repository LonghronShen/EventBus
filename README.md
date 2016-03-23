# 简介
基于ABP框架，使ABP中的EventBus能够通过Azure ServiceBus跨越不同的网站。

### 使用方法如下所示：
首先需要安装依赖的类库：
```
Install-Package Sino.EventBus
```
    

然后在Web.Config中增加以下配置：
```
<add key="Azure.ServiceBus" value="[Azure ServiceBus连接字符串]" />
```
    

完成上面的工作之后我们需要在Web项目中的App_Start下的`XXXXWebModule`文件中增加以下语句启用：
```
using Sino.EventBus

public override void Initialize()
{
  //忽略
  Configuration.EventBus.UseEventBus("Azure.ServiceBus");
}
```

完成以上操作后其他的使用方式同ABP的[EventBus]("教程" "http://www.aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events")
