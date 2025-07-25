using SSO.Infra.SQL.Library.Common.Exceptions;
using System.Security.Claims;

namespace SSO.Infra.SQL.Library.Extensions;

public static class ClaimsExtentions
{
    public static List<Claim> ToClaims(this Dictionary<string, string> keyValues)
    {
        var claims = new List<Claim>();

        if (keyValues == null)
            return claims;

        foreach (var kvp in keyValues)
        {
            if (!string.IsNullOrEmpty(kvp.Key) && kvp.Value != null)
            {
                claims.Add(new Claim(kvp.Key, kvp.Value));
            }
        }

        return claims;
    }
}
public static class ExceptionExtentions
{
    public static DatabaseException ThrowException(this Exception exception) => new DatabaseException(exception);

    public static DatabaseException ThrowDatabaseException(this string message, params string[] parameters)
        => new DatabaseException(message, parameters);

    public static DatabaseException ThrowDatabaseException(this Exception exception) => new DatabaseException(exception);

}
