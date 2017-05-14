using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ConsoleApplication
{
    public class Program
    {
        public static readonly String TOPIC_NAME = "messages";

        public static void Main(string[] args)
        {
            var consumers = new List<Task>();
            var producers = new List<Task>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            CreateConsumers(1, consumers, cancellationToken);
            CreateProducers(1, producers, cancellationToken);

            consumers.ForEach(c => c.Start());
            producers.ForEach(p => p.Start());

            Console.WriteLine("Press any key to end");
            Console.ReadKey();

            cancellationTokenSource.Cancel();

            consumers.ForEach(p => p.Wait());
            consumers.ForEach(c => c.Wait());
        }

        private static void CreateProducers(int NUMBER_OF_PRODUCERS, List<Task> producers, CancellationToken cancellationToken)
        {
            for (int i = 0; i < NUMBER_OF_PRODUCERS; ++i)
            {
                var producerName = "producer:" + i;

                producers.Add(new Task(() =>
                {
                    var producer = new Producer(producerName);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        producer.ProduceMessage();
                        Thread.Sleep(500);
                    }
                }));
            }
        }

        private static void CreateConsumers(int NUMBER_OF_CONSUMERS, List<Task> consumers, CancellationToken cancellationToken)
        {
            for (int i = 0; i < NUMBER_OF_CONSUMERS; ++i)
            {
                var consumerName = "consumer:" + i;

                consumers.Add(new Task(() =>
                {
                    var consumer = new Consumer(consumerName);
                    
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(500);
                    }
                }));
            }
        }
    }

    public class Producer : RedisSubscriber
    {
        private int counter = 0;

        public Producer(String name) : base(name)
        {
        }

        public void ProduceMessage()
        {
            this.Subscriber.Publish(Program.TOPIC_NAME, String.Format("{0}:{1}", this.Name, counter));
            this.counter++;
        }
    }

    public class Consumer : RedisSubscriber
    {
        public Consumer(String name) : base(name)
        {
            this.Subscriber.Subscribe(Program.TOPIC_NAME, (channel, message) =>
            {
                System.Console.WriteLine("{0}: Got message {1}", this.Name, message);
            });
        }
    }

    public class RedisSubscriber : IDisposable
    {
        private readonly ConnectionMultiplexer redis;

        public RedisSubscriber(String name)
        {
            this.Name = name;
            this.redis = ConnectionMultiplexer.Connect("127.0.0.1");
            this.Subscriber = redis.GetSubscriber();
        }

        protected ISubscriber Subscriber { get; private set; }
        protected String Name { get; private set; }

        void IDisposable.Dispose()
        {
            this.redis.Dispose();
        }
    }
}
