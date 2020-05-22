using FluentValidation;

namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequestValidator:AbstractValidator<LoginUserRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}