using CachingSample.Store.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CachingSample.Store.Caching.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseDefaultCaching(this IServiceCollection services)
    {
        return services.AddScoped(typeof(ICachedStore<>), typeof(DefaultCachedStore<>));
    }
}
