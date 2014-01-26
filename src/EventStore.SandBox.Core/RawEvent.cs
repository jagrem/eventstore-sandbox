using EventStore.ClientAPI;

namespace EventStore.SandBox
{
	public class RawEvent : IRawEvent
	{
		public RawEvent(string eventType, byte[] metadata, byte[] data)
		{
			this.EventType = eventType;
			this.Metadata = metadata;
			this.Data = data;
		}

		public RawEvent(RecordedEvent recordedEvent)
		{
			this.EventType = recordedEvent.EventType;
			this.Metadata = recordedEvent.Metadata;
			this.Data = recordedEvent.Data;
		}

		public string EventType { get; private set; }

		public byte[] Metadata { get; private set; }

		public byte[] Data { get; private set; }
	}
}
