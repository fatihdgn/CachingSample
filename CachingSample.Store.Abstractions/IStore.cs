namespace CachingSample.Store.Abstractions;

public interface IStore
{
    IAsyncEnumerable<IStoreItem> GetAll();
    ValueTask<IStoreItem> Get(StoreItemId id);
    Task<IStoreItem> Add(IStoreItem item);
    Task<IStoreItem> Update(StoreItemId item);
    Task<IStoreItem> Delete(StoreItemId id);
}
