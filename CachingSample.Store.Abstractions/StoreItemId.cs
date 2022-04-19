using ValueExtensions;

namespace CachingSample.Store.Abstractions;

public readonly record struct StoreItemId(Guid Value) : ValueOf<Guid, StoreItemId>.AsStruct;