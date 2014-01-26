using System;
using FluentAssertions;
using NUnit.Framework;

namespace EventStore.SandBox.UnitTests
{
	[TestFixture]
	public class EventTests
	{
		[Test]
		public void TestConstructor ()
		{
			var eventId = "id";
			var expectedData = "data";
			var expectedType = typeof(string);

			var subject = new Event (new AnyEventHeader { EventId = eventId }, new AnyEventBody {
				EventType = expectedType,
				EventData = expectedData
			});

			subject.Header.Should ().NotBeNull ();
			subject.Header.EventId.Should ().Be (eventId);
			subject.Body.Should ().NotBeNull ();
			subject.Body.EventType.Should ().Be<string> ();
			subject.Body.EventData.Should ().Be (expectedData);
		}
	}
}
