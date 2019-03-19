using System;

namespace Fathcore.ResponseWrapper
{
    public class ResponseWrapperOptions
    {
        private Type _responseHandler;

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
