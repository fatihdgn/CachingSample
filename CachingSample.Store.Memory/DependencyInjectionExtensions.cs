using CachingSample.Store.Abstractions;
using CachingSample.Store.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CachingSample.Store.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseMemoryStore(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IStore<>), typeof(DelayedMemoryStore<>));
    }
}
