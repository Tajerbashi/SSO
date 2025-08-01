namespace SSO.EndPoint.WebApp.Middleware.ValidationHandler;

public static class ValidationExceptionExtensions
{
    public static void UseValidationExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationExceptionMiddleware>();
    }
}
