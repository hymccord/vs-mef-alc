using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Mef.Host
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            await new App().Run();
        }
    }

    public class App
    {
        public App()
        {
        }

        public async Task Run()
        {
        }
    }
}
