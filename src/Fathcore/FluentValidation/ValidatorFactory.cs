using System;
using FluentValidation;
using FluentValidation.Attributes;

namespace Fathcore.FluentValidation
{
    /// <summary>
    /// Represents custom validator factory that looks for the attribute instance on the specified type in order to provide the validator instance.
    /// </summary>
    public class ValidatorFactory : AttributedValidatorFactory, IValidatorFactory
    {
        /// <summary>
        /// Gets a validator for the appropriate type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Created IValidator instance; null if a validator cannot be created.</returns>
        public override IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            var validatorAttribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
            if (validatorAttribute == null || validatorAttribute.ValidatorType == null)
                return null;

            var instance = Engine.Current.ResolveUnregistered(validatorAttribute.ValidatorType);

            return instance as IValidator;
        }
    }
}
