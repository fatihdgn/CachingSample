using CachingSample.Store.Abstractions;

namespace CachingSample.Store.Caching.Abstractions;

// Currently exists solely for dependency injection purposes.
public interface ICachedStore<TItem> : IStore<TItem>
    where TItem : IStoreItem
{

}
