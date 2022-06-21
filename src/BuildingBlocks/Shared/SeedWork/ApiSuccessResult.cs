namespace Shared.SeedWork;

public class ApiSuccessResult<T> : ApiResult<T>
{
    public ApiSuccessResult(T data) : base(true, data)
    {
    }

    public ApiSuccessResult(T data, string message) : base(true, data, message)
    {
    }
}