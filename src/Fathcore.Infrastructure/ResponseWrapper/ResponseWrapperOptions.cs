using System;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents a response wrapper options.
    /// </summary>
    public class ResponseWrapperOptions
    {
        private Type _responseHandler;

        /// <summary>
        /// Gets or sets the implementation type of <see cref="IResponseHandler"/>.
        /// </summary>
        public Type ResponseHandler
        {
            get => _responseHandler;
            set
            {
                if (!typeof(IResponseHandler).IsAssignableFrom(value) || !value.IsClass)
                    throw new InvalidOperationException($"The {nameof(value)} must be concrete class and implements {nameof(IResponseHandler)}.");

                _responseHandler = value;
            }
        }
    }
}
