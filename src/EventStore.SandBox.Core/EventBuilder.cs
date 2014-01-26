using System;
using Newtonsoft.Json;

namespace EventStore.SandBox
{
	public class EventBuilder
	{
		protected EventBuilder ()
		{
		}

		public static EventBuilder Create()
		{
			var builder = new EventBuilder ();
			return builder;
		}

		protected IEventHeader Header { get; set; }

		protected IEventBody Body { get; set; }

		public Event Build(object @event)
		{
			this.Header = new EventHeader (Guid.NewGuid ().ToString ());
			this.Body = new EventBody (@event.GetType(), JsonConvert.SerializeObject (@event));
			return new Event (this.Header, this.Body);
		}
	}
}

