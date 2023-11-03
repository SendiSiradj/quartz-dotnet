using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;

public class SimpleJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Job executed at: {DateTime.Now}");
        return Task.CompletedTask;
    }
}

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

        JobKey jobKey = new JobKey("notASmpleJob", "group1");

        //sebuah trigger itu spesifik ke job, jika trigger sudah terdaftar untuk suatu job, tidak bisa pakai trigger yang sama utk job lain

        if (!await scheduler.CheckExists(jobKey))
        {
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                .WithIdentity(jobKey)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("notASimpleTrigger", "group1")
                .StartAt(DateBuilder.FutureDate(2, IntervalUnit.Second))
                .WithSimpleSchedule(x => x
                                    .WithIntervalInSeconds(10)
                                    .RepeatForever())
                                    .Build();

            await scheduler.ScheduleJob(job, trigger);

        }

        Console.WriteLine("Press any key to shutdwon...");
        Console.ReadKey();

        // Shutdown the scheduler gracefully
        await scheduler.Shutdown();

    }
}