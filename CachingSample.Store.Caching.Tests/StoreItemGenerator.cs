using Bogus;
using CachingSample.Store.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace CachingSample.Store.Caching.Tests;

public static class StoreItemGenerator
{
    static Faker<StoreItem> storeItemFaker;
    static StoreItemGenerator()
    {
        storeItemFaker = new Faker<StoreItem>()
            .CustomInstantiator(f =>
                new StoreItem(
                    new StoreItemId(f.Random.Guid()),
                    f.Commerce.ProductName(),
                    f.Lorem.Paragraph(),
                    decimal.Parse(f.Commerce.Price(0, 100, 2))
                )
            );
    }
    public static StoreItem Generate() => storeItemFaker.Generate();
    public static IEnumerable<StoreItem> Generate(int count) => Enumerable.Range(0, count).Select(i => Generate());
}
