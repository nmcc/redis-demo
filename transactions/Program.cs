using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Run MONITOR command on redis-cli to display the commands that are being executed
            using (var redis = ConnectionMultiplexer.Connect("127.0.0.1"))
            {
                var redisDb = redis.GetDatabase();

                var transaction = redisDb.CreateTransaction();
                // This is specific of the StackExchange API
                transaction.AddCondition(Condition.KeyExists("incrementCounters"));

                // Both commands are run in a single round trip to the server 
                var counter1Value = transaction.StringIncrementAsync("counter:1", 1);
                var counter2Value = transaction.StringIncrementAsync("counter:2", 10);
                transaction.Execute();

                if (counter1Value.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("counter1 = {0}, counter2 = {1}", counter1Value.Result, counter2Value.Result);
                else
                    Console.WriteLine("Counters were not updated");
            }
        }
    }
}
