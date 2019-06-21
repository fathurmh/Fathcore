using Microsoft.AspNetCore.Http;

namespace Fathcore.Infrastructure.Enum
{
    /// <summary>
    /// Provides the class for a strongly typed enumerations.
    /// </summary>
    public sealed class ErrorType : TypeSafeEnum<ErrorType, int>, ITypeSafeEnum<int>, IErrorType, ITypeSafeEnum
    {
        /// <summary>
        /// Gets a status code value.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets invalid request error type.
        /// </summary>
        public static ErrorType InvalidRequest { get; } = new ErrorType(0, nameof(InvalidRequest), "INVALID_REQUEST", StatusCodes.Status400BadRequest);
        public static ErrorType UnauthorizedAccess { get; } = new ErrorType(1, nameof(UnauthorizedAccess), "UNAUTHORIZED_ACCESS", StatusCodes.Status401Unauthorized);
        public static ErrorType Forbidden { get; } = new ErrorType(2, nameof(Forbidden), "FORBIDDEN", StatusCodes.Status403Forbidden);
        public static ErrorType ResourceNotFound { get; } = new ErrorType(3, nameof(ResourceNotFound), "RESOURCE_NOT_FOUND", StatusCodes.Status404NotFound);
        public static ErrorType InternalServerError { get; } = new ErrorType(4, nameof(InternalServerError), "INTERNAL_SERVER_ERROR", StatusCodes.Status500InternalServerError);
        public static ErrorType Conflict { get; } = new ErrorType(5, nameof(Conflict), "INVALID_DATA", StatusCodes.Status409Conflict);
        public static ErrorType PreconditionFailed { get; } = new ErrorType(6, nameof(PreconditionFailed), "INVALID_DATA", StatusCodes.Status412PreconditionFailed);

        private ErrorType(int id, string name, string description, int statusCode) : base(id, name, description)
        {
            StatusCode = statusCode;
        }
    }
}
