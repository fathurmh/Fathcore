using System;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Specifies that the method that this attribute is applied to does not wrap a response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SkipResponseWrapperAttribute : Attribute
    {
        public SkipResponseWrapperAttribute() { }
    }
}
