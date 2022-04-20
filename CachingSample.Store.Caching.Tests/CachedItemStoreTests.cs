using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
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
        public async Task GetAll_ShouldAddToCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await sut.Add(item);
            var items = await sut.GetAll().ToListAsync();
            var serializedItems = await memoryCache.GetAsync($"{nameof(StoreItem)}s");
            
            var result = JsonSerializer.Deserialize<List<StoreItem>>(serializedItems);
            
            result.Should().BeEquivalentTo(items);
        }

        [Fact]
        public async Task Get_ShouldGetFromCacheIfExistsInCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await sut.Add(item);
            var addedItem = await sut.Get(item.Id);
            var serializedItem = await memoryCache.GetAsync($"{nameof(StoreItem)}:{item.Id}");

            var result = JsonSerializer.Deserialize<StoreItem>(serializedItem);

            result.Should().BeEquivalentTo(addedItem);
        }

        [Fact]
        public async Task Get_ShouldGetFromStoreIfItDoesntExistsInCacheAndAddItToTheCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await itemsStore.Add(item);
            var addedItem = await sut.Get(item.Id);
            var serializedItem = await memoryCache.GetAsync($"{nameof(StoreItem)}:{item.Id}");
            
            var result = JsonSerializer.Deserialize<StoreItem>(serializedItem);

            result.Should().BeEquivalentTo(addedItem);
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

        [Fact]
        public async Task Update_ShouldUpdateCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await sut.Add(item);
            item = item with { Name = "Updated just now" };
            item = await sut.Update(item);
            

            var serializedItem = await memoryCache.GetStringAsync($"{nameof(StoreItem)}:{item.Id}");
            var result = JsonSerializer.Deserialize<StoreItem>(serializedItem);
            
            result.Should().BeEquivalentTo(item);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCache()
        {
            var item = StoreItemGenerator.Generate();
            item = await sut.Add(item);
            item = item with { Name = "Updated just now" };
            await sut.Delete(item.Id);
            
            var result = await memoryCache.GetStringAsync($"{nameof(StoreItem)}:{item.Id}");

            result.Should().BeNull();
        }
    }
}