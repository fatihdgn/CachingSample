namespace CachingSample.Store.Abstractions;

public interface IStoreItem
{
    StoreItemId Id { get; }
    string Name { get; }
    string Description { get; }
    decimal Price { get; }
}
