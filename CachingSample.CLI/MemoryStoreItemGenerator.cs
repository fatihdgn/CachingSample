using Bogus;
using CachingSample.Store.Abstractions;
using CachingSample.Store.Memory;

public static class MemoryStoreItemGenerator
{
    static Faker<MemoryStoreItem> storeItemFaker;
    static MemoryStoreItemGenerator()
    {
        storeItemFaker = new Faker<MemoryStoreItem>()
            .CustomInstantiator(f =>
                new MemoryStoreItem(
                    new StoreItemId(f.Random.Guid()),
                    f.Commerce.ProductName(),
                    f.Lorem.Paragraph(),
                    decimal.Parse(f.Commerce.Price(0, 100, 2))
                )
            );
    }
    public static MemoryStoreItem Generate() => storeItemFaker.Generate();
    public static IEnumerable<MemoryStoreItem> Generate(int count) => Enumerable.Range(0, count).Select(i => Generate());
}
