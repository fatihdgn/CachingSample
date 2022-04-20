using CachingSample.Store.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CachingSample.Store.Caching;

public class CachedStore<TItem> : IStore<TItem>
    where TItem : IStoreItem
{
    private readonly IStore<TItem> _store;
    private readonly IDistributedCache _cache;
    private readonly Type _itemType;
    public CachedStore(IStore<TItem> store, IDistributedCache cache)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _itemType = typeof(TItem);
    }

    public async IAsyncEnumerable<TItem> GetAll()
    {
        var cacheId = $"{_itemType.Name}s";
        var content = await _cache.GetStringAsync(cacheId).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(content))
        {
            foreach (var item in JsonSerializer.Deserialize<List<TItem>>(content)!)
            {
                yield return item;
            }
            yield break;
        }

        var items = await _store.GetAll().ToListAsync();
        await _cache.SetStringAsync(cacheId, JsonSerializer.Serialize(items)).ConfigureAwait(false);
        foreach (var item in items)
        {
            yield return item;
        }
    }

    public async Task<TItem> Get(StoreItemId id)
    {
        var cacheId = $"{_itemType.Name}:{id}";
        var content = await _cache.GetStringAsync(cacheId).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(content))
            return JsonSerializer.Deserialize<TItem>(content)!;

        var item = await _store.Get(id);
        await _cache.SetStringAsync(cacheId, JsonSerializer.Serialize(item)).ConfigureAwait(false);
        return item;
    }

    public async Task<TItem> Add(TItem item)
    {
        var result = await _store.Add(item).ConfigureAwait(false);
        await _cache.SetStringAsync($"{_itemType.Name}:{result.Id}", JsonSerializer.Serialize(result)).ConfigureAwait(false);
        return result;
    }

    public async Task<TItem> Update(TItem item)
    {
        var result = await _store.Update(item);
        await _cache.SetStringAsync($"{_itemType.Name}:{result.Id}", JsonSerializer.Serialize(result)).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> Delete(StoreItemId id)
    {
        var result = await _store.Delete(id).ConfigureAwait(false);
        if (result)
            await _cache.RemoveAsync($"{_itemType.Name}:{id}").ConfigureAwait(false);
        return result;
    }
}
