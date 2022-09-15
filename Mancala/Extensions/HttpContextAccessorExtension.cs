namespace Mancala.Extensions;

/// <summary>
/// Extension methods for HttpContextAccessor
/// </summary>
public static class HttpContextAccessorExtension
{
    /// <summary>
    /// Get current user ip address
    /// </summary>
    /// <returns></returns>
    public static string GetIp(this IHttpContextAccessor httpContextAccessor) 
        => httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
}