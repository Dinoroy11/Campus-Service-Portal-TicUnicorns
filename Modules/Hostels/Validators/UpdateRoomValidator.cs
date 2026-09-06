using CampusServicePortal.Modules.Hostels.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Hostels.Validators;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty()
            .WithMessage("Room number is required.")
            .MaximumLength(20)
            .WithMessage("Room number cannot exceed 20 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Room capacity must be greater than 0.");

        RuleFor(x => x.RoomType)
            .NotEmpty()
            .WithMessage("Room type is required.")
            .MaximumLength(50)
            .WithMessage("Room type cannot exceed 50 characters.");
    }
}