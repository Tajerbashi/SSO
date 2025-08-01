//using SSO.Core.Application.Library.Models;
//using SSO.EndPoint.WebApi.Extensions;

//namespace SSO.EndPoint.WebApi.Controllers.Apis;

//public class AuthenticationController : BaseController
//{
//    private readonly ILogger<AuthController> _logger;
//    private readonly IIdentityService _identityService;

//    public AuthenticationController(ILogger<AuthController> logger, IIdentityService identityService)
//    {
//        _logger = logger;
//        _identityService = identityService;
//    }



//    [HttpPost("login")]
//    public async Task<IActionResult> LoginAs(LoginParameter parameter)
//    {
//        try
//        {
//            var result = await _identityService.TokenService.LoginAsync(parameter);
//            if (result.Succeeded)
//            {
//                var token = await _identityService.TokenService.GenerateTokenAsync(new()
//                {
//                    Username = parameter.Username,
//                });

//                return Ok(token);
//            }
//            return BadRequest(result);
           
//        }
//        catch (Exception ex)
//        {

//            throw ex.ThrowException();
//        }
//    }


//    [HttpGet("logout")]
//    public async Task<IActionResult> Logout(string authKey)
//    {
//        try
//        {
//            var result = await _identityService.TokenService.LogoutAsync(authKey);

//            return Ok(result);
//        }
//        catch (Exception ex)
//        {

//            throw ex.ThrowException();
//        }
//    }

//    [HttpPost("index")]
//    public async Task<IActionResult> Index(LoginSSOParameter parameter,CancellationToken cancellation = default)
//    {
//        await Task.Delay(1000,cancellation);
//        var response = new LoginSSOResult()
//        {
//            AccessToken = "",
//            ExpiresIn = 6000,
//            RefreshToken = "",
//            ReturnUrl = parameter.ReturnUrl,
//            Scope = "",
//            TokenType = "",
//        };
//        return Ok(response);
//    }
//}
