# FlyFlint

![FlyFlint](images/FlyFlint-128.png)

[![NuGet FlyFlint](https://img.shields.io/nuget/v/FlyFlint.svg?style=flat)](https://www.nuget.org/packages/FlyFlint)

[![Project Status: Concept – Minimal or no implementation has been done yet, or the repository is only intended to be a limited example, demo, or proof-of-concept.](https://www.repostatus.org/badges/latest/concept.svg)](https://www.repostatus.org/#concept)

## これは何？

コンパイル時に実行される、軽量のスタティックO/Rマッパービルダーです。

FlyFlintは、データへのアクセサコードをコンパイル時に生成することで、
実行時にリフレクションAPIを一切使いません。
つまりこれは、AOT環境と親和性があり、高速で、軽量なO/Rマッパーです。

使用する場合は、レコードの器としてのモデル型（しばしば、エンティティ型やエレメント型と呼ばれます）
を定義して、[FlyFlintのNuGetパッケージ](https://www.nuget.org/packages/FlyFlint)を導入するだけです。
追加の作業は一切必要ありません！

```csharp
using FlyFlint;
using FlyFlint.Utilities;

// モデル型は、クラス・構造体・フィールド・プロパティを
// 任意に組み合わせる事が出来ます...
private sealed class Model
{
    public int Id;
    public string? Name;    // FlyFlintはヌル許容型に対応しています
    public DateTime? Birth;
}

public async Task<Model[]> GetModelsFromDatabaseAsync()
{
    using var connection = new SQLiteConnection(
        "Data Source=:memory:");
    await connection.OpenAsync();

    // クエリを生成します
    var query = connection.Query<Model>(
        "SELECT * FROM target");

    // 非同期でクエリを実行して、結果を列挙します。
    // （内部では高速なprefetcherが動作します）
    return await query.
        ExecuteAsync(query).
        ToArrayAsync();
}
```

FlyFlintは、レコードデータを `Model` に格納するときに、
リフレクションAPIを一切使用しません。
`DbDataReader` から得られたレコードデータは、
コンパイル時に生成されたコードで、直接格納されます。

これは、`DbDataReader.GetInt32()`などを手動で呼び出すコードとほぼ同等のコードを、
自動的に挿入することで実現しています。

## 対応環境

### 実行環境

* .NET 6/5
* .NET Core 3.1/3.0/2.1/2.0
* .NET Standard 2.1/2.0
* .NET Framework 4.8/4.6.1/4.6/4.5/4.0/3.5
* 全ての、`DbConnection`を提供する、ADO.NETデータベースドライバ
  * SQL Server, Oracle, SQLite など...

#### 制限

* .NET Framework 4.6.1未満: 非同期列挙(`IAsyncEnumerable<T>`)には対応しません。
* .NET Framework 4.0: パッケージは `Microsoft.Bcl.Async` に依存します。
* .NET Framework 3.5: パッケージは `AsyncBridge` に依存します。

### 開発が可能な環境

新しいMSBuild環境(≒.NET Core世代)が必要です。少なくとも以下の環境で動作します:

* Visual Studio 2021/2019/2017
* Rider with .NET 6/5/.NET Core SDK

## 基本的な使い方

NuGetからパッケージをインストールします。

データベースアクセスは、度々SQLインジェクション攻撃の標的となります。
もちろん、FlyFlintはパラメータ化クエリに対応していますが、
C#ユーザーが安全かつ簡便にクエリを書くことが出来るように、
C#の `string interporation` に対応しています:

```csharp
    // このようなクエリパラメータ値があるとします:
    var id = 123;

    // これをクエリに含めるには、string interporationを使います:
    var query = connection.Query<Model>(
        $"SELECT * FROM target WHERE Id = {id}");
```

見ての通り、これは非常に自然に書けて、確認も容易です。
そして、このように書いても、実際には文字列として解釈されるのではなく、
パラメータ化クエリに展開されて、データベースに送信されます。

恐らく、あなたは既に似たようなO/Rマッパーである `Dapper` を使っているのでしょう。
FlyFlintは、 `Dapper` のようなパラメータ指定も可能です:

```csharp
    // Dapperライクなパラメータ指定
    var query = connection.Query<Model>(
        "SELECT * FROM target WHERE Id = @id").
        Parameter(new { id = 123 });
```

O/Rマッパーの内部実装に詳しい人なら、このコードは内部でリフレクションを使う筈だと思うかもしれません。
そして、もちろんFlyFlintは、このようなコードをコンパイル時に解析して、アクセサコードを生成するため、
リフレクションAPIは使用しません。

機能はまだあります! パラメータ化クエリの生成には、追加のコストがあります。
そこで、クエリを発行する前に `prepared query` を生成する方法もあります:

```csharp
    // prepared queryを生成します
    var prepared = Query.Prepare<Model>(
        () => $"SELECT * FROM target WHERE Id = {id}");

    // prepared queryを使います
    var query = connection.Query(prepared);
```

この `prepared query` は、クエリ式の評価を、実行時まで遅らせる事が出来ます。

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

