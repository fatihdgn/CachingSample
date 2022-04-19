using CachingSample.Store.Abstractions;
using System.Collections.Concurrent;

namespace CachingSample.Store;

public class ItemsStore : IStore
{
    private static readonly ConcurrentDictionary<StoreItemId, IStoreItem> _items = new ConcurrentDictionary<StoreItemId, IStoreItem>();
    static ItemsStore()
    {
        Populate();
    }
    private static void Populate()
    {
        if (_items.Count == 0)
        {
            foreach (var item in StoreItemGenerator.Generate(5))
            {
                _items.TryAdd(item.Id, item);
            }
        }
    }

    public Task<IStoreItem> Add(IStoreItem item)
    {
        throw new NotImplementedException();
    }

    public Task<IStoreItem> Delete(StoreItemId id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IStoreItem> Get(StoreItemId id)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IStoreItem> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<IStoreItem> Update(StoreItemId item)
    {
        throw new NotImplementedException();
    }



}
