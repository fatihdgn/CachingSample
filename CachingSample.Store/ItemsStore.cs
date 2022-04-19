using CachingSample.Store.Abstractions;
using System.Collections.Concurrent;

namespace CachingSample.Store;

public class ItemsStore : IStore<StoreItem>
{
    private readonly ConcurrentDictionary<StoreItemId, StoreItem> _items;
    public ItemsStore(ConcurrentDictionary<StoreItemId, StoreItem> items)
    {
        _items = items;
    }
    
    public async IAsyncEnumerable<StoreItem> GetAll()
    {
        foreach (var item in _items)
        {
            await Task.Delay(10); // Simulate I/O
            yield return item.Value;
        }
    }

    public async ValueTask<StoreItem> Get(StoreItemId id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));

        await Task.Delay(10); // Simulate I/O

        if (!_items.ContainsKey(id))
            throw new Exception("Item not found");

        return _items[id];
    }

    public async Task<StoreItem> Add(StoreItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await Task.Delay(10); // Simulate I/O

        var newId = StoreItemId.From(Guid.NewGuid());
        return _items[newId] = item with { Id = newId };
    }
    
    public async Task<StoreItem> Update(StoreItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        await Task.Delay(10); // Simulate I/O

        if (!_items.ContainsKey(item.Id))
            throw new Exception("Item not found");

        _items[item.Id] = item;
        return item;
    }

    public async Task<bool> Delete(StoreItemId id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));

        await Task.Delay(10); // Simulate I/O

        return _items.TryRemove(id, out _);
    }
}
