using Cocona;
using CachingSample.Store.Caching.Extensions.DependencyInjection;
using CachingSample.Store.Extensions.DependencyInjection;
using CachingSample.Store.Abstractions;
using CachingSample.Store.Memory;
using CachingSample.Store.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddDistributedMemoryCache()
                .UseMemoryStore()
                .UseDefaultCaching();

var app = builder.Build();

app.AddCommand(async (int? count, int? iteration, bool cached) =>
{
    Console.WriteLine(cached ? "Using cached store" : "Using regular store");
    IStore<MemoryStoreItem> store = cached ? app.Services.GetService<ICachedStore<MemoryStoreItem>>()! : app.Services.GetService<IStore<MemoryStoreItem>>()!;
    Stopwatch sw = new Stopwatch();

    Console.WriteLine("Starting stopwatch...");
    sw.Start();

    count = count ?? 100;
    iteration = iteration ?? 5;

    {
        Console.WriteLine($"Adding {count} items...");

        foreach (var item in MemoryStoreItemGenerator.Generate(count.Value))
            await store.Add(item);
    }

    List<MemoryStoreItem>? items = null;
    {
        Console.WriteLine($"Getting all items {iteration} times.");
        for (int i = 0; i < iteration.Value; i++)
        {
            items = await store.GetAll().ToListAsync();
        }
    }

    {
        Console.WriteLine($"Getting each item {iteration} times.");
        foreach (var item in items!)
        {
            MemoryStoreItem fetchedItem;
            for (int i = 0; i < iteration; i++)
            {
                fetchedItem = await store.Get(item.Id);
            }
        }
    }

    {
        Console.WriteLine($"Update each item {iteration} times.");
        foreach (var item in items!)
        {
            MemoryStoreItem updatedItem = item;
            for (int i = 0; i < iteration; i++)
            {
                updatedItem = updatedItem with { Description = $"{i + 1}. update" };
                updatedItem = await store.Update(updatedItem);
            }
        }
    }

    {
        Console.WriteLine($"Delete each item.");
        foreach (var item in items!)
        {
            await store.Delete(item.Id);
        }
    }

    sw.Stop();
    Console.WriteLine("Stopped stopwatch...");

    Console.WriteLine($"Total runtime: {sw.ElapsedMilliseconds}ms");
});

await app.RunAsync();