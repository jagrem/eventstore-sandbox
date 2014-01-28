using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EventStore.SandBox
{

	public class WriteFloodAgent : FloodAgent
	{
		private const int addItemsSize = 10;

		public WriteFloodAgent(int id, string streamId, CancellationToken cancellationToken, EventStoreAdapter eventStore)
			: base(id, streamId, cancellationToken, eventStore)
		{
		}

		public override StatsCounter Run()
		{
			Task.Factory.StartNew (() => {
				var expectedVersion = -1;
				while (!this.CancellationToken.IsCancellationRequested) {
					this.Counter.Start ();
					this.EventStore.SaveEventsWithId (this.StreamId, TestEvents.CreateEvents (addItemsSize), expectedVersion);
					expectedVersion += addItemsSize;
					this.Counter.Record (addItemsSize);
					Console.Write ("-W{0}", this.Id);
					this.Counter.Stop ();
				}
			}, this.CancellationToken)
				.ContinueWith (
					t => Console.WriteLine (t.Exception), 
					TaskContinuationOptions.OnlyOnFaulted);

			return base.Run ();
		}


	}

}
