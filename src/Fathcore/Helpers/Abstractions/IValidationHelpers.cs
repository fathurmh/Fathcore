using System;
using Fathcore.Abstractions;

namespace Fathcore.Helpers.Abstractions
{
    public interface IValidationHelpers : ISingletonService
    {
        void ThrowIfNull(string parameter, string parameterName);
        void ThrowIfNull(object parameter, string parameterName);
        void ThrowIfNull<T>(T parameter, string parameterName);
    }
}
