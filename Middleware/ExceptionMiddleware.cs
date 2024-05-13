using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace API;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _Logger;
    private readonly IHostEnvironment _env;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _env = env;
        _Logger = logger;
        _next = next;
    }

    public ExceptionMiddleware(JsonNamingPolicy propertyNamingPolicy)
    {
        this.PropertyNamingPolicy = propertyNamingPolicy;

    }
    public JsonNamingPolicy PropertyNamingPolicy { get; private set; }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
            : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
