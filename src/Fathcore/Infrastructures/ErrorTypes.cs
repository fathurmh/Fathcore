using System;

namespace Fathcore.Infrastructures
{
    public static class ErrorTypes
    {
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string InvalidData = "INVALID_DATA";
        public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
        public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
        public const string ApiError = "API_ERROR";
        public const string SystemError = "SYSTEM_ERROR";
    }
}
