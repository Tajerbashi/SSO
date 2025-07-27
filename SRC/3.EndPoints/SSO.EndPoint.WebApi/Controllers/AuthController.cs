using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System.Web;

namespace SSO.EndPoint.WebApi.Controllers;

public class AuthController : BaseController
{
    private readonly IIdentityServerInteractionService _interaction;

    public AuthController(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    [HttpGet("login")]
    public IActionResult Login([FromQuery] string returnUrl)
    {
        // Create the proper login URL with parameters
        var loginUrl = $"/Account/Login?returnUrl={HttpUtility.UrlEncode(returnUrl ?? "/")}";

        return Ok(new { LoginUrl = loginUrl });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout([FromQuery] string logoutId)
    {
        var context = await _interaction.GetLogoutContextAsync(logoutId);
        return Ok(new
        {
            logoutUrl = context.PostLogoutRedirectUri,
            clientName = context.ClientName,
            signOutIFrameUrl = context.SignOutIFrameUrl
        });
    }
}