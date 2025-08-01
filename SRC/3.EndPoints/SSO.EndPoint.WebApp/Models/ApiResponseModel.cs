namespace SSO.EndPoint.WebApp.Models;

public static class ApiResponseModel
{
    public static ApiResponseModel<T> Success<T>(T data, string message = "Successed")
    {
        return new ApiResponseModel<T>()
        {
            Date = data,
            IsSuccess = true,
            Message = message,
        };
    }
    public static ApiResponseModel<T> Faild<T>(T data, string message = "Faild")
    {
        return new ApiResponseModel<T>()
        {
            Date = data,
            IsSuccess = false,
            Message = message,
        };
    }
}

public class ApiResponseModel<T>
{
    public T Date { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string RefreshToken { get; set; }
    public ApiResponseModel<T> SetRefreshToken (string value)
    {
        RefreshToken = value;
        return this;
    }
}