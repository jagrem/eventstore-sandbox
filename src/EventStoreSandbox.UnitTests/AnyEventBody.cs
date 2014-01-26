using System;

namespace EventStore.SandBox.UnitTests
{
	internal class AnyEventBody : IEventBody
	{
		public Type EventType { get; set; }

		public string EventData { get; set; }
	}
}
