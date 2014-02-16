namespace EventStore.SandBox
{
	public class EventType : IEventType
	{
		public EventType(string name)
		{
			this.Name = name;
		}

		public static EventType Any { get { return new Any(); } }

		public string Name { get; private set; }

		public static implicit operator EventType(string name)
		{
			return new EventType (name);
		}
	}
}

