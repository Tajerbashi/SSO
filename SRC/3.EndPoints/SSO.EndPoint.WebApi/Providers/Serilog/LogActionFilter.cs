using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using SSO.SharedKernel.Utilities.Library.SerializerProvider;
using System.Text.Json;

namespace SSO.EndPoint.WebApi.Providers.Serilog;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;
    private readonly IJsonSerializer _jsonSerializer;
    public LogActionFilter(ILogger<LogActionFilter> logger, IJsonSerializer jsonSerializer)
    {
        _logger = logger;
        _jsonSerializer = jsonSerializer;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        var controller = context.RouteData.Values["controller"];
        var action = context.RouteData.Values["action"];
        var parameters = JsonSerializer.Serialize(context.ActionArguments);
        // Read request body (buffered copy)
        request.EnableBuffering();
        var bodyReader = new StreamReader(request.Body);
        string requestBody = bodyReader.ReadToEnd();
        request.Body.Position = 0;

        using (LogContext.PushProperty("Controller", controller))
        using (LogContext.PushProperty("Action", action))
        using (LogContext.PushProperty("RequestBody", requestBody))
        {
            _logger.LogInformation("Handled request {Method} {Path}", request.Method, request.Path);
        }
        _logger.LogInformation("Executing Controller={Controller}, Action={Action}, Parameters={Parameters}",
            controller, action, parameters);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null && context.Result != null)
        {
            object resultValue = null;

            if (context.Result is ObjectResult objectResult)
            {
                resultValue = objectResult.Value;
            }
            else if (context.Result is JsonResult jsonResult)
            {
                resultValue = jsonResult.Value;
            }
            else if (context.Result is ContentResult contentResult)
            {
                resultValue = contentResult.Content;
            }

            if (resultValue != null)
            {
                var responseData = _jsonSerializer.Serialize(resultValue);
                _logger.LogInformation("Executed Action Result: {Result}", responseData);
            }
            else
            {
                _logger.LogInformation("Executed Action with no serializable result.");
            }
        }
    }
}
