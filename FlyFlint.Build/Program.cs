////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FlyFlint
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var isTrace = false;
            void Message(LogLevels level, string message)
            {
                switch (level)
                {
                    case LogLevels.Information:
                        Console.WriteLine($"FlyFlint.Build: {message}");
                        break;
                    case LogLevels.Trace when !isTrace:
                        break;
                    default:
                        Console.WriteLine($"FlyFlint.Build: {level.ToString().ToLowerInvariant()}: {message}");
                        break;
                }
            }

            try
            {
                var referencesBasePath = args[0].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var targetAssemblyPath = args[1];
                isTrace = args.ElementAtOrDefault(2) is { } arg2 && bool.TryParse(arg2, out var v) && v;

                var injector = new Injector(referencesBasePath, Message);

                if (injector.Inject(targetAssemblyPath))
                {
                    Message(
                        LogLevels.Information,
                        $"Replaced injected assembly: Assembly={Path.GetFileName(targetAssemblyPath)}");
                }
                else
                {
                    Message(LogLevels.Information,
                        $"Injection target isn't found: Assembly={Path.GetFileName(targetAssemblyPath)}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Message(LogLevels.Error, $"{ex.GetType().Name}: {ex.Message}");
                return Marshal.GetHRForException(ex);
            }
        }
    }
}
