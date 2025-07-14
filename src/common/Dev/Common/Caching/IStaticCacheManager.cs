namespace Dev.Common.Caching;

internal interface IStaticCacheManager: IDisposable, ICacheKeyService
{
    Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);
    Task<T> GetAsync<T>(CacheKey key, Func<T> acquire);
    Task<T> GetAsync<T>(CacheKey key, T defaultValue = default);
    Task<object> GetAsync(CacheKey key);
    Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters);
    Task SetAsync<T>(CacheKey key, T data);
    Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);
    Task ClearAsync();
}
