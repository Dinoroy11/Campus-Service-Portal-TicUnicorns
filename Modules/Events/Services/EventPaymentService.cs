using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Entities;
using CampusServicePortal.Modules.Events.Repositories;
 

namespace CampusServicePortal.Modules.Events.Services;

public class EventPaymentService : IEventPaymentService
{
    private readonly IEventPaymentRepository _paymentRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;

    public EventPaymentService(
        IEventPaymentRepository paymentRepository,
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository)
    {
        _paymentRepository = paymentRepository;
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
    }

    public async Task<List<EventPaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();

        return payments
            .Select(MapToDto)
            .ToList();
    }

    public async Task<EventPaymentDto?> GetByIdAsync(
        int eventPaymentId)
    {
        var payment =
            await _paymentRepository.GetByIdAsync(eventPaymentId);

        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<EventPaymentDto?> GetByRegistrationIdAsync(
        int registrationId)
    {
        var payment =
            await _paymentRepository.GetByRegistrationIdAsync(
                registrationId);

        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<EventPaymentDto> CreateAsync(
        EventPaymentDto dto)
    {
        var registration =
            await _registrationRepository.GetByIdAsync(
                dto.RegistrationId);

        if (registration == null)
            throw new ArgumentException(
                "Event registration not found.");

        var eventEntity =
            await _eventRepository.GetByIdAsync(
                registration.EventId);

        if (eventEntity == null)
            throw new ArgumentException(
                "Event not found.");

        if (!eventEntity.IsPaid)
        {
            throw new ArgumentException(
                "Payment is not required for a free event.");
        }

        if (dto.Amount <= 0)
        {
            throw new ArgumentException(
                "Payment amount must be greater than zero.");
        }

        if (eventEntity.FeeAmount.HasValue &&
            dto.Amount != eventEntity.FeeAmount.Value)
        {
            throw new ArgumentException(
                "Payment amount does not match the event fee.");
        }

        var existingPayment =
            await _paymentRepository.GetByRegistrationIdAsync(
                dto.RegistrationId);

        if (existingPayment != null)
        {
            throw new ArgumentException(
                "A payment already exists for this registration.");
        }

        var payment = new EventPayment
        {
            RegistrationId = dto.RegistrationId,
            Amount = dto.Amount,
            PaymentStatus = string.IsNullOrWhiteSpace(dto.PaymentStatus)
                ? "Pending"
                : dto.PaymentStatus.Trim(),
            PaymentReference = dto.PaymentReference?.Trim(),
            PaidAt = dto.PaidAt
        };

        var createdPayment =
            await _paymentRepository.CreateAsync(payment);

        return MapToDto(createdPayment);
    }

    public async Task<bool> UpdateAsync(
        int eventPaymentId,
        EventPaymentDto dto)
    {
        var payment =
            await _paymentRepository.GetByIdAsync(
                eventPaymentId);

        if (payment == null)
            return false;

        var registration =
            await _registrationRepository.GetByIdAsync(
                payment.RegistrationId);

        if (registration == null)
            throw new ArgumentException(
                "Event registration not found.");

        var eventEntity =
            await _eventRepository.GetByIdAsync(
                registration.EventId);

        if (eventEntity == null)
            throw new ArgumentException(
                "Event not found.");

        if (!eventEntity.IsPaid)
        {
            throw new ArgumentException(
                "Payment is not required for a free event.");
        }

        if (dto.Amount <= 0)
        {
            throw new ArgumentException(
                "Payment amount must be greater than zero.");
        }

        if (eventEntity.FeeAmount.HasValue &&
            dto.Amount != eventEntity.FeeAmount.Value)
        {
            throw new ArgumentException(
                "Payment amount does not match the event fee.");
        }

        payment.Amount = dto.Amount;
        payment.PaymentStatus = dto.PaymentStatus.Trim();
        payment.PaymentReference = dto.PaymentReference?.Trim();
        payment.PaidAt = dto.PaidAt;

        await _paymentRepository.UpdateAsync(payment);

        return true;
    }

    private static EventPaymentDto MapToDto(
        EventPayment payment)
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