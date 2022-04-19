using Bogus;
using CachingSample.Store.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingSample.Store;

public static class StoreItemGenerator
{
    static Faker<StoreItem> storeItemFaker;
    static StoreItemGenerator()
    {
        storeItemFaker = new Faker<StoreItem>()
            .RuleFor(o => o.Id, f => StoreItemId.From(f.Random.Guid()))
            .RuleFor(o => o.Name, f => f.Commerce.ProductName())
            .RuleFor(o => o.Price, f => decimal.Parse(f.Commerce.Price(0, 100, 2)))
            .RuleFor(o => o.Description, f => f.Lorem.Paragraph());
    }
    public static StoreItem Generate() => storeItemFaker.Generate();
    public static IEnumerable<StoreItem> Generate(int count) => Enumerable.Range(0, count).Select(i => Generate());
}
