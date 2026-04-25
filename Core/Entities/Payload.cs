using Ping_Project.Core.ValueObjects;
namespace Ping_Project.Core.Entities;

public class Payload
{
    public int ID { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public required Log log { get; set; }
}