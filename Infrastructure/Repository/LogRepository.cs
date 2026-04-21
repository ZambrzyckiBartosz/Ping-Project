using Microsoft.EntityFrameworkCore;
using Ping_Project.Entities;
namespace Ping_Project.Infrastructure.Repository;
public class LogRepository(AppDbContext _context)
{
    public async Task SaveLogs(Payload payload)
    {
        _context.HashedLogs.Add(payload);
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> PrintAllLogs()
    {
        var result = await _context.HashedLogs.ToListAsync();
        var logs = new List<string>();

        foreach (var res in result)
        {
            logs.Add($"[{res.Date:yyyy-MM-dd HH:mm:ss}] log: {res.log.Value}");
        }

        return logs;
    }
}