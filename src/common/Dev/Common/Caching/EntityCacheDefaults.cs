namespace Dev.Common.Caching;

internal static class EntityCacheDefaults<TEntity> where TEntity : class
{
    public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

    public static CacheKey ByIdCacheKey => new($"Dev.{EntityTypeName}.byid.{{0}}", ByIdPrefix, Prefix);

    public static CacheKey ByIdsCacheKey => new($"Dev.{EntityTypeName}.byids.{{0}}", ByIdsPrefix, Prefix);

    public static CacheKey AllCacheKey => new($"Dev.{EntityTypeName}.all.", AllPrefix, Prefix);

    public static string Prefix => $"Dev.{EntityTypeName}.";

    public static string ByIdPrefix => $"Dev.{EntityTypeName}.byid.";

    public static string ByIdsPrefix => $"Dev.{EntityTypeName}.byids.";

    public static string AllPrefix => $"Dev.{EntityTypeName}.all.";
}
