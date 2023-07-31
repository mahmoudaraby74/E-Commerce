using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface iResponseCacheService
    {
        Task CacheResponseAsync(string key, string response, TimeSpan timeToLive);
        Task<string> GetCachedResponse(string key);
    }
}
