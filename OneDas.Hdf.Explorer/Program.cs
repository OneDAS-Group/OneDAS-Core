using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OneDas.Hdf.Explorer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.BaseDirectoryPath = @"M:\DATABASE";
            Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "EXPORT"));
            Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF Explorer"));

            Program.BuildWebHost(args).Run();
        }

        public static string BaseDirectoryPath { get; private set; }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:32769")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
    }
}
