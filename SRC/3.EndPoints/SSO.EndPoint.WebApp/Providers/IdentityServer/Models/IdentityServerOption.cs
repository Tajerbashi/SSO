namespace SSO.EndPoint.WebApp.Providers.IdentityServer.Models;

public class IdentityServerOption
{
    public List<IdentityUserConfig> Users { get; set; }
}

public class IdentityUserConfig
{
    public string Name { get; set; }
    public string ClientId { get; set; }
    public string Uri { get; set; }
    public string AllowedGrantTypes { get; set; }
}