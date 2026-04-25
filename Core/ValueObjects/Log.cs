namespace Ping_Project.Core.ValueObjects;

public class Log
{
    public string Value { get; }

    public Log(string log)
    {
        if (string.IsNullOrEmpty(log)) throw new ArgumentException("empty log");
        Value = log;
    }
    public static implicit operator Log(string value) => new(value);
    public static implicit operator string(Log log) => log.Value;

}