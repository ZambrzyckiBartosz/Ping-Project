using System.Collections.Concurrent;

namespace Ping_Project.Core.State;

public class TaskQueueManager
{
    private ConcurrentQueue<string> tasks = new ConcurrentQueue<string>();
    
    public void AddTask(string task)
    {
        tasks.Enqueue(task);
    }

    public string? GetTask()
    {
        tasks.TryDequeue(out string? task);
        
        return string.IsNullOrWhiteSpace(task) ? null : task;
    }
}