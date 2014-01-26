namespace EventStore.SandBox
{
	public class EventHeader : IEventHeader
	{
		public EventHeader(string eventId)
		{
			this.EventId = eventId;
		}

		public string EventId { get; private set; }
	}
}
