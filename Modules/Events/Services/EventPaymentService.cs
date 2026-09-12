using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Enums;
using CampusServicePortal.Modules.Events.Repositories;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal.Modules.Events.Services;

public class EventPaymentService : IEventPaymentService
{
    private readonly IEventPaymentRepository _paymentRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventSeatRepository _seatRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public EventPaymentService(
        IEventPaymentRepository paymentRepository,
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEventSeatRepository seatRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _paymentRepository = paymentRepository;
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _seatRepository = seatRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    public async Task<List<EventPaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return payments.Select(MapToDto).ToList();
    }

    public async Task<EventPaymentDto?> GetByIdAsync(int eventPaymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(eventPaymentId);
        return payment == null ? null : MapToDto(payment);
    }

    public async Task<EventPaymentDto?> GetByRegistrationIdAsync(int registrationId)
    {
        var payment = await _paymentRepository.GetByRegistrationIdAsync(registrationId);
        return payment == null ? null : MapToDto(payment);
    }

    public async Task<EventPaymentDto> CreateAsync(EventPaymentDto dto, int studentId)
    {
        var registration = await _registrationRepository.GetByIdAsync(dto.RegistrationId)
            ?? throw new ArgumentException("Event registration not found.");

        if (registration.StudentId != studentId)
            throw new UnauthorizedAccessException("This registration does not belong to the current student.");

        var eventEntity = await _eventRepository.GetByIdAsync(registration.EventId)
            ?? throw new ArgumentException("Event not found.");

        ValidatePayableRegistration(registration, eventEntity);
        ValidateAmount(dto.Amount, eventEntity);

        var existingPayment = await _paymentRepository.GetByRegistrationIdAsync(dto.RegistrationId);
        if (existingPayment != null)
            throw new ArgumentException("A payment already exists for this registration.");

        if (dto.PaymentStatus != EventPaymentStatus.Pending &&
            dto.PaymentStatus != EventPaymentStatus.Paid)
        {
            throw new ArgumentException(
                "A new simulated payment can only be created as Pending or Paid.");
        }

        var payment = new EventPayment
        {
            RegistrationId = dto.RegistrationId,
            Amount = dto.Amount,
            PaymentStatus = dto.PaymentStatus,
            PaymentReference = dto.PaymentReference?.Trim(),
            PaidAt = dto.PaymentStatus == EventPaymentStatus.Paid
                ? dto.PaidAt ?? DateTime.UtcNow
                : null
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);

        if (createdPayment.PaymentStatus == EventPaymentStatus.Paid)
            await ConfirmPaidRegistrationAsync(registration, eventEntity);

        return MapToDto(createdPayment);
    }

    public async Task<bool> UpdateAsync(int eventPaymentId, EventPaymentDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(eventPaymentId);
        if (payment == null)
            return false;

        var registration = await _registrationRepository.GetByIdAsync(payment.RegistrationId)
            ?? throw new ArgumentException("Event registration not found.");

        var eventEntity = await _eventRepository.GetByIdAsync(registration.EventId)
            ?? throw new ArgumentException("Event not found.");

        if (!eventEntity.IsPaid)
            throw new ArgumentException("Payment is not required for a free event.");

        ValidateAmount(dto.Amount, eventEntity);

        if (dto.PaymentStatus == EventPaymentStatus.Paid)
            ValidatePayableRegistration(registration, eventEntity);

        payment.Amount = dto.Amount;
        payment.PaymentStatus = dto.PaymentStatus;
        payment.PaymentReference = dto.PaymentReference?.Trim();
        payment.PaidAt = dto.PaymentStatus == EventPaymentStatus.Paid
            ? dto.PaidAt ?? DateTime.UtcNow
            : dto.PaidAt;

        await _paymentRepository.UpdateAsync(payment);

        if (payment.PaymentStatus == EventPaymentStatus.Paid &&
            registration.Status != EventRegistrationStatus.Confirmed)
        {
            await ConfirmPaidRegistrationAsync(registration, eventEntity);
        }

        return true;
    }

    private static void ValidatePayableRegistration(
        EventRegistration registration,
        Event eventEntity)
    {
        if (!eventEntity.IsPaid)
            throw new ArgumentException("Payment is not required for a free event.");

        if (DateTime.UtcNow >= eventEntity.StartDateTime)
            throw new ArgumentException("Payment is closed because the event has already started.");

        if (registration.Status == EventRegistrationStatus.Expired ||
            registration.Status == EventRegistrationStatus.Cancelled)
        {
            throw new ArgumentException("This registration is no longer active.");
        }

        if (registration.Status == EventRegistrationStatus.Held &&
            registration.ExpiresAt.HasValue &&
            registration.ExpiresAt.Value <= DateTime.UtcNow)
        {
            throw new ArgumentException("The registration hold has expired. Please register again.");
        }

        if (registration.Status == EventRegistrationStatus.Confirmed)
            throw new ArgumentException("This registration is already confirmed.");
    }

    private static void ValidateAmount(decimal amount, Event eventEntity)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (!eventEntity.FeeAmount.HasValue || amount != eventEntity.FeeAmount.Value)
            throw new ArgumentException("Payment amount does not match the event fee.");
    }

    private async Task ConfirmPaidRegistrationAsync(
        EventRegistration registration,
        Event eventEntity)
    {
        registration.Status = EventRegistrationStatus.Confirmed;
        registration.ExpiresAt = null;
        await _registrationRepository.UpdateAsync(registration);

        if (registration.EventSeatId.HasValue)
        {
            var seat = await _seatRepository.GetByIdAsync(registration.EventSeatId.Value);
            if (seat != null)
            {
                seat.Status = "Booked";
                await _seatRepository.UpdateAsync(seat);
            }
        }

        var student = await _studentRepository.GetByIdAsync(registration.StudentId);
        if (student?.UserId is int userId)
        {
            await _notificationService.CreateAsync(new NotificationCreateDto
            {
                UserId = userId,
                Title = "Event Registration Confirmed",
                Message = $"Payment received. Your registration for {eventEntity.EventName} is confirmed.",
                ReferenceType = "EventRegistration",
                ReferenceId = registration.EventRegistrationId
            });
        }
    }

    private static EventPaymentDto MapToDto(EventPayment payment)
    {
        return new EventPaymentDto
        {
            EventPaymentId = payment.EventPaymentId,
            RegistrationId = payment.RegistrationId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            PaymentReference = payment.PaymentReference,
            PaidAt = payment.PaidAt
        };
    }
}
