
namespace WeatherCollector.Api.Middleware;

public class RequestTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

    public RequestTimeoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            context.RequestAborted,
            timeoutCts.Token);

        // Replace request abortion token 
        context.RequestAborted = linkedCts.Token;

        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
            if (timeoutCts.IsCancellationRequested)
            {
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsync("Request exceeded 5-second timeout.");
                return;
            }

            throw;
        }
    }
}
