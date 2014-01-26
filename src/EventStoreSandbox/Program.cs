using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EventStore.SandBox
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Write ("**** EVENTSTORE PLAYGROUND ****\nVersion: 0.0.1.0\nAuthor: James McAuley\nCopyright 2013\n******************************\n");
			Stopwatch writeStopwatch = new Stopwatch ();
			Stopwatch readStopwatch = new Stopwatch ();
			var eventStore = new EventStoreAdapter ("10.1.1.11");
			Thread.Sleep (200);
			var streamId = "events-" + Guid.NewGuid ();
			const int size = 10;
			int totalWrites = 0;
			int totalReads = 0;
			bool isFlooding = false;
			var expectedVersion = -1;
			char c = '0';

			while (c != 'q') {
				switch (c) {
				case 'n':
					expectedVersion = -1;
					streamId = "events-" + Guid.NewGuid ();
					Console.Write ("\nCreating new stream with id {0}\n", streamId);
					break;
				case 'a':
					writeStopwatch.Start ();
					eventStore.SaveEventsWithId (streamId, CreateEvents (size), expectedVersion);
					writeStopwatch.Stop ();
					Console.Write ("\nSaving {0} events with id {1}\n", size, streamId);
					expectedVersion += size;
					totalWrites += size;
					break;
				case 'd':
					eventStore.DeleteEventsWithId (streamId, expectedVersion);
					Console.Write ("\nDeleting stream {0}\n", streamId);
					expectedVersion = -1;
					totalWrites += 1;
					break;
				case 'h':
					readStopwatch.Start ();
					var loadedEvents = eventStore.LoadEventsForId (streamId, 0).ToList ();
					readStopwatch.Stop ();
					Console.Write ("\nLoading {0} events for id {1}\n", loadedEvents.Count, streamId);
					totalReads += loadedEvents.Count;
					loadedEvents.ForEach (x => Console.WriteLine ("{0} | {1}", x.Header.EventId, x.Body.EventType));
					break;
				case 't':
					readStopwatch.Start ();
					var lastEvents = eventStore.LoadLastEventsForId (streamId, 5).ToList ();
					readStopwatch.Stop ();
					Console.Write ("\nLoading last {0} events for id {1}\n", lastEvents.Count, streamId);
					totalReads += lastEvents.Count;
					lastEvents.ForEach (x => Console.WriteLine ("{0} | {1}", x.Header.EventId, x.Body.EventType));
					break;
				case 'f':
					Console.Write ("\nFlooding stream {0} with events\n[s]top\nplayground :", streamId);

					//if (!isFlooding) {
						isFlooding = true;

						// write
						Task.Factory.StartNew (() => {
							writeStopwatch.Start ();
							var w_streamId = "events-" + Guid.NewGuid();
							var w_expectedVersion = -1;

							while (isFlooding) {
								eventStore.SaveEventsWithId (w_streamId, CreateEvents (size), w_expectedVersion);
								w_expectedVersion += size;
								totalWrites += size;
								Console.Write (".");
							}
							writeStopwatch.Stop ();
						}).ContinueWith (t => Console.WriteLine (t.Exception), TaskContinuationOptions.OnlyOnFaulted);

						// read
						Task.Factory.StartNew (() => {
							readStopwatch.Start ();
							while (isFlooding) {
								if (c == 's') {
									isFlooding = false;
								}
								var events = eventStore.LoadLastEventsForId (streamId, 100).ToList ();
								totalReads += events.Count;
								Console.Write ("*");
							}
							readStopwatch.Stop ();
						}).ContinueWith (t => Console.WriteLine (t.Exception), TaskContinuationOptions.OnlyOnFaulted);
					//}
					break;
				case 's':
					isFlooding = false;
					break;
				}

				Console.Write("\n[n]ew [a]dd, [d]elete, [h]istory [t]ail [q]uit\nplayground :");
				c = (char)Console.Read ();
			}

			isFlooding = false;
			Console.WriteLine ("\nStopping...");
			Console.WriteLine ("Wrote {0} events in {1}", totalWrites, writeStopwatch.Elapsed);
			Console.WriteLine ("At an average of {0} writes per second", totalWrites/((double)writeStopwatch.Elapsed.Seconds));
			Console.WriteLine ("Read {0} events in {1}", totalReads, readStopwatch.Elapsed);
			Console.WriteLine ("At an average of {0} reads per second", totalReads/((double)readStopwatch.Elapsed.Seconds));
		}

		private static IEnumerable<Event> CreateEvents(int count)
		{
			var builder = EventBuilder.Create();
			return Enumerable.Range (1, count).Select (x => builder.Build (new HelloWorld { Message = "Hi" }));
		}
	}

}
