using CachingSample.Store.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CachingSample.Store.Caching;

public class CachedItemStore : IStore<StoreItem>
{
    private readonly IStore<StoreItem> _store;
    private readonly IDistributedCache _cache;

    public CachedItemStore(IStore<StoreItem> store, IDistributedCache cache)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async IAsyncEnumerable<StoreItem> GetAll()
    {
        var cacheId = $"{nameof(StoreItem)}s";
        var content = await _cache.GetStringAsync(cacheId).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(content))
        {
            foreach (var item in JsonSerializer.Deserialize<List<StoreItem>>(content)!)
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

    public async Task<StoreItem> Get(StoreItemId id)
    {
        var cacheId = $"{nameof(StoreItem)}:{id}";
        var content = await _cache.GetStringAsync(cacheId).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(content))
            return JsonSerializer.Deserialize<StoreItem>(content)!;

        var item = await _store.Get(id);
        await _cache.SetStringAsync(cacheId, JsonSerializer.Serialize(item)).ConfigureAwait(false);
        return item;
    }

    public async Task<StoreItem> Add(StoreItem item)
    {
        var result = await _store.Add(item).ConfigureAwait(false);
        await _cache.SetStringAsync($"{nameof(StoreItem)}:{result.Id}", JsonSerializer.Serialize(result)).ConfigureAwait(false);
        return result;
    }

    public async Task<StoreItem> Update(StoreItem item)
    {
        var updated = await _store.Update(item);
        await _cache.SetStringAsync($"{nameof(StoreItem)}:{item.Id}", JsonSerializer.Serialize(item)).ConfigureAwait(false);
        return updated;
    }

    public async Task<bool> Delete(StoreItemId id)
    {
        var result = await _store.Delete(id).ConfigureAwait(false);
        if (result)
            await _cache.RemoveAsync($"{nameof(StoreItem)}:{id}").ConfigureAwait(false);
        return result;
    }
}
