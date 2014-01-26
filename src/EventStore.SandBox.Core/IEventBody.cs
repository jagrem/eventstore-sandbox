using System;

namespace EventStore.SandBox
{
	public interface IEventBody
	{
		Type EventType { get; }

		string EventData { get; }
	}
}
