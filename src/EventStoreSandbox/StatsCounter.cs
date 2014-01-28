using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EventStore.SandBox
{
	
	public class StatsCounter
	{
		private readonly Stopwatch stopwatch;

		private int total;

		public StatsCounter()
		{
			this.stopwatch = new Stopwatch();
		}

		public void Start()
		{
			this.stopwatch.Start ();
		}

		public void Stop()
		{
			this.stopwatch.Stop ();
		}

		public void Record(int count)
		{
			this.total += count;
		}

		public int TotalEvents
		{
			get { return this.total; }
		}

		public TimeSpan TotalTime
		{
			get { return this.stopwatch.Elapsed; }
		}
	}
}
