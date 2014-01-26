namespace EventStore.SandBox
{
	public interface IRawEvent
	{
		string EventType { get; }

		byte[] Metadata { get; }

		byte[] Data { get; }
	}
}
