using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace infrastructure.Service
{
    public class ResponseCacheService : iResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }
        public async Task CacheResponseAsync(string key, string response, TimeSpan timeToLive)
        {
            if (response!=null) 
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var serilizedReponse =JsonSerializer.Serialize(response, options);
                await _database.StringSetAsync(key, serilizedReponse, timeToLive);
            }
        }

        public async Task<string> GetCachedResponse(string key)
        {
           var CacheResponse = await _database.StringGetAsync(key);
            if (CacheResponse.IsNullOrEmpty)
                return null;
            return CacheResponse;
        }
    }
}
