using Dev.Common.Caching;
using Dev.Core.Domain;

namespace Dev.Services;

internal static  class SettingsDefaults
{
    public static CacheKey SettingsAllAsDictionaryCacheKey => new("Dev.setting.all.dictionary.", "Dev.Setting");
}
