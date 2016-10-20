using System;
using StackExchange.Redis;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Run MONITOR command on redis-cli to display the commands that are being executed
            using (var redis = ConnectionMultiplexer.Connect("localhost"))
            {
                var redisDb = redis.GetDatabase();

                var batch = redisDb.CreateBatch();
                // Both commands are run in a single round trip to the server 
                var counter1Value = batch.StringIncrementAsync("counter:1", 1);
                var counter2Value = batch.StringIncrementAsync("counter:2", 10);
                batch.Execute();

                Console.WriteLine("counter1 = {0}, counter2 = {1}", counter1Value.Result, counter2Value.Result);
            }
        }
    }
}
