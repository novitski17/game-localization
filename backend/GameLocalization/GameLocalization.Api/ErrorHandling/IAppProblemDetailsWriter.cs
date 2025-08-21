using GameLocalization.Core.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.ErrorHandling
{
    public interface IAppProblemDetailsWriter
    {
        ProblemDetails Create(HttpContext httpContext, AppError error);
        ProblemDetails Create(
            HttpContext httpContext,
            int status,
            string title,
            Exception? devException = null);
    }
}
