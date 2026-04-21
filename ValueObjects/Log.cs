using Microsoft.AspNetCore.Mvc.Formatters;

namespace Ping_Project.ValueObjects;

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