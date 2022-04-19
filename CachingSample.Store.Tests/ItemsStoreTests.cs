using CachingSample.Store.Abstractions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Collections.Concurrent;

namespace CachingSample.Store.Tests;

public class ItemsStoreTests
{
    private readonly ConcurrentDictionary<StoreItemId, StoreItem> _items;
    private readonly ItemsStore sut;
    public ItemsStoreTests()
    {
        _items = new ConcurrentDictionary<StoreItemId, StoreItem>();
        Populate(_items, 5);
        sut = new ItemsStore(_items);
    }

    private static void Populate(ConcurrentDictionary<StoreItemId, StoreItem> items, int count)
    {
        foreach (var item in StoreItemGenerator.Generate(count))
        {
            items.TryAdd(item.Id, item);
        }
    }

    [Fact]
    public async Task GetAll_ShouldntBeEmpty()
    {
        var result = await sut.GetAll().AnyAsync();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Get_ShouldReturnItem_WithValidId()
    {
        var (key, value) = _items.FirstOrDefault();
        var result = await sut.Get(key);
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task Get_ShouldThrowException_WithUndefinedId()
    {
        var act = async () => await sut.Get(new StoreItemId(Guid.NewGuid()));
        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task Add_ShouldThrowArgumentNullException_WithNullItem()
    {
#pragma warning disable CS8625 // For testing purposes
        var act = async () => await sut.Add(null);
#pragma warning restore CS8625
        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Add_ShouldAddTheItem_WithDifferentIdDefined()
    {
        var id = new StoreItemId(Guid.NewGuid());
        var item = await sut.Add(new StoreItem(id, "item", "description", 100));
        item.Should().NotBeNull();
        item.Id.Should().NotBe(id);
    }

    [Fact]
    public async Task Update_ShouldThrowArgumentNullException_WithNullItem()
    {
#pragma warning disable CS8625 // For testing purposes
        var act = async () => await sut.Update(null);
#pragma warning restore CS8625
        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Update_ShouldThrowException_WithDifferentIdDefined()
    {
        var newItem = StoreItemGenerator.Generate();
        var act = async () => await sut.Update(newItem);
        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task Update_ShouldUpdateItem_WithDefinedId()
    {
        var previousItem = _items.FirstOrDefault().Value;
        var updatedItem = previousItem with { Price = 10 };
        var result = await sut.Update(updatedItem);
        result.Should().NotBeEquivalentTo(previousItem);
        result.Should().BeEquivalentTo(updatedItem);
    }

    [Fact]
    public async Task Delete_ShouldReturnTrue_WithDefinedId()
    {
        var (key, _) = _items.FirstOrDefault();
        var result = await sut.Delete(key);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ShouldReturnTrue_WithNotDefinedId()
    {
        var id = new StoreItemId(Guid.NewGuid());
        var result = await sut.Delete(id);
        result.Should().BeFalse();
    }
}
