using CampusServicePortal.Modules.Fees.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Fees.Validators;

public class CreateFeeTypeValidator
    : AbstractValidator<CreateFeeTypeDto>
{
    public CreateFeeTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Fee type name is required.")
            .MaximumLength(100)
            .WithMessage("Fee type name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Fee type description cannot exceed 500 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Fee amount must be greater than 0.");
    }
}


public class CreateStudentFeeValidator
    : AbstractValidator<CreateStudentFeeDto>
{
    public CreateStudentFeeValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be greater than 0.");

        RuleFor(x => x.FeeTypeId)
            .GreaterThan(0)
            .WithMessage("FeeTypeId must be greater than 0.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Fee amount must be greater than 0.");

        RuleFor(x => x.ExamReference)
            .MaximumLength(200)
            .WithMessage("Exam reference cannot exceed 200 characters.");
    }
}


public class FeePaymentValidator
    : AbstractValidator<FeePaymentDto>
{
    public FeePaymentValidator()
    {
        RuleFor(x => x.StudentFeeId)
            .GreaterThan(0)
            .WithMessage("StudentFeeId must be greater than 0.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than 0.");

        RuleFor(x => x.PaymentStatus)
            .NotEmpty()
            .WithMessage("Payment status is required.");

        RuleFor(x => x.PaymentReference)
            .MaximumLength(200)
            .WithMessage("Payment reference cannot exceed 200 characters.");

        RuleFor(x => x.SourcePaymentId)
            .GreaterThan(0)
            .When(x => x.SourcePaymentId.HasValue)
            .WithMessage("SourcePaymentId must be greater than 0.");

        RuleFor(x => x.SourceFeeId)
            .GreaterThan(0)
            .When(x => x.SourceFeeId.HasValue)
            .WithMessage("SourceFeeId must be greater than 0.");

        RuleFor(x => x.TargetStudentFeeId)
            .GreaterThan(0)
            .When(x => x.TargetStudentFeeId.HasValue)
            .WithMessage("TargetStudentFeeId must be greater than 0.");
    }
}


public class RefundRequestValidator
    : AbstractValidator<RefundRequestDto>
{
    public RefundRequestValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithMessage("PaymentId must be greater than 0.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Refund reason is required.")
            .MaximumLength(1000)
            .WithMessage("Refund reason cannot exceed 1000 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than 0.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Refund status is required.");

        RuleFor(x => x.ReviewedByUserId)
            .GreaterThan(0)
            .When(x => x.ReviewedByUserId.HasValue)
            .WithMessage("ReviewedByUserId must be greater than 0.");
    }
}