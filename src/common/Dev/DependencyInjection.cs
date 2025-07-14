using Dev.Common;
using Dev.Core.Interfaces;
using Dev.Infrastructure;
using Dev.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dev;

public static class DependencyInjection
{
    public static void AddDev(this IServiceCollection services, WebApplicationBuilder builder)
    {
        //create default file provider
        CommonHelper.DefaultFileProvider = new DevFileProvider(builder.Environment);
        services.AddDevDataProtection();

        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        services.AddScoped<ICoreDbContext, ICoreDbContext>(provider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            optionsBuilder.UseMySQL(connectionString);
            return new CoreDbContext(optionsBuilder.Options);
        });

        services.AddScoped<IDevFileProvider, DevFileProvider>();
        //register type finder
        var typeFinder = new TypeFinder();
        services.AddSingleton<ITypeFinder>(typeFinder);

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IWebHelper, WebHelper>();
    }

    public static void UseDev(this IApplicationBuilder app)
    {

    }

    private static void AddDevDataProtection(this IServiceCollection services)
    {

        var dataProtectionKeysPath = CommonHelper.DefaultFileProvider!.MapPath("~/App_Data/DataProtectionKeys");
        var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

        //configure the data protection system to persist keys to the specified directory
        services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
    }
}
