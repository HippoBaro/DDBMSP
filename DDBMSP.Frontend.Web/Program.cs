using System;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;

namespace DDBMSP.Frontend.Web
{
    public static class Constants
    {
        public const string BlogUrl = "http://localhost:5000";
        public const string BlogTitle = "DDBMSP — Project";
        public const string BlogDescription = "description";
        public const string BlogCoverImage = "https://demo.ghost.io/content/images/2017/07/blog-cover.jpg";
        public const string BlogLogo = "https://demo.ghost.io/content/images/2014/09/Ghost-Transparent-for-DARK-BG.png";

        public static string CurrentPage { get; set; }
    }
    
    public static class Program
    {
        public static void Main(string[] args)
        {
            InitializeWithRetries(ClientConfiguration.LocalhostSilo(), 10);
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
    
    
}