using FluentValidation;
using myCharacter.DTOs.Characters;

public class UpdateCharacterDtoValidator : AbstractValidator<UpdateCharacterDto>
{
    public UpdateCharacterDtoValidator()
    {
        // Regra para o Nome
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Character name is required.")
            .MaximumLength(100).WithMessage("Character name cannot exceed 100 characters.");

        // Regra para o Nível
        RuleFor(x => x.Level)
            .GreaterThan(0).WithMessage("Level must be a positive number.");
    }
}