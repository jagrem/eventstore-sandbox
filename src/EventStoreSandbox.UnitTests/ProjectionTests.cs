using NUnit.Framework;
using FluentAssertions;
using System;

namespace EventStore.SandBox.UnitTests
{
	[TestFixture]
	public class ProjectionTests
	{
		[TestFixture]
		public class WhenCreatingAnInstanceWithNameOnly
		{
			private const string ProjectionName = "test";

			[Test]
			public void ShouldThrowIfNameIsNull()
			{
				Action action = () => new Projection (null);
				action.ShouldThrow<ArgumentNullException> ()
					.WithMessage ("*name*");
			}

			[Test]
			public void ShouldThrowIfNameIsEmpty()
			{
				Action action = () => new Projection (string.Empty);
				action.ShouldThrow<ArgumentException> ()
					.WithMessage ("*name*");
			}

			[Test]
			public void ShouldBeOneTimeProjectionByDefault()
			{
				var subject = new Projection (ProjectionName);
				subject.ProjectionType.Should ().Be (ProjectionType.OneTime);
			}

			[Test]
			public void ShouldHaveSpecifiedName()
			{
				var subject = new Projection (ProjectionName);
				subject.Name.Should ().Be (ProjectionName);
			}
		}

		[TestFixture]
		public class WhenCreatingAnInstanceWithNameAndProjectionType
		{
			private const string ProjectionName = "test";
			private readonly ProjectionType ProjectionType = ProjectionType.Continuous;

			[Test]
			public void ShouldThrowIfNameIsNull()
			{
				Action action = () => new Projection (null, ProjectionType);
				action.ShouldThrow<ArgumentNullException> ()
					.WithMessage ("*name*");
			}

			[Test]
			public void ShouldThrowIfNameIsEmpty()
			{
				Action action = () => new Projection (string.Empty, ProjectionType);
				action.ShouldThrow<ArgumentException> ()
					.WithMessage ("*name*");
			}

			[Test]
			public void ShouldHaveSpecifiedName()
			{
				var subject = new Projection (ProjectionName, ProjectionType);
				subject.Name.Should ().Be (ProjectionName);
			}

			[Test]
			public void ShouldHaveSpecifiedProjectionType()
			{
				var subject = new Projection (ProjectionName, ProjectionType);
				subject.ProjectionType.Should ().Be (ProjectionType);
			}
		}

		[TestFixture]
		public class WhenSelectingFromAllStreams
		{
			private const string ProjectionName = "test";

			[Test]
			public void ShouldSelectFromAllStreams()
			{
				var subject = new Projection (ProjectionName)
					.FromAll ();
				var actual = subject.ToJson ();
				actual.Should ().Be ("fromAll()");
			}

			[Test]
			public void ShouldSelectFromAllStreamsWhenAny()
			{
				var subject = new Projection (ProjectionName)
					.FromAll ()
					.WhenAny ("function(s,e) { return null; }");

				var actual = subject.ToJson ();

				actual.Should ().Be ("fromAll().whenAny(function(s,e) { return null; })") ;
			}

			[Test]
			public void ShouldSelectFromAllStreamsWhen()
			{
				var subject = new Projection(ProjectionName)
					.FromAll ()
					.When("someEvent", "function(s,e) { return null; }");

				var actual = subject.ToJson ();

				actual.Should ().Be ("fromAll().when({ 'someEvent':function(s,e) { return null; } })");
			}

			[Test]
			public void ShouldAllowMulitpleWhenClauses()
			{
				var subject = new Projection (ProjectionName)
					.FromAll ()
					.When ("someEvent", "function(s,e) { s + 1; }")
					.When ("someEvent", "function(s,e) { s + 2; }");

				var actual = subject.ToJson ();

				actual.Should ().Be ("fromAll().when({ 'someEvent1':function(s,e) { s + 1; }, 'someEvent2':function(s,e) { s + 2; } })");
			}

			[Test]
			public void ShouldNotAllowWhenClausesTargetingTheSameEventType()
			{
				Action action = () => new Projection (ProjectionName)
					.FromAll ()
					.When ("someEvent1", "function(s,e) { s + 1; }")
					.When ("someEvent2", "function(s,e) { s + 2; }");

				action.ShouldThrow<InvalidOperationException> ();
			}
		}
			
		[Test]
		public void TestFromAllInit()
		{
			var subject = new Projection("test")
				.FromAll ()
				.Init(() => null);

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromAll().when({ '$init':function() { return null; } })");
		}

		[Test]
		public void TestFromAllSomeEvent()
		{
			var subject = new Projection("test")
				.FromAll ()
				.When("someEvent", (s, e) => null);

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromAll().when({ 'someEvent':function(s,e) { return null; } })");
		}

		[Test]
		public void TestFromStream()
		{
			var subject = new Projection ("test")
				.FromStream ("events")
				.WhenAny((s, e) => null);

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromStream('events').whenAny(function(s,e) { return null; })");
		}

		[Test]
		public void TestFromStreams()
		{
			var subject = new Projection ("test")
				.FromStreams (new[] {"stream1", "stream2", "stream3"})
				.WhenAny ((s, e) => null);

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromStreams(['stream1','stream2','stream3']).whenAny(function(s,e) { return null; })");
		}

		[Test]
		public void TestFromCategory()
		{
			var subject = new Projection ("test")
				.FromCategory ("$stats");

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromCategory('$stats')");
		}

		[Test]
		public void TestForEachStream()
		{
			var subject = new Projection ("test")
				.FromStreams (new[] { "stream1", "stream2", "stream3" })
				.ForEachStream ()
				.WhenAny ((s, e) => null);

			var actual = subject.ToJson ();

			actual.Should ().Be ("fromStreams(['stream1','stream2','stream3']).foreachStream().whenAny(function(s,e) { return null; })");
		}
	}
}
