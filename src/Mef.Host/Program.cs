using System;
using System.CommandLine;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var loaderOption = new Option<Loader>(
                "--loader",
                getDefaultValue: () => Loader.Plugin,
                description: "An option to specify what assembly loader to use: isolated or plugin (default)"
            );

            var rootCommand = new RootCommand("vs-mef with custom AssemblyLoadContexts")
            {
                loaderOption
            };

            rootCommand.SetHandler(
                async (Loader loader) =>
                {
                    Resolver resolver = loader switch
                    {
                        Loader.Isolated => new IsolatedResolver(),
                        Loader.Plugin => new PluginResolver(),
                        _ => throw new NotImplementedException(),
                    };
                    Console.WriteLine($"Using {resolver.GetType().Name}");

                    await new App().Run(resolver);
                }, loaderOption);

            await rootCommand.InvokeAsync(args);
        }
    }

    enum Loader
    {
        Isolated,
        Plugin
    }
}
