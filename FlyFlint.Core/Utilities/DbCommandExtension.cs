////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

#if NET35 || NET40
using System.Data.Common;
using System.Threading.Tasks;

namespace FlyFlint.Utilities
{
    internal static class DbCommandExtension
    {
        public static Task<int> ExecuteNonQueryAsync(this DbCommand command) =>
            Task.Factory.StartNew(command.ExecuteNonQuery);
        public static Task<object?> ExecuteScalarAsync(this DbCommand command) =>
            Task.Factory.StartNew(command.ExecuteScalar);
    }
}
#endif
