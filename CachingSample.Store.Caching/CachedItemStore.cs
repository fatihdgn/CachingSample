using CachingSample.Store.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CachingSample.Store.Caching
{
    public class CachedItemStore : IStore<StoreItem>
    {
        private readonly IStore<StoreItem> _store;
        private readonly IDistributedCache _cache;

        public CachedItemStore(IStore<StoreItem> store, IDistributedCache cache)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<StoreItem> Add(StoreItem item)
        {
            var result = await _store.Add(item).ConfigureAwait(false);
            await _cache.SetStringAsync($"{nameof(StoreItem)}:{result.Id}", JsonSerializer.Serialize(result)).ConfigureAwait(false);
            return result;
        }

        public Task<bool> Delete(StoreItemId id)
        {
            return _store.Delete(id);
        }

        public ValueTask<StoreItem> Get(StoreItemId id)
        {
            return _store.Get(id);
        }

        public IAsyncEnumerable<StoreItem> GetAll()
        {
            return _store.GetAll();
        }

        public Task<StoreItem> Update(StoreItem item)
        {
            return _store.Update(item);
        }
    }
}