using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("vs-mef with custom AssemblyLoadContexts")
            {
                new Option<Loader>("--loader")
            };
            rootCommand.Handler = CommandHandler.Create(
                async (Loader loader) =>
                {
                    Resolver resolver;
                    switch (loader)
                    {
                        case Loader.Isolated:
                            resolver = new IsolatedResolver();
                            break;
                        case Loader.Plugin:
                        default:
                            resolver = new PluginResolver();
                            break;
                    }
                    Console.WriteLine($"Using {resolver.GetType().Name}");

                    await new App().Run(resolver);
                });
            await rootCommand.InvokeAsync(args);
        }
    }

    enum Loader
    {
        Isolated,
        Plugin
    }
}
