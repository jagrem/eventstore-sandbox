using System;
using System.IO;

namespace EventStore.SandBox
{
	public class EventBody : IEventBody
	{
		public EventBody (Type eventType, string eventData)
		{
			this.EventType = eventType;
			this.EventData = eventData;
		}

		public static EventBody Empty() { return new EventBody(null, null); }
	
		public Type EventType { get; private set; }

		public string EventData { get; private set; }
	}
}

