﻿using System;
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

                // These commands will be executed once at a time
                var counter1Value = redisDb.StringIncrementAsync("counter:1", 1);
                var counter2Value = redisDb.StringIncrementAsync("counter:2", 10);

                Console.WriteLine("counter1 = {0}", counter1Value.Result);
                Console.WriteLine("counter2 = {0}", counter2Value.Result);

                var batch = redisDb.CreateBatch();
                // Both commands are run in a single round trip to the server 
                counter1Value = batch.StringIncrementAsync("counter:1", 1);
                counter2Value = batch.StringIncrementAsync("counter:2", 10);
                batch.Execute();

                Console.WriteLine("counter1 = {0}", counter1Value.Result);
                Console.WriteLine("counter2 = {0}", counter2Value.Result);
            }
        }
    }
}
