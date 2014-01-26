using EventStore.ClientAPI;
using System;
using Newtonsoft.Json;
using System.Text;

namespace EventStore.SandBox
{
	public class EventDataConverter
	{
		public EventData ToEventData(IEvent eventToConvert)
		{
			if (eventToConvert == null) {
				throw new ArgumentNullException ("eventToConvert");
			}

			if (eventToConvert.Header == null) {
				throw new ArgumentException ("Event header is required");
			}

			if (eventToConvert.Body == null) {
				throw new ArgumentException ("Event body is required");
			}
		
			var metadata = Encoding.UTF8.GetBytes (JsonConvert.SerializeObject (eventToConvert.Header));
			var data = eventToConvert.Body.EventData != null ? Encoding.UTF8.GetBytes(eventToConvert.Body.EventData) : new byte[0];
			var eventData = new EventData (Guid.NewGuid(), eventToConvert.Body.EventType != null ? eventToConvert.Body.EventType.FullName : null, true, data, metadata);
			return eventData;
		}

		public IEvent FromEventData(IRawEvent rawEvent)
		{
			if (rawEvent == null) {
				throw new ArgumentNullException ("rawEvent");
			}

			if (rawEvent.Metadata == null || rawEvent.Metadata.Length < 1) {
				throw new ArgumentException ("Event metadata is required");
			}

			var eventHeader = JsonConvert.DeserializeObject<EventHeader>(Encoding.UTF8.GetString (rawEvent.Metadata));
			var eventType = Type.GetType (rawEvent.EventType);
			var eventBody = eventType != null  && rawEvent.Data != null ? new EventBody (eventType, Encoding.UTF8.GetString (rawEvent.Data)) : EventBody.Empty();
			return new Event(eventHeader, eventBody);
		}
	}
}
