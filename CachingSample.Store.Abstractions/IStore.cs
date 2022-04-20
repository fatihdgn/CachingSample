namespace CachingSample.Store.Abstractions;

public interface IStore<TItem>
    where TItem : IStoreItem
{
    IAsyncEnumerable<TItem> GetAll();
    // Maybe I can add NotFound
    Task<TItem> Get(StoreItemId id);
    Task<TItem> Add(TItem item);
    Task<TItem> Update(TItem item);
    // Maybe I can add NotFound
    Task<bool> Delete(StoreItemId id);
}
