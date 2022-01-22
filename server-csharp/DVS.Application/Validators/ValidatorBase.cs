using FluentValidation;

namespace DVS.Application.Validators
{
    public abstract class ValidatorBase<TModel> : AbstractValidator<TModel>
    {
    }
}