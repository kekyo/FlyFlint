# FlyFlint

![FlyFlint](Images/FlyFlint.100.png)

FlyFlint - コンパイル時に実行される、軽量のスタティックO/Rマッパービルダー

[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)

## NuGet

|Package|NuGet|
|:--|:--|
|FlyFlint|[![NuGet FlyFlint](https://img.shields.io/nuget/v/FlyFlint.svg?style=flat)](https://www.nuget.org/packages/FlyFlint)|
|FlyFlint.Dynamic|[![NuGet FlyFlint.Dynamic](https://img.shields.io/nuget/v/FlyFlint.Dynamic.svg?style=flat)](https://www.nuget.org/packages/FlyFlint.Dynamic)|

## CI

|main|develop|
|:--|:--|
|[![FlyFlint CI build (main)](https://github.com/kekyo/FlyFlint/workflows/.NET/badge.svg?branch=main)](https://github.com/kekyo/FlyFlint/actions?query=branch%3Amain)|[![FlyFlint CI build (develop)](https://github.com/kekyo/FlyFlint/workflows/.NET/badge.svg?branch=develop)](https://github.com/kekyo/FlyFlint/actions?query=branch%3Adevelop)|

---

[English is here](https://github.com/kekyo/FlyFlint)

## これは何？

コンパイル時に実行される、軽量のスタティックO/Rマッパービルダーです。

FlyFlintは、データへのアクセサコードをコンパイル時に生成することで、
実行時にリフレクションAPIを一切使いません。
つまりこれは、AOT環境と親和性があり、高速で、軽量なO/Rマッパーです。

使用する場合は、結果レコードの器となるレコード型を定義して、
[FlyFlintのNuGetパッケージ](https://www.nuget.org/packages/FlyFlint)を導入するだけです。
追加の作業は必要ありません！

```csharp
// レコード型は、クラス・構造体・フィールド・プロパティを
// 任意に組み合わせる事が出来ます...
private sealed class Target
{
    public int Id;
    public string? Name;    // FlyFlintはヌル許容型に対応しています
    public DateTime? Birth;
}

public static async Task Main()
{
    using var connection = new SQLiteConnection(
        "Data Source=:memory:");
    await connection.OpenAsync();

    // クエリを生成します
    var query = connection.Query<Target>(
        $"SELECT * FROM target");

    // 非同期でクエリを実行して、結果を列挙します。
    // （内部では高速なprefetcherが動作します）
    Target[] targets = await query.
        ExecuteAsync().
        ToArrayAsync();
}
```

FlyFlintは、レコードデータをインスタンスに格納するときに、
リフレクションAPIを一切使用しません。
`DbDataReader` から得られたレコードデータは、
コンパイル時に生成されたコードで、直接格納されます。

これは、`DbDataReader.GetInt32()`などを手動で呼び出すコードとほぼ同等のコードを、
自動的に挿入することで実現しています。

---

## 機能

注意: まだ作業中のため、全ての機能は網羅していません。

* 完全にコンパイル時に挿入されるアクセサコード（getter/setter）
* NuGetパッケージをインストールするだけで動作
* リフレクションAPIを一切使用しない
* シンプルで高速なアーキテクチャ
* タイプセーフ性のあるクエリビルダインターフェイス
* DBNullからの解放とNullable型のサポート
* string interpolation構文でパラメタライズドクエリを記述可能
* 全てのADO.NETドライバに対応
* 必要であれば、未挿入の型に対してリフレクションによる動的クエリを使用可能
* カスタム型変換が可能
* F#フレンドリなAPIにも対応

---

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

---

## O/Rマッパーとは

FlyFlintの説明の前に、O/Rマッパーについて少し説明します
（既にO/Rマッパーについて理解がある場合は、飛ばしても構いません）

O/Rマッパーは、データベースアクセスを補助するライブラリです。
データベースアクセスを行う場合は、以下のような頻出する課題があります:

* データベースにクエリ文を発行するための一連の準備:
  * `DbConnection` クラスを生成して、接続を確立する。
  * `DbCommand` クラスを生成して、クエリ文を指定する。
  * `DbParameter` クラスを生成して、クエリにパラメータ群を指定する。
* 上記で構築したクエリを実行する。
* 結果レコードを変換する:
  * 結果レコード群は、表構造になっているので、その行と列の値を取り出す必要がある。
  * 列のデータは、.NETで必要としている型と一対一ではない場合があるため、そのような場合は変換する必要がある。

このような処理は、大体同じような実装になりがちなので、
一から手動で実装していると、大変な手間が発生します。
また、似て異なる実装になるため、保守時に苦労することになります。

そこで、O/Rマッパーと呼ばれるライブラリが考え出されました。

O/Rマッパーには様々な実装が存在しますが、しばしば次のような住み分けがなされています:

* 重量級O/Rマッパー:
  * ホストの言語構文(C#やF#)でクエリ文を書かせ、再解釈して、SQL文に変換する。
  * 連結構造化データをオブジェクト型に完全に変換する。
  * データベースの世界を、可能な限りホスト言語の世界観で抽象化する。
* 軽量O/Rマッパー:
  * クエリ文はSQL文をほぼそのまま書かせる。
  * クエリ文の構築をサポートするユーティリティ。
  * レコードデータをオブジェクト型に（ほぼ一対一で）変換する。

重量級O/Rマッパーが目指す世界は、ホスト言語処理系から見ると、高度に抽象化・統合化されていて、
うまく行けば非常に扱いやすくなります。

クエリ文は、ホスト言語でそのまま記述することが出来て、型検査も行われるため、
コンパイル時にクエリ文の単純な誤りを検出することが可能です。
ホスト言語構文とSQL言語を行ったり来たりする必要が無いので、
多くの言語を扱う事にストレスを感じるのであれば、これは大変魅力的な選択肢でしょう
（C#で言うなら、LINQのクエリ構文で書けば、それがそのままデータベースで実行される、と想像出来ます）

一方、デメリットも存在し、ホスト言語の表現力とデータベースの機能との最大公約数的になりがちです。
一例を挙げると、データベースにはクエリヒントという機能が備わっていることが多いのですが、
使用するデータベースに固有の機能であることが多く、O/Rマッパーではサポートしていないない可能性があります
（使用するデータベースを限定して、サポートできるようにした実装も存在します）

また、データベースの表を結合する「リレーショナル」な特性と、
.NETの世界である「オブジェクト」との相互関係を自動的、あるいは半自動的に解決する事も出来ますが、
そもそも両者は異なる概念のため、この問題が解決されても、別の新たな問題を抱えることになります。

リレーションをコレクションで表現できるようにした実装が存在しますが、
コレクションへのアクセスはいつ解決（レコードの抽出）されるのか、
その時の物理トランザクションは維持されるのかされないのか、範囲はどうなのか、
コレクションの要素を更新した場合はどうなるのか、など、
データベースを詳しく知っている人であれば、却って分かりにくい印象を持つかも知れません。

軽量O/Rマッパーは、このような過度な抽象化を避けて、
実装が煩雑になる部分だけを補助することに専念しています。

そして、この分類分けで言えば、FlyFlintは、軽量O/Rマッパーに相当します。

---

## 基本的な使い方

FlyFlintは、軽量O/Rマッパーなので、クエリ文(SQL文)は、
再解釈されずにそのままデータベースサービスで実行され、
結果がレコード型（しばしば、エンティティ型、エレメント型、モデル型などと呼ばれます）に格納されて返されます。

最初に、[NuGetからパッケージをインストール](https://www.nuget.org/packages/FlyFlint)します。

以下に、SQLiteを使用した、最も簡単な例を示します:

* 想定環境: .NET 6/5, .NET Core 3, .NET Framework 4.6.1以上
* SQLiteは例に挙げているだけで、SQL ServerやMySQLなどでも同様です。

```csharp
using FlyFlint;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;

// レコード型の定義
public struct Target
{
    public int Id;
    public string? Name;
    public DateTime? Birth;
}

public static class Program
{
    public static async Task Main()
    {
        // データベースに接続
        using var connection = new SQLiteConnection("...");
        await connection.OpenAsync();

        // 1. レコード型(ここではTarget型)を返すクエリ文を定義して:
        var query = connection.Query<Target>(
            $"SELECT * FROM target");

        // 2. 実行し、結果がレコード型に格納されて返される
        await foreach (var target in query.ExecuteAsync())
        {
            Console.WriteLine($"Id={target.Id}, Name={target.Name}");
        }
    }
}
```

クエリ文の実行には2段階のステップがあります:

1. データベース接続 `DbConnection` に対して、`Query` メソッドを呼び出して
クエリ文を定義します。
2. 定義済みのクエリ文を実行します。

クエリ文を定義する `Query` メソッドには、レコード型を指定するメソッドと指定しないメソッドがあります:

* `Query(...)`: クエリの結果がレコードを返さない場合。
* `Query<TRecord>(...)`: クエリの結果がレコードを返す場合。ジェネリック引数にレコード型を指定する。

一度クエリ文を定義すれば、何度でも呼び出す事が出来ます（但し、同じデータベース接続を使います）。

クエリ文の実行は、以下のバリエーションがあります。
これらはADO.NETのメソッド名慣例に習っています:

* `ExecuteNonQueryAsync()`: クエリを実行し、反映行数のみ返される。
* `ExecuteScalarAsync()`: 単一の値が返されるクエリを実行する。
* `ExecuteAsync()`: 複数レコードが返されるクエリを実行し、結果を列挙する。
  * レコードの型は、クエリ文の定義で与える（上記例を参照）。

最後の `ExecuteAsync` メソッドは、 .NET Core 3で対応した `IAsyncEnumerable<TRecord>`
を返します。このインターフェイスは、非同期列挙を行うことが出来ます。

上記の例では `await foreach` 文を使って非同期列挙を行いましたが、
組み込みで、いくつかのLINQ相当の演算子 (`Select`, `Where`, `ToArrayAsync` など) を
持っているため、これを使う事も出来ます:

```csharp
// FlyFlint組み込みのLINQ演算子を使う
using FlyFlint.Collections;

// ...

// LINQで絞り込んで配列に格納する
Target[] targets = await query.
    ExecuteAsync().
    Where(target => target.Id < 100).
    ToArrayAsync();
```

このLINQ演算は、データベースエンジンで実行されるのではなく、結果が得られた後にローカルで実行することに注意してください。
例えば、何百万件ものレコードを絞り込む場合は、普通にクエリ文内で `WHERE` 句を使います。

より高度なLINQ演算を行いたい場合は、Reactive Extensionsの公式の実装である、
[System.Linq.Asyncパッケージ](https://www.nuget.org/packages/System.Linq.Async)を使用すると良いでしょう。

---

## 安全で書きやすいパラメータ化クエリ

クエリ文は、度々SQLインジェクション攻撃の標的となります。
FlyFlintは、いわゆるパラメータ化クエリに対応していますが、
安全かつ簡便にクエリを書くことが出来るように、
C#やF#の `string interporation` 構文に対応しています:

```csharp
// このようなクエリパラメータ値があるとします:
var id = 123;

// これをクエリに含めるには、string interporation構文を使います:
var query = connection.Query<Target>(
    $"SELECT * FROM target WHERE Id={id}");
```

見ての通り、これは非常に自然に書けて、確認も容易です。
そして、このように書いても、実際には文字列として解釈されるのではなく、
パラメータ化クエリに展開されて、データベースに送信されます。

仮に、普通の文字列としてクエリ文を与えた場合は:

```csharp
var id = 123;

// string interporation構文を使っていない:
var query = connection.Query<Target>(
    "SELECT * FROM target WHERE Id=" + id);

// コンパイルできない
Target[] targets = await query.
    ExecuteAsync().    // `ExecuteAsyncは定義されていません`
    ToArrayAsync();

// 正しくは:
Target[] targets = await query.
    ExecuteNonParameterizedAsync().   // `NonParameterized`バージョンを使う
    ToArrayAsync();
```

このように、明示的に `NonParameterized` と命名されたメソッドを使用する必要があります。
この制約によって、誤って文字列でクエリを定義してしまうことを防止したり、コードレビューでの確認を容易にします。

---

## Dapperライクなパラメータ化クエリ

恐らく、あなたは既に似たような軽量O/Rマッパーである Dapper を知っているか、使った事があるでしょう。
FlyFlintは、Dapperのようなパラメータ指定も可能です:

```csharp
// Dapperライクなパラメータ指定
var query = connection.Query<Target>(
    "SELECT * FROM target WHERE Id=@id").  // 文字列として記述
    Parameter(new { id = 123 });           // Parameterメソッドでパラメータを付与

// 実行
Target[] targets = await query.
    ExecuteAsync().
    ToArrayAsync();
```

Dapperライクなパラメータ化クエリを使う場合は、クエリ文本体は `string interpolation`
構文を「使わずに」記述してください。
その後、`Parameter`メソッドでパラメータ値を与えます。

O/Rマッパーの内部実装に詳しい人なら、このコードは内部でリフレクションを使う筈だと思うかもしれません。
もちろんFlyFlintは、このようなコードをコンパイル時に解析してアクセサコードを生成するため、
リフレクションAPIは使用しません。

---

## クエリの事前定義

パラメータ化クエリの生成には、追加のコストがあります。
そこで、クエリを発行する前に、事前に定義しておく方法もあります:

```csharp
var id = 123;

// 事前定義クエリを生成します
var prepared = Query.Prepare<Target>(
    $"SELECT * FROM target WHERE Id={id}");

// 事前定義クエリを使います
var query = connection.Query(prepared);

Target[] targets = await query.
    ExecuteAsync().
    ToArrayAsync();
```

事前定義クエリは、事前に定義しておくと言うだけで、
通常の `Query` メソッドを使った定義と変わりません。

但し、定義しただけでは `DbConnection` と紐づいていないため、
フィールドに保存して使いまわす事が出来ます。

上記の例では、事前定義した時点のパラメータ (id) を保持しますが、
このパラメータ評価自体を遅延させることも出来ます:

```csharp
var id = 123;

// 事前定義遅延クエリを生成します
// この時点ではまだ id は保持されません
var prepared = Query.Prepare<Target>(
    () => $"SELECT * FROM target WHERE Id={id}");

id = 456;

// 事前定義を使います
// この時点の id が保持されます（この場合は456）
var query = connection.Query(prepared);

Target[] targets = await query.
    ExecuteAsync().
    ToArrayAsync();
```

パラメータ値だけ変化する状況での、クエリの使い回しが可能です。
但し、事前評価することによるコスト低減の効果は、前者の方が大きくなります。

---

## パラメータ型とレコード型定義の細かい調整

レコード型のフィールドは、別名を定義したり、対象を手動で調整する方法があります:

```csharp
public class User
{
    // 別名を適用する
    [QueryField("id")]
    public int Identity;

    // プライベートフィールドを対象とする
    [QueryField]
    private string? Name;

    // パブリックフィールドを除外する
    [QueryIgnore]
    public DateTime? Birth;
}
```

通常、パブリック定義された.NETのフィールドとプロパティが、自動的に対象となりますが、
`QueryField` や `QueryIgnore` 属性を適用することで、
対象を手動で調整する事が出来ます。

パラメータ型はDapperライクな使用方法を想定しているため、通常は匿名型で暗黙に定義されますが、
手動で定義することもできます:

```csharp
// パラメータを匿名型で定義
var query1 = connection.Query<Target>(
    "SELECT * FROM target WHERE Id=@id").
    Parameter(new { id = 123 });      // 匿名型

// パラメータを明示的に定義した型で定義
public struct IdParameter
{
    public int id;
}

var query2 = connection.Query<Target>(
    "SELECT * FROM target WHERE Id=@id").
    Parameter(new IdParameter { id = 123 });    // IdParameter型
```

このように、匿名型でも明示的に指定する型でも、どちらでも問題ありません。
また、明示的に指定する場合は、レコード型同様に属性を使用して、
対象のフィールドを細かく調整することも可能です。

---

## パラメータ型とレコード型の継承による拡張

パラメータ型とレコード型は、クラス型を使用することで、継承を利用して拡張する事が出来ます:

```csharp
// 基底クラス
public class Base
{
    public int Id;
}

// 派生クラス
public class User : Base
{
    public string? Name;
    public DateTime? Birth;
}
public class Item : Base
{
    public string? Name;
    public int Amount;
}
```

上記の定義を使うと、`User`型の場合はテーブルに `Id`, `Name`, `Birth` が存在する事が想定され、
`Item`型の場合は `Id`, `Name`, `Amount` が存在することが想定されます。

---

## パラメータ型とレコード型を分割して定義する

FlyFlintは、 `Query` メソッドの呼び出しを解析して、自動的にパラメータ型とレコード型を認識します

しかし、例えばレコード型の定義が別のアセンブリで行われている場合は、コードの操作が出来ないため、
少し工夫が必要です。

例えば、以下のようなシナリオを考えます:

* レコード型 `User` は、別のプロジェクト `Models.csproj` に定義されている。
* `User` を使ってクエリを発行するプロジェクトは、 `Accessor.csproj` で実装されている。

この場合、FlyFlintが自動的に処理できるのは、`Accessor.csproj` が生成するアセンブリのみで、
`Models.csproj`は自動的には処理しません。

このような場合は、以下のどちらかの手法を使う事が出来ます:

1. `Models.csproj` の `User` 型定義に、対象の属性を適用する。
2. 動的クエリを有効化する（後述）

ここでは、1について説明します。

`Models.csproj` に、FlyFlintのパッケージをインストールして、
`User` 型に対して、以下のように `QueryRecord` 属性を適用します:

`Models.csproj`:

```csharp
// レコードとなる型に属性を適用
[QueryRecord]
public class User
{
    public int Id;
    public string? Name;
    public DateTime? Birth;
}
```

`Accessor.csproj`:

```csharp
// User型を使用可能
var query = connection.Query<User>(
    $"SELECT * FROM users");
```

これで、 `User` 型がレコード型として使用出来るように認識されます。

パラメータ型についても同様の制約がありますが、パラメータ型に適用する場合は、
`QueryParameter` 属性を使用してください。

---

## Database特性の定義

`Database`クラスには、いくつかのデータベース特性の定義があり、
これを使用して、データベース固有の処理を行う事が出来ます。
この定義を `Trait` と呼びます:

```csharp
public static class Database
{
    // デフォルトの定義
    public static readonly Trait Default;

    // SQL Serverの定義
    public static readonly Trait SQLServer;
    // ORACLEの定義
    public static readonly Trait Oracle;
    // SQLiteの定義
    public static readonly Trait SQLite;
    // MySQLの定義
    public static readonly Trait MySQL;
    // Postgresqlの定義
    public static readonly Trait Postgresql;
}
```

デフォルトの定義は、ORACLE以外のデータベースで全て共通です。
実際には、ほとんどここで定義されているものをそのまま使う事で間に合うでしょう。
以下のようにして、デフォルトの `Query` メソッドの特性を変更する事が出来ます:

```csharp
// デフォルトの特性をORACLEに変更
Query.DefaultTrait = Database.Oracle;

// ORACLE向けのクエリが発行される
var query = connection.Query<Target>(
    $"SELECT * FROM dbo.persons WHERE id={id}");
```

但し、`DefaultTrait` の変更は、グローバルに反映されるので注意してください。
同じアプリケーション内で同時に異なるデータベースを操作する場合は、
`DbConnection.Query` メソッドを使わずに、次のように `Trait` から操作するようにします:

```csharp
// ORACLE traitからクエリを生成:
var oraQuery = Database.Oracle.Query<Target>(
    oraConnection,
    $"SELECT * FROM [persons] WHERE id={id}");

// SQL Server traitからクエリを生成（両立できる）:
var sqlQuery = Database.SQLServer.Query<Target>(
    sqlConnection,
    $"SELECT * FROM [persons] WHERE id={id}");
```

クエリの事前定義も、 `Trait` から `Prepare` メソッドを呼び出して定義する事が出来ます。

自分で `Trait` を定義することも出来ます。

* ほぼ存在しないと思いますが、デフォルトの `Trait` に合わない独自のデータベースで使う場合。
* カスタム型変換を使いたい場合（後述）

```csharp
// 独自Traitの定義
var customTrait = Database.CreateTrait(
    ConversionContext.Default,          // カスタム型変換の方法
    StringComparer.OrdinalIgnoreCase,   // フィールド名一致方法
    "@");                               // パラメータ化クエリのプレフィックス

// Traitを明示的に使用
var customQuery = customTrait.Query<Target>(
    customConnection,
    $"SELECT * FROM [persons] WHERE id={id}");
```

---

## 動的クエリ

FlyFlintは完全スタティック動作が特徴ですが、動的クエリ（リフレクションを使用）を必要とする場合もあります:

* レコード型やパラメータ型が、コンパイル時コード生成を行えない場合:
  * 別のアセンブリに定義されている既存の型を流用していて、しかもその型を含むアセンブリを同時にビルド出来ない。
  * .NETの標準的な型の流用や、NuGetパッケージに定義されている型を流用する場合。
  * 発生するメッセージ:
    * コンパイル時警告: `Could not inject parameter type, because it is declared in another assembly`
    * 実行時例外: `InvalidOperationException("Dynamic query feature is not enabled")`
* 複雑な理由で、コンパイル時コード生成を使用したくない場合。

TODO: public memberに対してアクセスする静的コードの生成

TODO: 生成出来ない場合の無視・警告・エラーの選択

そのような場合は、[`FlyFlint.Dynamic` NuGetパッケージ](https://www.nuget.org/packages/FlyFlint.Dynamic) をインストールして、クエリ実行前に、以下のメソッドを呼び出します:

```csharp
// 動的クエリを有効にする（何度呼び出しても良い）
DynamicQuery.Enable();
```

動的クエリはリフレクションAPIを使いますが、IL emitting（動的コード生成）を行っているため、素のリフレクションよりは高速に動作します。この機能は強力なため、あらゆる型に対してアクセスが可能になりますが、AOT環境を要求する.NETランタイムでは使用出来ない可能性があることに注意してください。

---

### フォールバック動作

TODO: 静的クエリに失敗した場合の動的クエリ

フォールバックが発生するより技術的な詳細については、Deeper FlyFlintの項を参照して下さい。

---

### カスタム型変換

TODO: 実装方法

TODO: デフォルトのサポート型

TODO: Nullable reference typeの扱い

---

## 同期インターフェイス

理由があって、非同期ではなく同期的な操作を行いたい場合、又は.NET Framework 4.6.1未満の環境で使用する場合は、
`FlyFlint.Synchronized` 名前空間の `ExecuteNonQuery`, `ExecuteScalar`, `Execute` を使う事が出来ます。

* FlyFlintでは、同期インターフェイスはサポートしますが、使用は推奨しません。
* 名前空間が分割されているため、レビュー時に容易に見分ける事が出来ます。

---

### F#固有の情報

TODO:

---

## Deeper FlyFlint

TODO: injected type case

---

## License

Apache-v2.
