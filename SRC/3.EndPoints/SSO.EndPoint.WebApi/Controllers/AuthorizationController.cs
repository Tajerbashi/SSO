namespace SSO.EndPoint.WebApi.Controllers;

public class AuthorizationController : AuthController
{
    [HttpGet("IsAuthorized")]
    public IActionResult IsAuthorized()
    {
        return Ok(User.Identity.IsAuthenticated);
    }

    [HttpGet("Claims")]
    public IActionResult Claims()
    {
        return Ok(HttpContext.User.Claims.ToList());
    }

    [HttpGet("Profile")]
    public IActionResult Profile()
    {
        return Ok(ProviderServices.User);
    }
}

