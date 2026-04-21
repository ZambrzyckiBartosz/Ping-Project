using Ping_Project.ValueObjects;
namespace Ping_Project.Entities;

public class Payload
{
    public int ID { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public Log log { get; set; }
}