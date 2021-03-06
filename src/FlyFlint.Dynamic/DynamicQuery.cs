////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Dynamic;

namespace FlyFlint
{
    public static class DynamicQuery
    {
        private static readonly DynamicQueryExecutor executor = new();

        public static void Enable() =>
            DynamicQueryExecutorFacade.SetQueryExecutor(executor);
    }
}
