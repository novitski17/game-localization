using FluentResults;

namespace GameLocalization.Core.Errors
{
    public abstract class AppError : Error
    {
        protected AppError(string message, int httpStatus, string code) : base(message)
        {
            Metadata["httpStatus"] = httpStatus;
            Metadata["code"] = code;
        }

        public int HttpStatus => Metadata
            .TryGetValue("httpStatus", out var s) && s is int i
            ? i
            : 400;

        public string Code => Metadata
            .TryGetValue("code", out var c)
            ? c?.ToString() ?? "error"
            : "error";
    }

    public sealed class NotFoundError : AppError
    {
        public NotFoundError(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.", 404, "NotFound")
        {
        }
    }

    public sealed class ConflictError : AppError
    {
        public ConflictError(string message = "Conflict")
            : base(message, 409, "Conflict")
        {
        }
    }

    public sealed class BadRequestError : AppError
    {
        public BadRequestError(string message = "Bad request")
            : base(message, 400, "BadRequest")
        {
        }
    }

    public sealed class DatabaseError : AppError
    {
        public DatabaseError(string message = "Database error")
            : base(message, 500, "Database")
        {
        }
    }

    public sealed class UnexpectedError : AppError
    {
        public UnexpectedError(Exception _)
            : base("Unexpected error occurred.", 500, "Unexpected")
        {
        }
    }

    public sealed class ValidationAppError : AppError
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationAppError(IDictionary<string, string[]> errors)
            : base("Validation failed", 400, "Validation")
        {
            Errors = new Dictionary<string, string[]>(errors ?? new Dictionary<string, string[]>());
        }
    }
}