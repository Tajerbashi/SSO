using Microsoft.AspNetCore.Mvc.Infrastructure;
using SSO.Core.Application.Library;
using SSO.EndPoint.WebApi.Extensions;

namespace SSO.EndPoint.WebApi.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{

    protected ProviderServices ProviderServices => HttpContext.ApplicationContext();

    public override OkObjectResult Ok([ActionResultObjectValue] object? value)
    {
        return base.Ok(ApiResponseModel.Success(value).SetRefreshToken(ProviderServices.User.RefreshToken));
    }

    public override BadRequestObjectResult BadRequest([ActionResultObjectValue] object? error)
    {
        return base.BadRequest(ApiResponseModel.Faild(error).SetRefreshToken(ProviderServices.User.RefreshToken));
    }
}
