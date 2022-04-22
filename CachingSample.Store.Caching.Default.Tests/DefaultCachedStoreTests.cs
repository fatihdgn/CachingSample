using CachingSample.Store.Memory;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CachingSample.Store.Caching.Tests;

public class DefaultCachedStoreTests
{
    private readonly DefaultCachedStore<MemoryStoreItem> sut;
    private readonly MemoryStore itemsStore;
    private readonly MemoryDistributedCache memoryCache;
    public DefaultCachedStoreTests()
    {
        itemsStore = new MemoryStore();
        memoryCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        sut = new DefaultCachedStore<MemoryStoreItem>(itemsStore, memoryCache);
    }

    [Fact]
    public async Task GetAll_ShouldAddToCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        var items = await sut.GetAll().ToListAsync();
        var serializedItems = await memoryCache.GetAsync($"{nameof(MemoryStoreItem)}s");

        var result = JsonSerializer.Deserialize<List<MemoryStoreItem>>(serializedItems);

        result.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task GetAll_ShouldGetFromCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        var items = await sut.GetAll().ToListAsync();
        items = await sut.GetAll().ToListAsync(); // Triggering the same call to get from cache
        var serializedItems = await memoryCache.GetAsync($"{nameof(MemoryStoreItem)}s");

        var result = JsonSerializer.Deserialize<List<MemoryStoreItem>>(serializedItems);

        result.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task Get_ShouldGetFromCacheIfExistsInCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        var addedItem = await sut.Get(item.Id);
        var serializedItem = await memoryCache.GetAsync($"{nameof(MemoryStoreItem)}:{item.Id}");

        var result = JsonSerializer.Deserialize<MemoryStoreItem>(serializedItem);

        result.Should().BeEquivalentTo(addedItem);
    }

    [Fact]
    public async Task Get_ShouldGetFromStoreIfItDoesntExistsInCacheAndAddItToTheCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await itemsStore.Add(item);
        var addedItem = await sut.Get(item.Id);
        var serializedItem = await memoryCache.GetAsync($"{nameof(MemoryStoreItem)}:{item.Id}");

        var result = JsonSerializer.Deserialize<MemoryStoreItem>(serializedItem);

        result.Should().BeEquivalentTo(addedItem);
    }

    [Fact]
    public async Task Add_ShouldAddToCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        var serializedItem = await memoryCache.GetStringAsync($"{nameof(MemoryStoreItem)}:{item.Id}");

        var result = JsonSerializer.Deserialize<MemoryStoreItem>(serializedItem);

        result.Should().BeEquivalentTo(item);
    }

    [Fact]
    public async Task Update_ShouldUpdateCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        item = item with { Name = "Updated just now" };
        item = await sut.Update(item);


        var serializedItem = await memoryCache.GetStringAsync($"{nameof(MemoryStoreItem)}:{item.Id}");
        var result = JsonSerializer.Deserialize<MemoryStoreItem>(serializedItem);

        result.Should().BeEquivalentTo(item);
    }

    [Fact]
    public async Task Delete_ShouldRemoveCache()
    {
        var item = MemoryStoreItemGenerator.Generate();
        item = await sut.Add(item);
        item = item with { Name = "Updated just now" };
        await sut.Delete(item.Id);

        var result = await memoryCache.GetStringAsync($"{nameof(MemoryStoreItem)}:{item.Id}");

        result.Should().BeNull();
    }
}
