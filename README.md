# CachingSample

A usage sample for IDistributedCache. 

Project includes
- Store definition that contains the basic CRUD operations. 
- In-memory store that adds a 3ms delay on all operations to simulate I/O interaction. You might think that 3ms delay is overkill, but I think it's not that far from reality and serves the purposes of this sample.
- Cached store that uses IDistributedCache as a cache and acts as an intermediate layer for store operations.

App uses in-memory distributed cache.

## Usage

Just run the application from console.

## Parameters
--cached - whether to use the cached version of the store (default: false)

--count \<int\> - the number of items to generate (default: 100)
s
--iteration \<int\> - the number of times to iterate get all, get and update operations (default: 5)

## Results

If you want to just look for the results, the test took about 217435ms for cached and 559562ms for the non-cached version on my machine. Specs are below.

CPU: i7-10875H

RAM: 32GB