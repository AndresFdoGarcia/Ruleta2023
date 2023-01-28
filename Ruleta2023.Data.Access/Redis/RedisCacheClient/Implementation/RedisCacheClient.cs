using Autofac.Features.AttributeFilters;
using Ruleta2023.Data.Access.Redis.RedisCacheClient.Contract;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.Redis.RedisCacheClient.Implementation
{
    public class RedisCacheClient : ICacheClient
    {
        private readonly IDatabase db;
        private readonly ConnectionMultiplexer redis;

        public RedisCacheClient([KeyFilter("RedisUrl")] string redisUrl)
        {
            redis = ConnectionMultiplexer.Connect(redisUrl);
            db = redis.GetDatabase(1);
        }

        public async Task<string> Get(string key)
        {
            try
            {
                var response = await db.StringGetAsync(key);
                return response.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In Get Redis client, key [{key}]");
            }
            return string.Empty;
        }

        public Task Set(string key, string value, int ttlKeyS)
        {
            try
            {
                //db.ListRightPush(key, value);
                db.StringSet(key, value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In Set Redis client, key [{key}], value [{value}], ttlKeyMs [{ttlKeyS}]");
            }

            return Task.CompletedTask;
        }

        public async Task DeleteKey(string key)
        {
            try
            {
                await db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In DeleteKey Redis client, key [{key}]");
            }
        }
    }
}
