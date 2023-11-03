using Quartz;

namespace QuartzConsoleJob;
public class SimpleJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Job executed at: {DateTime.Now}");
        return Task.CompletedTask;
    }
}