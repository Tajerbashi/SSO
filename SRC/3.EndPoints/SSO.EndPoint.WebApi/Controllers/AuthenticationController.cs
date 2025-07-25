using SSO.Core.Application.Library.Models;
using SSO.EndPoint.WebApi.Extensions;

namespace SSO.EndPoint.WebApi.Controllers;
public class AuthenticationController : BaseController
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IIdentityService _identityService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _logger.LogInformation("~> AuthenticationController Constructor !!!");
        _identityService = identityService;
    }



    [HttpPost("LoginAs")]
    public async Task<IActionResult> LoginAs(LoginParameter parameter)
    {
        try
        {
            var result = await _identityService.TokenService.LoginAsync(parameter);
            if (result.Succeeded)
            {
                var token = await _identityService.TokenService.GenerateTokenAsync(new()
                {
                    Username = parameter.Username,
                });

                return Ok(token);
            }
            return BadRequest(result);
           
        }
        catch (Exception ex)
        {

            throw ex.ThrowException();
        }
    }


    [HttpGet("Logout")]
    public async Task<IActionResult> Logout(string authKey)
    {
        try
        {
            var result = await _identityService.TokenService.LogoutAsync(authKey);

            return Ok(result);
        }
        catch (Exception ex)
        {

            throw ex.ThrowException();
        }
    }

}

