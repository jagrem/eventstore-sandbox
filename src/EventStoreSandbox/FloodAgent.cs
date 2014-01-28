using System.Threading;

namespace EventStore.SandBox
{
	public abstract class FloodAgent
	{
		protected readonly int Id;

		protected readonly string StreamId;

		protected readonly StatsCounter Counter;

		protected readonly EventStoreAdapter EventStore;

		protected readonly CancellationToken CancellationToken;

		protected FloodAgent(int id, string streamId, CancellationToken cancellationToken, EventStoreAdapter eventStore)
		{
			this.Id = id;
			this.StreamId = streamId;
			this.EventStore = eventStore;
			this.CancellationToken = cancellationToken;
			this.Counter = new StatsCounter();
		}

		public virtual StatsCounter Run()
		{
			return this.Counter;
		}
	}
}
