using FluentValidation;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequestValidator:AbstractValidator<RegisterUserRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(12).WithMessage("Username longer 12 characters");
        }
    }
}