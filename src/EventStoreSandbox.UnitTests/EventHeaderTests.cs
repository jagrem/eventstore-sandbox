using System;
using FluentAssertions;
using NUnit.Framework;

namespace EventStore.SandBox.UnitTests
{
	[TestFixture]
	public class EventHeaderTests
	{
		[Test]
		public void TestConstructor()
		{
			var expectedEventId = "eventId";

			var subject = new EventHeader (expectedEventId);

			subject.EventId.Should ().NotBeNullOrEmpty ();
			subject.EventId.Should ().Be (expectedEventId);
		}
	}
}
