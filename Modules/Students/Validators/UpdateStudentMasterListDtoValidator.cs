using CampusServicePortal_TicUnicorns.Modules.Students.DTOs;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Students.Validators;

public class UpdateStudentMasterListDtoValidator
    : AbstractValidator<UpdateStudentMasterListDto>
{
    public UpdateStudentMasterListDtoValidator()
    {
        RuleFor(x => x.UniversityId)
            .GreaterThan(0)
            .WithMessage("University ID must be greater than zero.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0)
            .WithMessage("Faculty ID must be greater than zero.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than zero.");

        RuleFor(x => x.UniversityStudentId)
            .NotEmpty()
            .WithMessage("University student ID is required.");

        RuleFor(x => x.StudentName)
            .NotEmpty()
            .WithMessage("Student name is required.");

        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.");
    }
}