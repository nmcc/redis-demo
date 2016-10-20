using System;
using System.IO;
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
                var server = redis.GetServer("127.0.0.1:6379");

                // Reset the key
                redisDb.KeyDelete("statistics");

                // Converts the script to a single line in order to be loaded to Redis
                // Also does some StackExchange.Redis specific arguments replacement
                LuaScript prepared = LuaScript.Prepare(File.ReadAllText("calculate_stats.lua.txt"));

                // Loads the script into the server and returns the SHA
                LoadedLuaScript loaded = prepared.Load(server);
                Console.WriteLine("Script SHA is " + BitConverter.ToString(loaded.Hash).Replace("-", String.Empty));

                for (int i = 1; i <= 5; ++i)
                {
                    loaded.Evaluate(redisDb, new { key = (RedisKey)"statistics", value = 10 * i });
                }
            }
        }
    }
}
