using CachingSample.Store.Abstractions;

namespace CachingSample.Store.Memory;

public record MemoryStoreItem(StoreItemId Id, string Name, string Description, decimal Price) : IStoreItem;
