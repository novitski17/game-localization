using GameLocalization.Core.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameLocalization.Api.ErrorHandling
{
    public sealed class DefaultProblemDetailsWriter : IAppProblemDetailsWriter
    {
        private readonly IWebHostEnvironment _env;
        public DefaultProblemDetailsWriter(IWebHostEnvironment env) => _env = env;

        public ProblemDetails Create(HttpContext httpContext, AppError error)
        {
            if (error is ValidationAppError ve)
            {
                var pd = new ProblemDetails
                {
                    Title = ve.Message,
                    Status = ve.HttpStatus,
                    Instance = httpContext.Request.Path
                };

                pd.Extensions["errors"] = ve.Errors;

                Enrich(pd, httpContext, ve.Code, devException: null);
                return pd;
            }

            var other = new ProblemDetails
            {
                Title = error.Message,
                Status = error.HttpStatus,
                Instance = httpContext.Request.Path
            };
            Enrich(other, httpContext, error.Code, devException: null);
            return other;
        }

        public ProblemDetails Create(HttpContext httpContext, int status, string title, Exception? devException = null)
        {
            var pd = new ProblemDetails
            {
                Title = title,
                Status = status,
                Instance = httpContext.Request.Path
            };
            Enrich(pd, httpContext, code: null, devException);
            return pd;
        }

        private void Enrich(ProblemDetails pd, HttpContext ctx, string? code, Exception? devException)
        {
            pd.Extensions["traceId"] = Activity.Current?.Id ?? ctx.TraceIdentifier;
            if (!string.IsNullOrWhiteSpace(code))
                pd.Extensions["code"] = code;

            if (_env.IsDevelopment() && devException != null)
            {
                pd.Extensions["exception"] = devException.GetType().Name;
                pd.Extensions["stackTrace"] = devException.StackTrace ?? string.Empty;
            }
        }
    }
}
