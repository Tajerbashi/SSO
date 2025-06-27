using Microsoft.AspNetCore.Authorization;

namespace SSO.EndPoint.WebApi.Common.Controllers;

[Authorize]
public abstract class AuthController : BaseController
{
}
