using System.Collections.Generic;
using System.Linq;

namespace EventStore.SandBox
{
	public static class TestEvents
	{
		public static IEnumerable<Event> CreateEvents(int count)
		{
			var builder = EventBuilder.Create();
			return Enumerable.Range (1, count).Select (x => builder.Build (new HelloWorld { Message = "Hi" }));
		}
	}
}

