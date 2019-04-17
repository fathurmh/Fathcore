using FluentValidation;

namespace Fathcore.Infrastructure.FluentValidation
{
    /// <summary>
    /// Base class for validators.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom initialization code to constructors.
        /// </summary>
        protected virtual void PostInitialize()
        {
        }
    }
}
