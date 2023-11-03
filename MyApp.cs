
using Quartz;

namespace QuartzConsoleJob;

    public class MyApp
{
    private readonly IScheduler _scheduler;

    public MyApp(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task RunAsync()
    {
        JobKey jobKey = new JobKey("notASimpleJob2", "group1");

        if (!await _scheduler.CheckExists(jobKey))
        {
            IJobDetail job = JobBuilder.Create<SimpleJob>()
            .WithIdentity(jobKey)
            .Build();

            //sebuah trigger itu spesifik ke job, jika trigger sudah terdaftar untuk suatu job, tidak bisa pakai trigger yang sama utk job lain


            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("notASimpleTrigger2", "group1")
            .StartAt(DateBuilder.FutureDate(2, IntervalUnit.Second))
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10)
                .RepeatForever())
            .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
    }
}

