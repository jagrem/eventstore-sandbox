namespace EventStore.SandBox
{
	public interface IEvent
	{
		IEventHeader Header { get; set; }

		IEventBody Body { get; set; }
	}
}
