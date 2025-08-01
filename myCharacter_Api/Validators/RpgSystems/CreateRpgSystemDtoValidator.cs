using FluentValidation;
using myCharacter.DTOs.RpgSystems;

public class CreateRpgSystemDtoValidator : AbstractValidator<CreateRpgSystemDto>
{
    public CreateRpgSystemDtoValidator()
    {
        // Regra para o Nome
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("RpgStystem name is required.")
            .MaximumLength(100).WithMessage("RpgSystem name cannot exceed 100 characters.");
    }
}