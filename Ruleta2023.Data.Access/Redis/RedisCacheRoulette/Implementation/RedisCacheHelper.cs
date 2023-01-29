using Autofac.Features.AttributeFilters;
using Ruleta2023.Data.Access.Redis.RedisCacheRoulette.Contract;
using Ruleta2023.Domain.Data.Bets;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ruleta2023.Data.Access.Redis.RedisCacheRoulette.Implementation
{
    public class RedisCacheHelper : ICacheHelper
    {
        private readonly IDatabase db;
        private readonly ConnectionMultiplexer redis;

        public RedisCacheHelper([KeyFilter("RedisUrl")] string redisUrl)
        {
            redis = ConnectionMultiplexer.Connect(redisUrl);
            db = redis.GetDatabase();
        }

        public async Task<string> Get(string key)
        {
            try
            {
                return await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In Get Redis client, key [{key}]");
            }
            return string.Empty;
        }

        public async Task Set(string key, string value, int ttlKeyS)
        {
            try
            {
                db.ListRightPush(key, value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In Set Redis client, key [{key}], value [{value}], ttlKeyMs [{ttlKeyS}]");
            }
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

        public async Task<List<BetClass>> GetAllBets(string id)
        {
            List<BetClass> result = new List<BetClass>();
            try
            {
                var response = await db.ListRangeAsync(id, 0, -1);
                foreach (var item in response)
                {
                    result.Add(JsonConvert.DeserializeObject<BetClass>(item));
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"In Get Redis client, doesn´t match");
            }
            return result;
        }
    }
}
