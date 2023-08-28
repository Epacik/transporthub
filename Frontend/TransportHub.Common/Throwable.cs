using Lindronics.OneOf.Result;

namespace TransportHub.Common;

public static class Throwable
{
    public static Result<T, Exception> ToResult<T>(this Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<Result<T, Exception>> ToResultAsync<T>(this Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<Result<T, Exception>> ToResultAsync<T>(this Task<T> task)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
