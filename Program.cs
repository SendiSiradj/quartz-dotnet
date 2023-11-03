using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;

namespace QuartzConsoleJob;

public class Program
{
    public static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        var quartzConfig = configuration.GetSection("Quartz").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var item in quartzConfig)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }

        NameValueCollection props = new NameValueCollection();
        foreach (var item in quartzConfig)
        {
            props.Add(item.Key, item.Value);
        }

        StdSchedulerFactory factory = new StdSchedulerFactory(props);

        IScheduler scheduler = await factory.GetScheduler();
        await scheduler.Start();

        MyApp myApp = new MyApp(scheduler);
        await myApp.RunAsync();


        Console.WriteLine("Press any key to shutdwon...");
        Console.ReadKey();

        // Shutdown the scheduler gracefully
        await scheduler.Shutdown();

    }
}