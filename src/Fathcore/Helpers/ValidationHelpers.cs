using System;
using Fathcore.Helpers.Abstractions;

namespace Fathcore.Helpers
{
    public class ValidationHelpers : IValidationHelpers
    {
        public ValidationHelpers()
        {
            
        }

        public virtual void ThrowIfNull(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter)) throw new ArgumentNullException(parameterName);
        }
        
        public virtual void ThrowIfNull(object parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }

        public virtual void ThrowIfNull<T>(T parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }
    }
}
