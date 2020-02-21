using System;
using StackExchange.Redis;

namespace hash_demo
{
    class Program
    {
        private static IDatabase redisDb;

        static void Main(string[] args)
        {
            using (var redis = ConnectionMultiplexer.Connect("127.0.0.1"))
            {
                redisDb = redis.GetDatabase();

                // Initiatilize
                const int pageId = 1;
                var key = BuildKey(pageId);

                InitPage(key);

                var stats = redisDb.HashValues(key);

                redisDb.HashIncrement(key, "PageViews", 1);
            }
        }

        static void InitPage(string key)
        {
            var exists = redisDb.KeyExists(key);

            if (exists)
            {
                return;
            }

            var stats = new HashEntry[]
            {
                new HashEntry("PageVisits", 0),
                new HashEntry("PageActions", 0),
            };

            redisDb.HashSet(key, stats);
        }

        static void Dump(string key)
        {

        }

        static string BuildKey(int pageId) => $"Page:{pageId}";
    }
}
