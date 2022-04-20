using CachingSample.Store.Abstractions;
using System.Collections.Concurrent;

namespace CachingSample.Store.Memory;

public class MemoryStore<TItem> : IStore<TItem>
    where TItem : IStoreItem
{
    private readonly ConcurrentDictionary<StoreItemId, TItem> _items;
    public MemoryStore(ConcurrentDictionary<StoreItemId, TItem> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }
    public MemoryStore() : this(new ConcurrentDictionary<StoreItemId, TItem>()) { }

    public async IAsyncEnumerable<TItem> GetAll()
    {
        foreach (var item in _items)
        {
            await Task.Delay(10); // Simulate I/O
            yield return item.Value;
        }
    }

    public async Task<TItem> Get(StoreItemId id)
    {
        await Task.Delay(10); // Simulate I/O

        if (!_items.ContainsKey(id))
            throw new Exception("Item not found");

        return _items[id];
    }

    public async Task<TItem> Add(TItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await Task.Delay(10); // Simulate I/O

        return _items[item.Id] = item;
    }

    public async Task<TItem> Update(TItem item)
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
        await Task.Delay(10); // Simulate I/O

        return _items.TryRemove(id, out _);
    }
}

public class MemoryStore : MemoryStore<MemoryStoreItem>
{
    public MemoryStore()
    {
    }

    public MemoryStore(ConcurrentDictionary<StoreItemId, MemoryStoreItem> items) : base(items)
    {
    }
}