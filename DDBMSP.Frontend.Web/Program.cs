using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DDBMSP.Frontend.Web
{
    public static class Constants
    {
        public const string BlogUrl = "http://localhost:5000";
        public const string BlogTitle = "DDBMSP — Project";
        public const string BlogDescription = "description";
        public const string BlogCoverImage = null;
        public const string BlogLogo = null;

        public static string CurrentPage { get; set; }
    }
    
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}