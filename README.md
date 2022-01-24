# FlyFlint

## What is this?

Lightweight static O/R mapping builder at compile time.

FlyFlint will generate data accessors infrastructure at that compile time and there are not use any runtime reflection.
That means, It is an AOT friendly, faster and lightweight O/R mapper.

You could define only `model` (`entity`, `element` and like) type and use it.

```csharp
using FlyFlint;
using FlyFlint.Utilities;

// You can write models with
// any class/struct/field/property combination...
private sealed class Model
{
    public int Id;
    public string? Name;    // Yes, FlyFlint covered nullable types.
    public DateTime? Birth;
}

public async Task<Model[]> GetModelsFromDatabaseAsync()
{
    using var connection = new SQLiteConnection(
        "Data Source=:memory:");
    await connection.OpenAsync();

    // Build the query.
    var query = connection.Query<Model>(
        "SELECT * FROM target");

    // Execute query and got enumerable results on asynchronously.
    // (And enabled fast prefetcher.)
    return await query.ExecuteAsync(query).
        ToArrayAsync();
}
```

FlyFlint can store record fields into `Model` **except using ANY reflection**.
The record data will be stored directly from `DbDataReader`
by compile-time generated code.

## Environment

### Target platforms

* .NET 6/5
* .NET Core 3.1/3.0/2.1/2.0
* .NET Standard 2.1/2.0
* .NET Framework 4.8/4.6.1/4.6/4.5/4.0/3.5

#### Limitation

* Lesser than .NET Framework 4.6.1: Could not use asynchronous iteration (`IAsyncEnumerable<T>`).
* .NET Framework 4.0: Package has dependency `Microsoft.Bcl.Async`.
* .NET Framework 3.5: Package has dependency `AsyncBridge`.

### Development environments

Maybe you have to develop with newer MSBuild infrastructure:

* Visual Studio 2021/2019/2017
* Rider with .NET 6/5/.NET Core SDK

## Basic usage

Install package via NuGet.

We can make safer code using string interpolated query in FlyFlint:

```csharp
    // Query parameters on the variables:
    var id = 123;

    // Build the parameterized query.
    var query = connection.Query<Model>(
        $"SELECT * FROM target WHERE Id = {id}");
```

It is naturally code, readable and writable. The FlyFlint will interpret
and construct parameterized query, it is **not RAW STRING**.

I understood you already use major lightweight O/R mapper `Dapper`,
FlyFlint can receive `Dapper` like query code:

```csharp
    // Build the parameterized query likes Dapper:
    var query = connection.Query<Model>(
        "SELECT * FROM target WHERE Id = @id").
        Parameter(new { id = 123 });
```

Yes, in general, that case will be used reflection API.
But FlyFlint will generate getter code at compile-time.

And more, construction for any parameterized query is required additional cost.
We can build `prepared query` before using it:

```csharp
    // Build the prepared query:
    var prepared = Query.Prepare<Model>(
        () => $"SELECT * FROM target WHERE Id = {id}");

    // Use prepared query:
    var query = connection.Query(prepared);
```

This `prepared query` is delayed to examine query expression.

## Database traits

TODO:

## Dynamic query (IL emitter)

TODO:

### Fallback usage

TODO:

## Deeper FlyFlint

TODO: injected type case

## License

Apache-v2.

