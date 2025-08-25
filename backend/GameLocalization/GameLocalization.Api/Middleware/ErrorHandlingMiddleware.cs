using GameLocalization.Api.ErrorHandling;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private const int ClientClosedStatus = 499;

        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IAppProblemDetailsWriter _writer;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IAppProblemDetailsWriter writer,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _writer = writer;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                await HandleCancellationAsync(context);
            }
            catch (Exception ex)
            {
                await HandleUnhandledAsync(context, ex);
            }
        }

        private Task HandleCancellationAsync(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = ClientClosedStatus;
            }
            return Task.CompletedTask;
        }

        private async Task HandleUnhandledAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw ex;
            }

            var pd = _writer.Create(
                context,
                status: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                devException: _env.IsDevelopment() ? ex : null);

            await WriteProblemAsync(context, pd);
        }

        private static async Task WriteProblemAsync(HttpContext context, ProblemDetails pd)
        {
            context.Response.Clear();
            context.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(pd);
        }
    }
}
