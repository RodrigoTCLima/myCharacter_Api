using FluentValidation;
using myCharacter.DTOs.Campaign;

public class UpdateCampaignDtoValidator : AbstractValidator<UpdateCampaignDto>
{
    public UpdateCampaignDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name is required.")
            .MaximumLength(150).WithMessage("Campaign name cannot exceed 150 characters");
    }
}
