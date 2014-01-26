using NUnit.Framework;
using FluentAssertions;
using System;

namespace EventStore.SandBox.UnitTests
{

	[TestFixture]
	public class EventBodyTests
	{
		[Test]
		public void TestConstructor()
		{
			var expectedType = typeof(string);
			var expectedData = "data";

			var subject = new EventBody (expectedType, expectedData);

			subject.EventType.Should ().NotBeNull ();
			subject.EventType.Should ().Be<string> ();
			subject.EventData.Should ().NotBeNull ();
			subject.EventData.Should ().Be (expectedData);
		}
	}

}
