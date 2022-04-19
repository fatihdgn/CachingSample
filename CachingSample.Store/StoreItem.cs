using CachingSample.Store.Abstractions;

namespace CachingSample.Store;

public record StoreItem(StoreItemId Id, string Name, string Description, decimal Price) : IStoreItem;
