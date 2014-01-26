using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Net;

namespace EventStore.SandBox
{	
	public class EventStoreAdapter 
	{
		private readonly IEventStoreConnection connection;

		private readonly EventDataConverter converter;

		public EventStoreAdapter(string ipAddress)
		{
			var connectionSettings = ConnectionSettings.Create ()
				.SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
				.OnAuthenticationFailed ((connection, message) => Console.WriteLine ("Authentication failed. {0}", message))
				.OnConnected ((connection, endPoint) => Console.WriteLine ("Connected. {0}:{1}", endPoint.Address, endPoint.Port))
				.OnClosed ((connection, message) => Console.WriteLine ("Closed. {0}", message))
				.OnDisconnected ((connection, endPoint) => Console.WriteLine ("Disconnected. {0}:{1}", endPoint.Address, endPoint.Port))
				.OnErrorOccurred ((connection, exception) => Console.WriteLine ("Error occured. {0}", exception))
				.OnReconnecting (connection => Console.WriteLine ("Reconnecting."))
				.EnableVerboseLogging();

			this.connection = EventStoreConnection.Create (connectionSettings, new IPEndPoint (IPAddress.Parse(ipAddress), 1113));
			this.converter = new EventDataConverter ();
			this.connection.Connect();
		}

		public void SaveEventsWithId(string id, IEnumerable<Event> events, int expectedVersion)
		{
			this.connection.AppendToStream(id, expectedVersion, events.Select (eventToSave => this.converter.ToEventData (eventToSave)).ToArray());
		}

		public IEnumerable<IEvent> LoadEventsForId(string id, int fromVersion = StreamPosition.Start)
		{
			var streamEvents = new List<ResolvedEvent>();

			StreamEventsSlice currentSlice;
			var nextSliceStart = fromVersion;
			do
			{
				currentSlice = this.connection.ReadStreamEventsForward(id, nextSliceStart, 200, false);
				nextSliceStart = currentSlice.NextEventNumber;

				streamEvents.AddRange(currentSlice.Events);
			} while (!currentSlice.IsEndOfStream);

			return streamEvents.Select(x => this.converter.FromEventData(new RawEvent(x.Event)));
		}

		public IEnumerable<IEvent> LoadLastEventsForId(string id, int count)
		{
			return this.connection.ReadStreamEventsBackward (id, StreamPosition.End, count, true)
				.Events
					.Select (x => this.converter.FromEventData (new RawEvent(x.Event)));
		}

		public async void DeleteEventsWithId(string id, int expectedVersion)
		{
			await this.connection.DeleteStreamAsync (id, expectedVersion);
		}
	}
}
