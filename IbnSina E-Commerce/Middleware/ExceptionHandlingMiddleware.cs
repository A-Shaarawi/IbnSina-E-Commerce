using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace IbnSina.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (DbUpdateException)
        {
            await WriteError(context, HttpStatusCode.BadRequest,
                "The request could not be completed due to invalid or conflicting data (e.g. a category that doesn't exist).");
        }
        catch (Exception ex)
        {
            await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { status = (int)statusCode, error = message });
        await context.Response.WriteAsync(payload);
    }
}