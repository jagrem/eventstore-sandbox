using System;

namespace EventStore.SandBox
{
	public class EventBody : IEventBody
	{
		public EventBody (Type eventType, string eventData)
		{
			this.EventType = eventType;
			this.EventData = eventData;
		}
	
		public Type EventType { get; private set; }

		public string EventData { get; private set; }
	}
}

