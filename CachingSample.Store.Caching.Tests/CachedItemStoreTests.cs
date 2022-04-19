using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CachingSample.Store.Caching.Tests
{
    public class CachedItemStoreTests
    {
        private readonly CachedItemStore sut;
        private readonly ItemsStore itemsStore;
        private readonly MemoryDistributedCache memoryCache;
        public CachedItemStoreTests()
        {
            itemsStore = new ItemsStore();
            memoryCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            sut = new CachedItemStore(itemsStore, memoryCache);
        }

        [Fact]
        public async Task Add_ShouldAddToCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await sut.Add(item);
            var serializedItem = await memoryCache.GetStringAsync($"{nameof(StoreItem)}:{item.Id}");
            var result = JsonSerializer.Deserialize<StoreItem>(serializedItem);
            result.Should().BeEquivalentTo(item);
        }
    }
}