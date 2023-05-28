using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using IHW4.Controllers;
using System.Data.SQLite;
using System.Threading;

namespace IHW4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var processor = new OrdersProcessor();
            var thread = new Thread(processor.Process);
            thread.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static SQLiteConnection connection;
        static SQLiteCommand command;

   
    }
}
