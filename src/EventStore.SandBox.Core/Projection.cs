using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventStore.SandBox;

namespace EventStore.SandBox
{
	public class Projection
	{
		// Use MethodInfo.GetMethodBody().GetILAsByteArray() 
		// to pass IL to MSIL to JavaScript compiler

		private string json = string.Empty;

		public Projection (string projectionName)
		{
			if (projectionName == null) {
				throw new ArgumentNullException ("projectionName");
			}

			this.Name = projectionName;
			this.ProjectionType = ProjectionType.OneTime;
		}

		public Projection(string projectionName, ProjectionType projectionType)
		{
			this.Name = projectionName;
			this.ProjectionType = projectionType;
		}

		public string Name { get; private set; }

		public ProjectionType ProjectionType { get; private set; }

		public Projection FromAll()
		{
			this.json = "fromAll()";
			return this;
		}

		public Projection FromStream(string streamName)
		{
			this.json = "fromStream('" + streamName + "')";
			return this;
		}

		public Projection FromStreams(IEnumerable<string> streamNames)
		{
			this.json = "fromStreams([" + string.Join (",", streamNames.Select(name => "'" + name + "'")) + "])";
			return this;
		}

		public Projection FromCategory(string categoryName)
		{
			this.json = "fromCategory('" + categoryName + "')";
			return this;
		}

		public Projection ForEachStream()
		{
			this.json += ".foreachStream()";
			return this;
		}

		public Projection WhenAny(string rawJson)
		{
			this.json += string.Format(@".whenAny({0})", rawJson);
			return this;
		}

		public Projection WhenAny(Func<object, IEvent, object> onWhenAny)
		{
			this.json += ".whenAny(function(s,e) { return null; })";
			return this;
		}			

		public Projection When(string eventType, string rawJson)
		{
			this.json += string.Format (".when({{ '{0}':{1} }})", eventType, rawJson);
			return this;
		}

		public Projection When(EventType eventType, Func<object, IEvent, object> onAny)
		{
			this.json += ".when({ '" + eventType.Name + "':function(s,e) { return null; } })";
			return this;
		}

		public Projection Init(Func<object> onInit)
		{
			this.json += ".when({ '$init':function() { return null; } })";
			return this;
		}

		public string ToJson()
		{
			return json;
		}
	}
}
