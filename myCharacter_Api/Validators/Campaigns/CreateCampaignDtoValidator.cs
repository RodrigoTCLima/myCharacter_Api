using FluentValidation;
using myCharacter.DTOs.Campaign;

public class CreateCampaignDtoValidator : AbstractValidator<CreateCampaignDto>
{
    public CreateCampaignDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name is required.")
            .MaximumLength(150).WithMessage("Campaign name cannot exceed 150 characters");

        RuleFor(x => x.RpgSystemId)
            .NotEmpty().WithMessage("RPG System ID is required.");
    }
}