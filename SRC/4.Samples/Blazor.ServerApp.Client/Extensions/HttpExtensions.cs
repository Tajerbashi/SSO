namespace Blazor.ServerApp.Client.Extensions;

public static class HttpExtensions
{
    public static string GetUrl(this string value) => value.Split("?")[0];
}
