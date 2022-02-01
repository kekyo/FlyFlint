# FlyFlint

![FlyFlint](Images/FlyFlint.100.png)

[![NuGet FlyFlint](https://img.shields.io/nuget/v/FlyFlint.svg?style=flat)](https://www.nuget.org/packages/FlyFlint)

[![Project Status: Concept – Minimal or no implementation has been done yet, or the repository is only intended to be a limited example, demo, or proof-of-concept.](https://www.repostatus.org/badges/latest/concept.svg)](https://www.repostatus.org/#concept)

---

## What is this?

In short word: Lightweight static O/R mapping builder at compile time.

FlyFlint will generate data accessors infrastructure at that compile time
and there are not use any runtime reflection.
That means, It is an AOT friendly, faster and lightweight O/R mapper.

To use it, you just need to define a record type (often called entity, element or model type)
as a vessel for your records.
And then simply install [FlyFlint NuGet package](https://www.nuget.org/packages/FlyFlint).
No additional work is required at all!

```csharp
using FlyFlint;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;

// Requires using `ToArrayAsync` or install `System.Linq.Async`.
using FlyFlint.Collections;

public static class Program
{
    // You can write record type with
    // any class/struct/field/property combination...
    private sealed class Target
    {
        public int Id;
        public string? Name;    // Yes, FlyFlint covered nullable types.
        public DateTime? Birth;
    }

    public static async Task Main()
    {
        using var connection = new SQLiteConnection(
            "Data Source=:memory:");
        await connection.OpenAsync();

        // Build the query.
        var query = connection.Query<Target>(
            $"SELECT * FROM target");

        // Execute query and got enumerable results on asynchronously.
        // (And enabled fast prefetcher.)
        Target[] targets = await query.
            ExecuteAsync().
            ToArrayAsync();
    }
}
```

FlyFlint can store record data into record type instance **except using ANY reflection**.
The record data will be stored directly from `DbDataReader`
by compile-time generated code.

This is achieved by automatically inserting code that is almost equivalent
to manually calling `DbDataReader.GetInt32()` or like.

---

## Environment

### Target platforms

* .NET 6/5
* .NET Core 3.1/3.0/2.1/2.0
* .NET Standard 2.1/2.0
* .NET Framework 4.8/4.6.1/4.6/4.5/4.0/3.5
* ADO.NET database driver that provides all `DbConnection` type.
  * SQL Server, Oracle, SQLite and etc...

#### Limitation

* Lesser than .NET Framework 4.6.1: Could not use asynchronous iteration (`IAsyncEnumerable<T>`).
* .NET Framework 4.0: Package has dependency `Microsoft.Bcl.Async`.
* .NET Framework 3.5: Package has dependency `AsyncBridge`.

### Development environments

Maybe you have to develop with newer MSBuild infrastructure:

* Visual Studio 2021/2019/2017
* Rider with .NET 6/5/.NET Core SDK

---

## Basic usage

[Install package via NuGet](https://www.nuget.org/packages/FlyFlint).

We can make safer code using string interpolated query in FlyFlint:

```csharp
    // Query parameters on the variables:
    var id = 123;

    // Build the parameterized query with string interpolation syntax.
    var query = connection.Query<Target>(
        $"SELECT * FROM target WHERE Id = {id}");

    Target[] targets = await query.
        ExecuteAsync().
        ToArrayAsync();
```

It is naturally code, readable and writable. The FlyFlint will interpret
and construct parameterized query, it is **not RAW STRING**.

I understood maybe you already use major lightweight O/R mapper `Dapper`,
FlyFlint can receive `Dapper` like query code:

```csharp
    // Build the parameterized query likes Dapper:
    var query = connection.Query<Target>(
        "SELECT * FROM target WHERE Id = @id").
        Parameter(new { id = 123 });

    Target[] targets = await query.
        ExecuteAsync().
        ToArrayAsync();
```

Yes, in general, that case will be used reflection API.
But FlyFlint will generate getter code at compile-time.

And more, construction for any parameterized query is required additional cost.
We can build `prepared query` before using it:

```csharp
    // Build the prepared query:
    var prepared = Query.Prepare<Target>(
        () => $"SELECT * FROM target WHERE Id = {id}");

    // Use prepared query:
    var query = connection.Query(prepared);

    Target[] targets = await query.
        ExecuteAsync().
        ToArrayAsync();
```

This `prepared query` is delayed to examine query expression.
Since it does not depend on the database connection (`DbConnection`),
if you generate it in advance, you can use it as many times as you like.

## Database traits

TODO:

## Dynamic query (IL emitter)

TODO:

### Fallback usage

TODO:

## Deeper FlyFlint

TODO: injected type case

---

## License

Apache-v2.

