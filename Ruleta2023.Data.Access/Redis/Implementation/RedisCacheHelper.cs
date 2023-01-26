﻿using Autofac.Features.AttributeFilters;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.Redis.Implementation
{
    public class RedisCacheHelper
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
                await db.StringSetAsync(key, value, TimeSpan.FromSeconds(ttlKeyS));
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
    }
}