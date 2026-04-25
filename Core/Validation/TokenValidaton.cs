namespace Ping_Project.Core.Validation;

public class TokenValidaton(IConfiguration _configuration)
{
    public bool IsValid(string token) => token == _configuration.GetValue<string>("ServerToken");
}