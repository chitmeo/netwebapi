namespace Dev.Common.Caching;

internal interface ICacheKeyService
{
    CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters);
    CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters);
}
