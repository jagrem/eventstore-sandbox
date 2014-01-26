namespace EventStore.SandBox
{
	public class Event : IEvent
	{
		public Event(IEventHeader eventHeader, IEventBody eventBody)
		{
			this.Header = eventHeader;
			this.Body = eventBody;
		}

		public IEventHeader Header { get; set; }

		public IEventBody Body { get; set; }
	}
}
