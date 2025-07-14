namespace Dev.Common;

public interface IWebHelper
{
    string GetCurrentIpAddress();
}
public class WebHelper : IWebHelper
{
    public string GetCurrentIpAddress()
    {
        //var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
        //return ip?.ToString() ?? "Unknown";
        return "Unknown";
    }
}
