
using System.ComponentModel.DataAnnotations;
using Fathcore.Infrastructures;
using Fathcore.Localization.Resources;
using Microsoft.Extensions.Localization;

namespace Fathcore.Filters
{
    /// <summary>
    /// Represents a required attribute
    /// </summary>
    public class RequiredAttribute : ValidationAttribute
    {
        private readonly IStringLocalizer _stringLocalizer;

        /// <summary>
        /// Gets or sets the required attribute error message
        /// </summary>
        public new string ErrorMessage { get; set; }

        public RequiredAttribute()
        {
            _stringLocalizer = EngineContext.Current.Resolve<IStringLocalizer<AttributeMessage>>();
        }

        /// <summary>
        /// Overrides is valid method from <see cref="ValidationAttribute"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            if (value is string valueString)
            {
                if (string.IsNullOrWhiteSpace(valueString))
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Overrides format error message method from <see cref="ValidationAttribute"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            string errorMessage = ErrorMessage;
            ErrorMessage = null;

            if (errorMessage == null)
            {
                errorMessage = _stringLocalizer.GetString(AttributeMessage.Required);
            }

            return string.Format(errorMessage, name);
        }
    }
}
