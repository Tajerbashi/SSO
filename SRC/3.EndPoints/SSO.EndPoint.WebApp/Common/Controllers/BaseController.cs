using Microsoft.AspNetCore.Mvc.Infrastructure;
using SSO.Core.Application.Library;
using SSO.EndPoint.WebApp.Extensions;

namespace SSO.EndPoint.WebApp.Common.Controllers;

[ApiController]
[Route("sso/[controller]")]
public abstract class BaseController : Controller
{

    protected ProviderServices ProviderServices => HttpContext.ApplicationContext();

    public override OkObjectResult Ok([ActionResultObjectValue] object? value)
    {
        return base.Ok(value);
        //return base.Ok(ApiResponseModel.Success(value).SetRefreshToken(ProviderServices.User.RefreshToken));
    }

    public override BadRequestObjectResult BadRequest([ActionResultObjectValue] object? error)
    {
        return base.BadRequest(error);
        //return base.BadRequest(ApiResponseModel.Faild(error).SetRefreshToken(ProviderServices.User.RefreshToken));
    }
}
