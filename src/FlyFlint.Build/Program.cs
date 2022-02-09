////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Mono.Options;
using System;
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
                        Console.WriteLine($"FlyFlint.Build[{ThisAssembly.AssemblyVersion}]: {message}");
                        break;
                    case LogLevels.Trace when !isTrace:
                        break;
                    default:
                        Console.WriteLine($"FlyFlint.Build[{ThisAssembly.AssemblyVersion}]: {level.ToString().ToLowerInvariant()}: {message}");
                        break;
                }
            }

            try
            {
                var options = new OptionSet()
                {
                    { "t|trace", "Trace diagnosis message", _ => isTrace = true },
                };

                var extra = options.Parse(args);
                if (extra.Count < 1)
                {
                    Console.WriteLine($"FlyFlint.Build [{ThisAssembly.AssemblyVersion}]");
                    Console.WriteLine("  Lightweight static O/R mapping builder at compile time.");
                    Console.WriteLine("  Copyright (c) Kouji Matsui.");
                    Console.WriteLine("usage: ffb.exe [options] <referenceBasePaths> <assembly_path>");
                    options.WriteOptionDescriptions(Console.Out);
                }
                else
                {
                    var referencesBasePath = extra[0].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var targetAssemblyPath = extra[1];

                    Message(LogLevels.Trace, $"TargetAssemblyPath={targetAssemblyPath}");
                    Message(LogLevels.Trace, $"ReferencesBasePath=[{string.Join(",", referencesBasePath)}]");

                    var injector = new Injector(referencesBasePath, Message);

                    injector.Inject(targetAssemblyPath);
                }
            }
            catch (Exception ex)
            {
                if (isTrace)
                {
                    Message(LogLevels.Error, ex.ToString());
                }
                else
                {
                    Message(LogLevels.Error, $"{ex.GetType().Name}: {ex.Message}");
                }
                return Marshal.GetHRForException(ex);
            }

            return 0;
        }
    }
}
