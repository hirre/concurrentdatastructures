using Xunit.Abstractions;

namespace ConcurrentDataStructures.Tests
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

        [Fact]
        public void TestQueueOperations()
        {
            var queue = new ConcurrentPriorityQueue<double>();

            var r = new Random();

            // Add 3 items prio 2
            queue.Enqueue(r.NextDouble() * 100);
            queue.Enqueue(r.NextDouble() * 100);
            queue.Enqueue(r.NextDouble() * 100);

            Assert.Empty(queue.GetQueueCopy(1));
            Assert.NotEmpty(queue.GetQueueCopy(2));
            Assert.Empty(queue.GetQueueCopy(3));

            queue.Enqueue(r.NextDouble() * 100, 1);
            queue.Enqueue(r.NextDouble() * 100, 1);
            queue.Enqueue(r.NextDouble() * 100, 1);
            queue.Enqueue(r.NextDouble() * 100, 1);

            queue.Enqueue(r.NextDouble() * 100, 3);
            queue.Enqueue(r.NextDouble() * 100, 3);
            queue.Enqueue(r.NextDouble() * 100, 3);
            queue.Enqueue(r.NextDouble() * 100, 3);

            queue.Dequeue(); // Remove prio 1
            queue.Dequeue(); // Remove prio 1
            queue.Dequeue(); // Remove prio 1

            Assert.Equal(1, queue.GetQueueCopy(1).Count); // One prio 1 left

            queue.Dequeue(); // Remove prio 1

            Assert.Empty(queue.GetQueueCopy(1)); // No more prio 1 left

            queue.Dequeue(); // Remove prio 2

            Assert.Equal(2, queue.GetQueueCopy(2).Count); // Two prio 2 left

            queue.Dequeue(); // Remove prio 2
            queue.Dequeue(); // Remove prio 2

            Assert.Empty(queue.GetQueueCopy(2)); // No more prio 2 left

            queue.Dequeue(); // Remove prio 3
            queue.Dequeue(); // Remove prio 3
            queue.Dequeue(); // Remove prio 3

            Assert.Equal(1, queue.GetQueueCopy(3).Count); // 1 prio 3 left

            queue.Dequeue(); // Remove prio 3

            Assert.Empty(queue.GetQueueCopy(3)); // No more prio 3 left

            Assert.Equal(0, queue.Count); // Queue empty

            var item = queue.Dequeue(); // Empty item

            Assert.Equal(default, item);

            // Enqueue 3 items prio 2
            queue.Enqueue(r.NextDouble() * 100);
            queue.Enqueue(r.NextDouble() * 100);
            queue.Enqueue(r.NextDouble() * 100);

            queue.Clear(); // Clear queue

            Assert.Equal(0, queue.Count);

        }
    }
}