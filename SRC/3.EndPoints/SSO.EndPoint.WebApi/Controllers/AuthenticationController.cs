using SSO.Core.Application.Library.Models;
using SSO.EndPoint.WebApi.Extensions;

namespace SSO.EndPoint.WebApi.Controllers;
public class AuthController : BaseController
{
    private readonly ILogger<AuthController> _logger;
    private readonly IIdentityService _identityService;

    public AuthController(ILogger<AuthController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _logger.LogInformation("~> AuthController Constructor !!!");
        _identityService = identityService;
    }



    [HttpPost("Login")]
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

