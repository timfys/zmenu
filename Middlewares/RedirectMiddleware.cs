namespace Menu4Tech.Middlewares;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "GET" && context.Request.Host.Value.Contains("zmenu", StringComparison.OrdinalIgnoreCase) && !context.Request.Host.Value.Contains("www", StringComparison.OrdinalIgnoreCase) && !context.Request.Host.Value.StartsWith("m.", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect($"https://www.zmenu.net{context.Request.Path.Value}{context.Request.QueryString.Value}");
            return;
        }
        
        await _next(context);
    }
}