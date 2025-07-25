﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using SSO.Core.Application.Library.Common.Exceptions;
using SSO.Core.Domain.Library.Exceptions;
using SSO.Infra.SQL.Library.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace SSO.EndPoint.WebApi.Middleware.ExceptionHandler;
public static class ApiExceptionMiddlewareExtensions
{
    public static void UseApiExceptionHandler(this IApplicationBuilder app)
    {
        ILoggerFactory loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var exception = contextFeature.Error;

                    var statusCode = exception switch
                    {
                        DomainLogicException or
                        DomainValueObjectException or
                        FluentValidation.ValidationException or
                        AppException
                        => HttpStatusCode.BadRequest,

                        DatabaseException 
                        => HttpStatusCode.InternalServerError,

                        UnauthorizedAccessException
                        => HttpStatusCode.Unauthorized,

                        KeyNotFoundException
                        => HttpStatusCode.NotFound,
                        _ => HttpStatusCode.InternalServerError
                    };

                    context.Response.StatusCode = (int)statusCode;

                    var errorResponse = new ApiErrorResponse
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = exception.getMessage(),
                        Errors = exception.getErrors(),
                    };

                    logger.LogError(exception, "Unhandled exception occurred");

                    var json = JsonSerializer.Serialize(errorResponse);
                    await context.Response.WriteAsync(json);
                }
            });
        });
    }

    private static IEnumerable<string> getErrors(this Exception exception)
    {
        var result =  exception is FluentValidation.ValidationException validationException
                            ? validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                            : null!;
        return result;
    }
    private static string getMessage(this Exception exception)
    {
        string message = exception switch
        {
            SecurityTokenExpiredException => "Token has expired",
            SecurityTokenInvalidSignatureException => "Invalid token signature",
            SecurityTokenNotYetValidException => "Token is not yet valid",
            DomainLogicException or
            DomainValueObjectException or
            FluentValidation.ValidationException or
            AppException or
            DatabaseException or
            UnauthorizedAccessException or
            KeyNotFoundException => exception.Message,
            _ => exception.Message
        };
        return message;
    }
}
