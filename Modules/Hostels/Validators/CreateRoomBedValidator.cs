using CampusServicePortal.Modules.Hostels.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Hostels.Validators;

public class CreateRoomBedValidator : AbstractValidator<CreateRoomBedDto>
{
    public CreateRoomBedValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Room is required.");

        RuleFor(x => x.BedNumber)
            .NotEmpty()
            .WithMessage("Bed number is required.")
            .MaximumLength(20)
            .WithMessage("Bed number cannot exceed 20 characters.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Bed status is required.")
            .MaximumLength(30)
            .WithMessage("Bed status cannot exceed 30 characters.");
    }
}