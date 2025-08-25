using AutoMapper;
using FluentResults;
using GameLocalization.Api.ErrorHandling;
using GameLocalization.Core.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult(this ControllerBase controller, Result result)
            => result.IsSuccess ? controller.NoContent() : controller.MapErrors(result.Errors);

        public static ActionResult<TResponse> ToActionResult<TDomain, TResponse>(
            this ControllerBase controller,
            Result<TDomain> result,
            IMapper mapper,
            Func<TResponse, ActionResult<TResponse>>? onSuccess = null)
        {
            if (result.IsSuccess)
            {
                var dto = mapper.Map<TResponse>(result.Value);
                return onSuccess != null ? onSuccess(dto) : controller.Ok(dto);
            }
            return controller.MapErrors(result.Errors);
        }

        public static ActionResult ToActionResult<TDomain>(
            this ControllerBase controller,
            Result<TDomain> result,
            Func<TDomain, ActionResult> onSuccess)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : controller.MapErrors(result.Errors);
        }

        public static ActionResult<TResponse> ToOkWrapped<TDomain, TResponse>(
            this ControllerBase controller,
            Result<TDomain> result,
            IMapper mapper)
        {
            return controller.ToActionResult<TDomain, TResponse>(result, mapper, r => controller.Ok(r));
        }

        private static ActionResult MapErrors(this ControllerBase controller, List<IError> errors)
        {
            var writer = controller.HttpContext.RequestServices.GetRequiredService<IAppProblemDetailsWriter>();

            var appError = errors.OfType<AppError>().FirstOrDefault();
            if (appError != null)
            {
                var pd = writer.Create(controller.HttpContext, appError);
                return new ObjectResult(pd) { StatusCode = pd.Status };
            }

            var pd500 = writer.Create(controller.HttpContext, 500, "An unexpected error occurred.");
            return new ObjectResult(pd500) { StatusCode = 500 };
        }
    }
}