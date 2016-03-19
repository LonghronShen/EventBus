using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Abp.Events.Bus;
using Sino.EventBus;
using System.Threading;

namespace EventBusUnitTest
{
	public class TestEvent : EventData
	{
		public string UserId { get; set; }

		public long AdminId { get; set; }
	}

	[TestClass]
	public class GlobalUnitTest
	{
		[TestMethod]
		public void TestEventBus()
		{
			Type eventType = null;
			IEventData eventData = null;
			int i = 0;

			var eventBus = new Mock<IEventBus>();
			eventBus.Setup(x => x.Trigger(It.IsAny<Type>(), It.IsAny<IEventData>())).Callback<Type, IEventData>((x, y) =>
			{
				eventType = x;
				eventData = y;
				++i;
			});

			var eventBusExtension = new EventBusExtensions(eventBus.Object, new EventPublish(eventBus.Object),
				new EventSubscribe(new EventDispatch(eventBus.Object)));
			eventBusExtension.Start();

			var eventBusExtension2 = new EventBusExtensions(eventBus.Object, new EventPublish(eventBus.Object),
				new EventSubscribe(new EventDispatch(eventBus.Object)));
			eventBusExtension2.Start();

			eventBusExtension.Trigger(new TestEvent
			{
				UserId = "test1",
				AdminId = 5546
			});

			Thread.Sleep(2000);

			Assert.AreEqual(i, 2);
		}
	}
}
