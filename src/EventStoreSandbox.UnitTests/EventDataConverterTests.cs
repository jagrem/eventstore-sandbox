using System;
using EventStore.ClientAPI;
using FluentAssertions;
using NUnit.Framework;

namespace EventStore.SandBox.UnitTests
{
	[TestFixture]
	public class EventDataConverterTests
	{
		internal class AnyEvent : IEvent
		{
			public AnyEvent (IEventHeader eventHeader, IEventBody eventBody)
			{
				this.Header = eventHeader;
				this.Body = eventBody;
			}

			public IEventHeader Header { get; set; }

			public IEventBody Body { get; set; }
		}

		[TestFixture]
		internal class ToEventDataTests
		{
			[Test]
			public void WhenEventIsNull ()
			{
				var subject = new EventDataConverter ();

				Action action = () => subject.ToEventData (null);

				action.ShouldThrow<ArgumentNullException> ()
				.WithMessage ("*eventToConvert*");
			}

			[Test]
			public void WhenEventHeaderIsMissing ()
			{
				var subject = new EventDataConverter ();

				Action action = () => subject.ToEventData (new AnyEvent (null, new AnyEventBody ()));

				action.ShouldThrow<ArgumentException> ()
				.WithMessage ("*event header is required*");
			}

			[Test]
			public void WhenEventBodyIsMissing ()
			{
				var subject = new EventDataConverter ();

				Action action = () => subject.ToEventData (new AnyEvent (new AnyEventHeader (), null));

				action.ShouldThrow<ArgumentException> ()
				.WithMessage ("*event body is required*");
			}

			[Test]
			public void WhenEventBodyEventTypeIsMissing ()
			{
				var subject = new EventDataConverter ();

				var actual = subject.ToEventData (new AnyEvent (new AnyEventHeader (), new AnyEventBody ()));

				actual.Type.Should ().BeNull ();
			}

			[Test]
			public void WhenEventBodyEventDataIsMissing ()
			{
				var subject = new EventDataConverter ();

				var actual = subject.ToEventData (new AnyEvent (new AnyEventHeader (), new AnyEventBody ()));

				actual.Data.Should ().NotBeNull ();
				actual.Data.Should ().BeEmpty ();
			}

			[Test]
			public void TestToEventData ()
			{
				var expectedEventType = typeof(string);
				var anyEvent = new AnyEvent (new AnyEventHeader {
					EventId = "eventId"
				}, new AnyEventBody {
					EventType = expectedEventType,
					EventData = "hi"
				});

				var subject = new EventDataConverter ();

				var actual = subject.ToEventData (anyEvent);

				actual.EventId.Should ().NotBeEmpty ();
				actual.IsJson.Should ().BeTrue ();
				actual.Type.Should ().Be (expectedEventType.FullName);
				actual.Metadata.Should ().NotBeNull ();
				actual.Metadata.Should ().NotBeEmpty ();
				actual.Data.Should ().NotBeNull ();
				actual.Data.Should ().NotBeEmpty ();
			}
		}

		[TestFixture]
		internal class FromEventDataTests
		{
			[Test]
			public void WhenRawEventIsNull()
			{
				var subject = new EventDataConverter ();

				Action action = () => subject.FromEventData (null);

				action.ShouldThrow<ArgumentNullException> ()
					.WithMessage ("*rawEvent*");
			}
		}
	}
}

