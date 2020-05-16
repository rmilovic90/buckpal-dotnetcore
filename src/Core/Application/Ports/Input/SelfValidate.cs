using FluentValidation;

namespace Buckpal.Core.Application.Ports.Input
{
    public abstract class SelfValidate<T> where T : class
    {
        private readonly IValidator<T> _validator;

        protected SelfValidate(IValidator<T> validator) => _validator = validator;

        protected void ValidateSelf()
        {
            _validator.ValidateAndThrow(this as T);
        }
    }
}