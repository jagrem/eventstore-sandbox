using NUnit.Framework;
using FluentAssertions;
using System;

namespace EventStore.SandBox.UnitTests
{
	[TestFixture]
	public class ProjectionTests
	{
		[Test]
		public void WhenNameIsNull()
		{
			Action action = () => new Projection (null);
			action.ShouldThrow<ArgumentNullException> ();
		}

		[Test]
		public void WhenTypeNotSpecified()
		{
			var subject = new Projection ("test");
			subject.ProjectionType.Should ().Be (ProjectionType.OneTime);
		}

		[Test]
		public void TestProjectionType()
		{
			var projectionType = ProjectionType.Continuous;
			var subject = new Projection ("test", projectionType);
			subject.ProjectionType.Should ().Be (projectionType);
		}

		[Test]
		public void TestName()
		{
			var subject = new Projection ("test");
			subject.Name.Should ().Be ("test");
		}

		[Test]
		public void TestFromAll()
		{
			var subject = new Projection ("test")
				.FromAll ();

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromAll()");
		}

		[Test]
		public void TestFromAllWhenAny()
		{
			var subject = new Projection ("test")
				.FromAll ()
				.WhenAny ((s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromAll().whenAny(function(s,e) { return null; })") ;
		}

		[Test]
		public void TestFromAllWhen()
		{
			var subject = new Projection("test")
				.FromAll ()
				.When(EventType.Any, (s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromAll().when({ '$any':function(s,e) { return null; } })");
		}

		[Test]
		public void TestFromAllInit()
		{
			var subject = new Projection("test")
				.FromAll ()
				.Init(() => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromAll().when({ '$init':function() { return null; } })");
		}

		[Test]
		public void TestFromAllSomeEvent()
		{
			var subject = new Projection("test")
				.FromAll ()
				.When("someEvent", (s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromAll().when({ 'someEvent':function(s,e) { return null; } })");
		}

		[Test]
		public void TestFromStream()
		{
			var subject = new Projection ("test")
				.FromStream ("events")
				.WhenAny((s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromStream('events').whenAny(function(s,e) { return null; })");
		}

		[Test]
		public void TestFromStreams()
		{
			var subject = new Projection ("test")
				.FromStreams (new[] {"stream1", "stream2", "stream3"})
				.WhenAny ((s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromStreams(['stream1','stream2','stream3']).whenAny(function(s,e) { return null; })");
		}

		[Test]
		public void TestFromCategory()
		{
			var subject = new Projection ("test")
				.FromCategory ("$stats");

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromCategory('$stats')");
		}

		[Test]
		public void TestForEachStream()
		{
			var subject = new Projection ("test")
				.FromStreams (new[] { "stream1", "stream2", "stream3" })
				.ForEachStream ()
				.WhenAny ((s, e) => null);

			var actual = subject.GetJson ();

			actual.Should ().Be ("fromStreams(['stream1','stream2','stream3']).foreachStream().whenAny(function(s,e) { return null; })");
		}
	}
}
		
