using Xunit.Abstractions;

namespace ConcurrentDataStructure.Tests
{
    public class QueueTests
    {
        private readonly ITestOutputHelper _output;

        public QueueTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(1, 1_000)]
        [InlineData(45, 1_000)]
        [InlineData(999, 1_000)]
        [InlineData(1, 100_000)]
        [InlineData(45, 100_000)]
        [InlineData(999, 100_000)]
        [InlineData(1, 1_000_000)]
        [InlineData(45, 1_000_000)]
        [InlineData(999, 1_000_000)]
        [InlineData(1, 10_000_000)]
        [InlineData(45, 10_000_000)]
        [InlineData(999, 10_000_000)]
        public void TestQueuePerformance(int nrOfPrios, int nrOfQueueItems)
        {
            var queue = new ConcurrentPriorityQueue<double>(nrOfPrios);
            var r = new Random();

            _output.WriteLine($"Number of priorities: {queue.NrOfPriorities}");

            var startTime = DateTime.Now;

            for (int i = 0; i < nrOfQueueItems; i++)
            {
                var prio = r.Next(nrOfPrios + 1);

                var val = r.NextDouble() * 1000;

                queue.Enqueue(val, prio);
            }

            _output.WriteLine($"Number of queued items: {queue.Count}");

            Assert.Equal(nrOfQueueItems, queue.Count);

            while (queue.Count != 0)
            {
                var item = queue.Dequeue();
            }

            _output.WriteLine($"Items left in queue after dequeue operation: {queue.Count}");

            Assert.Equal(0, queue.Count);

            var stopTime = (DateTime.Now - startTime).TotalMilliseconds;

            _output.WriteLine($"Time spent = {stopTime} ms");
        }
    }
}