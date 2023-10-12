using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Configurations.Cache.CacheServices
{
    public interface ICacheService
    {
        Task ClearFromCache(string key);

        Task ClearFromCache(CacheKeySets cacheKeySets, string key);

        Task WriteToCache<T>(string key, T payload, CacheKeySets? cacheKeySets = null, TimeSpan? absoluteExpireTime = null);

        Task<T?> ReadFromCache<T>(string key) where T : class;

        Task<IEnumerable<T>> BulkReadFromCache<T>(CacheKeySets cacheKeySets) where T : class;

        Task<IEnumerable<T>> BulkReadFromCache<T>(string pattern) where T : class;
    }
}
