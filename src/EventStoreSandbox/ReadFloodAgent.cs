using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EventStore.SandBox
{
	
	public class ReadFloodAgent : FloodAgent
	{
		private const int numberOfItemsToRead = 100;

		public ReadFloodAgent(int id, string streamId, CancellationToken cancellationToken, EventStoreAdapter eventStore)
			:base(id, streamId, cancellationToken, eventStore)
		{
		}

		public override StatsCounter Run ()
		{
			Task.Factory.StartNew (() => {
				while (!this.CancellationToken.IsCancellationRequested) {
					this.Counter.Start ();
					var events = this.EventStore.LoadLastEventsForId (
						this.StreamId, 
						numberOfItemsToRead)
						.ToList ();
					this.Counter.Record (events.Count);
					Console.Write ("-R{0}", this.Id);
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
