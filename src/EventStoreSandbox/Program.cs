using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EventStore.SandBox
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Write ("**** EVENTSTORE PLAYGROUND ****\nVersion: 0.0.1.0\nAuthor: James McAuley\nCopyright 2013\n******************************\n");
			var eventStore = new EventStoreAdapter ("10.1.1.11");
			Thread.Sleep (200);

			var streamId = "events-" + Guid.NewGuid ();
			const int addItemsSize = 10;
			bool isFlooding = false;
			var expectedVersion = -1;
			var floodAgentCount = 0;
			char userInputCharacter = '0';
			var writeCounters = new List<StatsCounter> ();
			var readCounters = new List<StatsCounter> ();
			var cancellationTokenSource = new CancellationTokenSource ();
			var cancellationToken = cancellationTokenSource.Token;

			while (userInputCharacter != 'q') {
				switch (userInputCharacter) {
				case 'n':
					expectedVersion = -1;
					streamId = "events-" + Guid.NewGuid ();
					Console.Write ("\nCreating new stream with id {0}\n", streamId);
					break;
				case 'a':
					eventStore.SaveEventsWithId (streamId, TestEvents.CreateEvents (addItemsSize), expectedVersion);
					Console.Write ("\nSaving {0} events with id {1}\n", addItemsSize, streamId);
					expectedVersion += addItemsSize;
					break;
				case 'd':
					eventStore.DeleteEventsWithId (streamId, expectedVersion);
					Console.Write ("\nDeleting stream {0}\n", streamId);
					expectedVersion = -1;
					break;
				case 'h':
					var loadedEvents = eventStore.LoadEventsForId (streamId, 0).ToList ();
					Console.Write ("\nLoading {0} events for id {1}\n", loadedEvents.Count, streamId);
					loadedEvents.ForEach (x => Console.WriteLine ("{0} | {1}", x.Header.EventId, x.Body.EventType));
					break;
				case 't':
					var lastEvents = eventStore.LoadLastEventsForId (streamId, 5).ToList ();
					Console.Write ("\nLoading last {0} events for id {1}\n", lastEvents.Count, streamId);
					lastEvents.ForEach (x => Console.WriteLine ("{0} | {1}", x.Header.EventId, x.Body.EventType));
					break;
				case 'f':
					if (!isFlooding) {
						isFlooding = true;
						floodAgentCount = 0;
					}

					floodAgentCount++;
					var id = "events-" + Guid.NewGuid ();

					writeCounters.Add(new WriteFloodAgent (floodAgentCount, id, cancellationToken , eventStore).Run());
					readCounters.Add(new ReadFloodAgent (floodAgentCount, id, cancellationToken , eventStore).Run());
					break;
				case 's':
					cancellationTokenSource.Cancel ();
					break;
				}

				Console.Write("\n[n]ew [a]dd, [d]elete, [h]istory [t]ail [f]lood [s]top [q]uit\nplayground :");
				userInputCharacter = (char)Console.Read ();
			}

			cancellationTokenSource.Cancel ();
			Console.WriteLine ("\nStopping...");
			var totalWriteEvents = writeCounters.Sum (x => x.TotalEvents);
			var writesTimeSpan = new TimeSpan (writeCounters.Sum (x => x.TotalTime.Ticks));
			Console.WriteLine ("Wrote {0} events in {1}", totalWriteEvents, writesTimeSpan);
			Console.WriteLine ("At an average of {0} writes per second", totalWriteEvents/((double)writesTimeSpan.Seconds));

			var totalReadEvents = readCounters.Sum (x => x.TotalEvents);
			var readsTimeSpan = new TimeSpan (readCounters.Sum (x => x.TotalTime.Ticks));
			Console.WriteLine ("Read {0} events in {1}", totalReadEvents, readsTimeSpan);
			Console.WriteLine ("At an average of {0} reads per second", totalReadEvents/((double)readsTimeSpan.Seconds));
		}
	}
}
