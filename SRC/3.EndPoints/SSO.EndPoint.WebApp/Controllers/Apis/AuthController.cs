

namespace SSO.EndPoint.WebApp.Controllers.Apis;

public class AuthController : BaseController
{
    private readonly IIdentityServerInteractionService _interaction;

    public AuthController(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    [HttpGet("login")]
    public IActionResult Login(string clientId, string returnUrl, string state = "")
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