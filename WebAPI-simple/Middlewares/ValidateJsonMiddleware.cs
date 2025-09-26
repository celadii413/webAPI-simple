using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebAPI_simple.Middlewares
{
    public class ValidateJsonMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateJsonMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/books/add-book") &&
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; 

                if (string.IsNullOrWhiteSpace(body))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Request body cannot be empty");
                    return;
                }

                try
                {
                    using var jsonDoc = JsonDocument.Parse(body);
                    var root = jsonDoc.RootElement;

                    // Kiểm tra các trường bắt buộc
                    // Title
                    if (!root.TryGetProperty("title", out var title) || string.IsNullOrWhiteSpace(title.GetString()))
                    {
                        await WriteError(context, 400, "Book Title cannot be empty or contain special characters");
                        return;
                    }

                    // PublisherID
                    if (!root.TryGetProperty("publisherID", out var publisherId) || publisherId.GetInt32() <= 0)
                    {
                        await WriteError(context, 400, "PublisherId does not exist");
                        return;
                    }

                    // AuthorIds
                    if (!root.TryGetProperty("authorIds", out var authors) ||
                        authors.ValueKind != JsonValueKind.Array ||
                        authors.GetArrayLength() == 0)
                    {
                        await WriteError(context, 400, "Missing or invalid 'AuthorIds'");
                        return;
                    }

                }
                catch (JsonException)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid JSON format");
                    return;
                }
            }
            await _next(context);
        }

        private async Task WriteError(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorObj = new { error = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj));
        }
    }

    public static class ValidateJsonMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateJsonMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateJsonMiddleware>();
        }
    }
}
