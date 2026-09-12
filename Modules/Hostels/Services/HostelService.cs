using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal.Modules.Hostels.DTOs;
using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal.Modules.Hostels.Repositories;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Fees.Enums;

namespace CampusServicePortal.Modules.Hostels.Services;

public class HostelService : IHostelService
{
    private const int DefaultHoldMinutes = 15;
    private const int HostelFeeDueDays = 7;
    private const string HostelFeeTypeName = "Hostel Accommodation Fee";

    private readonly IHostelRepository _hostelRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;
    private readonly IFeeTypeRepository _feeTypeRepository;
    private readonly IStudentFeeRepository _studentFeeRepository;
    private readonly IFeePaymentRepository _feePaymentRepository;

    public HostelService(
        IHostelRepository hostelRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService,
        IFeeTypeRepository feeTypeRepository,
        IStudentFeeRepository studentFeeRepository,
        IFeePaymentRepository feePaymentRepository)
    {
        _hostelRepository = hostelRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
        _feeTypeRepository = feeTypeRepository;
        _studentFeeRepository = studentFeeRepository;
        _feePaymentRepository = feePaymentRepository;
    }

    public async Task<List<HostelBlueprintDto>> GetHostelsAsync()
    {
        var hostels = await _hostelRepository.GetHostelsAsync();

        return hostels.Select(h => new HostelBlueprintDto
        {
            HostelId = h.HostelId,
            HostelName = h.HostelName
        }).ToList();
    }

    public async Task<HostelBlueprintDto?> GetHostelBlueprintAsync(int hostelId)
    {
        var now = DateTime.UtcNow;
        await _hostelRepository.ExpireActiveHoldsAsync(now);

        var hostel = await _hostelRepository
            .GetHostelBlueprintAsync(hostelId);

        if (hostel is null)
        {
            return null;
        }

        var heldBedIds = (await _hostelRepository
            .GetActiveHeldBedIdsAsync(now))
            .ToHashSet();

        var allocatedBedIds = (await _hostelRepository
            .GetActivelyAllocatedBedIdsAsync())
            .ToHashSet();

        return new HostelBlueprintDto
        {
            HostelId = hostel.HostelId,
            HostelName = hostel.HostelName,
            Floors = hostel.Floors
                .Where(f => f.IsActive)
                .OrderBy(f => f.FloorNumber)
                .Select(f => new FloorBlueprintDto
                {
                    FloorId = f.FloorId,
                    FloorNumber = f.FloorNumber,
                    Name = f.Name,
                    Rooms = f.Rooms
                        .Where(r => r.IsActive)
                        .OrderBy(r => r.RoomNumber)
                        .Select(r =>
                        {
                            var beds = r.RoomBeds
                                .Where(b => b.IsActive)
                                .OrderBy(b => b.BedNumber)
                                .Select(b =>
                                {
                                    var isAllocated =
                                        allocatedBedIds.Contains(b.BedId) ||
                                        b.Status.Equals(
                                            "Occupied",
                                            StringComparison.OrdinalIgnoreCase);

                                    var isHeld = heldBedIds.Contains(b.BedId);

                                    var isBaseAvailable =
                                        b.Status.Equals(
                                            "Available",
                                            StringComparison.OrdinalIgnoreCase);

                                    var isAvailable =
                                        isBaseAvailable &&
                                        !isHeld &&
                                        !isAllocated;

                                    var displayStatus = isAllocated
                                        ? "Occupied"
                                        : isHeld
                                            ? "Held"
                                            : b.Status;

                                    return new BedBlueprintDto
                                    {
                                        BedId = b.BedId,
                                        BedNumber = b.BedNumber,
                                        Status = displayStatus,
                                        IsAvailable = isAvailable
                                    };
                                })
                                .ToList();

                            return new RoomBlueprintDto
                            {
                                RoomId = r.RoomId,
                                RoomNumber = r.RoomNumber,
                                Capacity = r.Capacity,
                                RoomType = r.RoomType,
                                IsAvailable = beds.Any(b => b.IsAvailable),
                                Beds = beds
                            };
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    public async Task<HostelBlueprintDto> CreateHostelAsync(
        CreateHostelDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.HostelName))
        {
            throw new ArgumentException("Hostel name is required.");
        }

        var hostel = new Hostel
        {
            UniversityId = dto.UniversityId,
            HostelName = dto.HostelName.Trim(),
            HostelType = dto.HostelType.Trim(),
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        var createdHostel =
            await _hostelRepository.CreateHostelAsync(hostel);

        return new HostelBlueprintDto
        {
            HostelId = createdHostel.HostelId,
            HostelName = createdHostel.HostelName
        };
    }

    public async Task CreateFloorAsync(CreateFloorDto dto)
    {
        var hostel = await _hostelRepository.GetHostelByIdAsync(dto.HostelId);

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        var floor = new Floor
        {
            HostelId = dto.HostelId,
            FloorNumber = dto.FloorNumber,
            Name = dto.Name,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateFloorAsync(floor);
    }

    public async Task CreateRoomAsync(CreateRoomDto dto)
    {
        var floor = await _hostelRepository.GetFloorByIdAsync(dto.FloorId);

        if (floor is null)
        {
            throw new KeyNotFoundException("Floor not found.");
        }

        if (dto.Capacity <= 0)
        {
            throw new ArgumentException("Room capacity must be greater than zero.");
        }

        var room = new Room
        {
            FloorId = dto.FloorId,
            RoomNumber = dto.RoomNumber,
            Capacity = dto.Capacity,
            RoomType = dto.RoomType,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateRoomAsync(room);
    }

    public async Task CreateRoomBedAsync(CreateRoomBedDto dto)
    {
        var room = await _hostelRepository.GetRoomByIdAsync(dto.RoomId);

        if (room is null)
        {
            throw new KeyNotFoundException("Room not found.");
        }

        var activeBedCount = await _hostelRepository
            .CountActiveBedsInRoomAsync(dto.RoomId);

        if (dto.IsActive && activeBedCount >= room.Capacity)
        {
            throw new InvalidOperationException(
                "Room capacity has already been reached.");
        }

        if (await _hostelRepository.BedNumberExistsInRoomAsync(
                dto.RoomId,
                dto.BedNumber))
        {
            throw new InvalidOperationException(
                "A bed with this number already exists in the room.");
        }

        var roomBed = new RoomBed
        {
            RoomId = dto.RoomId,
            BedNumber = dto.BedNumber,
            Status = string.IsNullOrWhiteSpace(dto.Status)
                ? "Available"
                : dto.Status,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateRoomBedAsync(roomBed);
    }

    public async Task<bool> UpdateHostelAsync(
        int hostelId,
        UpdateHostelDto dto)
    {
        var hostel = await _hostelRepository.GetHostelByIdAsync(hostelId);

        if (hostel is null)
        {
            return false;
        }

        hostel.HostelName = dto.HostelName;
        hostel.HostelType = dto.HostelType;
        hostel.Description = dto.Description;
        hostel.IsActive = dto.IsActive;

        await _hostelRepository.UpdateHostelAsync(hostel);
        return true;
    }

    public async Task<bool> UpdateFloorAsync(
        int floorId,
        UpdateFloorDto dto)
    {
        var floor = await _hostelRepository.GetFloorByIdAsync(floorId);

        if (floor is null)
        {
            return false;
        }

        floor.FloorNumber = dto.FloorNumber;
        floor.Name = dto.Name;
        floor.IsActive = dto.IsActive;

        await _hostelRepository.UpdateFloorAsync(floor);
        return true;
    }

    public async Task<bool> UpdateRoomAsync(
        int roomId,
        UpdateRoomDto dto)
    {
        var room = await _hostelRepository.GetRoomByIdAsync(roomId);

        if (room is null)
        {
            return false;
        }

        var activeBedCount = await _hostelRepository
            .CountActiveBedsInRoomAsync(roomId);

        if (dto.Capacity < activeBedCount)
        {
            throw new InvalidOperationException(
                "Room capacity cannot be lower than the number of active beds.");
        }

        room.RoomNumber = dto.RoomNumber;
        room.Capacity = dto.Capacity;
        room.RoomType = dto.RoomType;
        room.IsActive = dto.IsActive;

        await _hostelRepository.UpdateRoomAsync(room);
        return true;
    }

    public async Task<bool> UpdateRoomBedAsync(
        int bedId,
        UpdateRoomBedDto dto)
    {
        var roomBed = await _hostelRepository.GetRoomBedByIdAsync(bedId);

        if (roomBed is null)
        {
            return false;
        }

        if (await _hostelRepository.IsBedActivelyAllocatedAsync(bedId) &&
            (!dto.IsActive ||
             !dto.Status.Equals(
                 "Occupied",
                 StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "An actively allocated bed cannot be disabled or marked available.");
        }

        roomBed.BedNumber = dto.BedNumber;
        roomBed.Status = dto.Status;
        roomBed.IsActive = dto.IsActive;

        await _hostelRepository.UpdateRoomBedAsync(roomBed);
        return true;
    }

    public async Task<HostelHoldDto> CreateHoldAsync(
        int userId,
        int bedId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        await _hostelRepository.ExpireActiveHoldsAsync(now);

        if (await _hostelRepository.HasActiveApplicationAsync(student.StudentId))
        {
            throw new InvalidOperationException(
                "You already have an active hostel application.");
        }

        if (await _hostelRepository.HasActiveAllocationAsync(student.StudentId))
        {
            throw new InvalidOperationException(
                "You already have an active hostel allocation.");
        }

        var bed = await _hostelRepository.GetRoomBedByIdAsync(bedId);

        if (bed is null || !bed.IsActive)
        {
            throw new KeyNotFoundException("Bed not found or inactive.");
        }

        if (!bed.Status.Equals(
                "Available",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Bed is not available.");
        }

        if (await _hostelRepository.IsBedActivelyAllocatedAsync(bedId))
        {
            throw new InvalidOperationException("Bed is already occupied.");
        }

        var bedHold = await _hostelRepository
            .GetActiveHoldForBedAsync(bedId, now);

        if (bedHold is not null)
        {
            throw new InvalidOperationException(
                "This bed is currently held by another student.");
        }

        var studentHold = await _hostelRepository
            .GetActiveHoldForStudentAsync(student.StudentId, now);

        if (studentHold is not null)
        {
            throw new InvalidOperationException(
                "You already have an active hostel bed hold.");
        }

        var hold = new HostelRoomHold
        {
            StudentId = student.StudentId,
            RoomBedId = bedId,
            ApplicationId = null,
            HeldAt = now,
            ExpiresAt = now.AddMinutes(DefaultHoldMinutes),
            Status = "Active"
        };

        var created = await _hostelRepository.CreateHoldAsync(hold);
        return MapHold(created);
    }

    public async Task<bool> ReleaseHoldAsync(
        int userId,
        int holdId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var hold = await _hostelRepository.GetHoldByIdAsync(holdId);

        if (hold is null)
        {
            return false;
        }

        if (hold.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot release another student's hold.");
        }

        if (hold.ApplicationId is not null)
        {
            throw new InvalidOperationException(
                "This hold is already linked to an application. Cancel the application instead.");
        }

        if (!hold.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        hold.Status = "Released";
        await _hostelRepository.UpdateHoldAsync(hold);
        return true;
    }

    public async Task<HostelApplicationDto> CreateApplicationAsync(
        int userId,
        CreateHostelApplicationDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        await _hostelRepository.ExpireActiveHoldsAsync(now);

        if (string.IsNullOrWhiteSpace(dto.District) ||
            string.IsNullOrWhiteSpace(dto.Province))
        {
            throw new ArgumentException(
                "District and province are required.");
        }

        if (await _hostelRepository.HasActiveApplicationAsync(student.StudentId))
        {
            throw new InvalidOperationException(
                "You already have an active hostel application.");
        }

        if (await _hostelRepository.HasActiveAllocationAsync(student.StudentId))
        {
            throw new InvalidOperationException(
                "You already have an active hostel allocation.");
        }

        var hostel = await _hostelRepository.GetHostelByIdAsync(dto.HostelId);

        if (hostel is null || !hostel.IsActive)
        {
            throw new KeyNotFoundException("Hostel not found or inactive.");
        }

        var hold = await _hostelRepository.GetHoldByIdAsync(dto.HoldId);

        if (hold is null)
        {
            throw new KeyNotFoundException("Hostel bed hold not found.");
        }

        if (hold.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "This hold does not belong to the current student.");
        }

        if (!hold.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase) ||
            hold.ExpiresAt <= now)
        {
            throw new InvalidOperationException(
                "The hostel bed hold has expired or is no longer active.");
        }

        if (hold.ApplicationId is not null)
        {
            throw new InvalidOperationException(
                "This hold is already linked to an application.");
        }

        var heldBedHostelId = await _hostelRepository
            .GetHostelIdByBedIdAsync(hold.RoomBedId);

        if (heldBedHostelId != dto.HostelId)
        {
            throw new InvalidOperationException(
                "The selected bed does not belong to the selected hostel.");
        }

        var application = new HostelApplication
        {
            StudentId = student.StudentId,
            HostelId = dto.HostelId,
            District = dto.District.Trim(),
            Province = dto.Province.Trim(),
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = now,
            UpdatedAt = null
        };

        var createdApplication = await _hostelRepository
            .CreateApplicationAsync(application);

        hold.ApplicationId = createdApplication.HostelApplicationId;
        await _hostelRepository.UpdateHoldAsync(hold);

        return MapApplication(createdApplication, hold.RoomBedId);
    }

    public async Task<List<HostelApplicationDto>> GetMyApplicationsAsync(
        int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var applications = await _hostelRepository
            .GetApplicationsByStudentAsync(student.StudentId);

        return await MapApplicationsAsync(applications);
    }

    public async Task<List<HostelApplicationDto>> GetApplicationsAsync()
    {
        var applications = await _hostelRepository.GetApplicationsAsync();
        return await MapApplicationsAsync(applications);
    }

    public async Task<HostelApplicationDto?> GetApplicationAsync(
        int applicationId)
    {
        var application = await _hostelRepository
            .GetApplicationByIdAsync(applicationId);

        if (application is null)
        {
            return null;
        }

        var hold = await _hostelRepository
            .GetHoldByApplicationIdAsync(applicationId);

        return MapApplication(application, hold?.RoomBedId);
    }

    public async Task<bool> CancelApplicationAsync(
        int userId,
        int applicationId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var application = await _hostelRepository
            .GetApplicationByIdAsync(applicationId);

        if (application is null)
        {
            return false;
        }

        if (application.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot cancel another student's application.");
        }

        var canCancel =
            application.Status.Equals(
                "Pending",
                StringComparison.OrdinalIgnoreCase) ||
            application.Status.Equals(
                "InProgress",
                StringComparison.OrdinalIgnoreCase);

        if (!canCancel)
        {
            throw new InvalidOperationException(
                "Only Pending or InProgress applications can be cancelled.");
        }

        application.Status = "Cancelled";
        application.UpdatedAt = DateTime.UtcNow;
        await _hostelRepository.UpdateApplicationAsync(application);

        var hold = await _hostelRepository
            .GetHoldByApplicationIdAsync(applicationId);

        if (hold is not null &&
            hold.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            hold.Status = "Released";
            await _hostelRepository.UpdateHoldAsync(hold);
        }

        return true;
    }

    public async Task<bool> UpdateApplicationStatusAsync(
        int applicationId,
        UpdateHostelApplicationStatusDto dto)
    {
        var application = await _hostelRepository
            .GetApplicationByIdAsync(applicationId);

        if (application is null)
        {
            return false;
        }

        var requestedStatus = dto.Status?.Trim() ?? string.Empty;

        if (requestedStatus.Equals(
                "Approved",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Use the allocate endpoint to approve and allocate a bed.");
        }

        var isInProgress = requestedStatus.Equals(
            "InProgress",
            StringComparison.OrdinalIgnoreCase);

        var isRejected = requestedStatus.Equals(
            "Rejected",
            StringComparison.OrdinalIgnoreCase);

        if (!isInProgress && !isRejected)
        {
            throw new ArgumentException(
                "Admin status must be InProgress or Rejected.");
        }

        if (application.Status.Equals(
                "Approved",
                StringComparison.OrdinalIgnoreCase) ||
            application.Status.Equals(
                "Rejected",
                StringComparison.OrdinalIgnoreCase) ||
            application.Status.Equals(
                "Cancelled",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "This application is already in a final state.");
        }

        application.Status = isInProgress ? "InProgress" : "Rejected";
        application.UpdatedAt = DateTime.UtcNow;
        await _hostelRepository.UpdateApplicationAsync(application);

        if (isRejected)
        {
            var hold = await _hostelRepository
                .GetHoldByApplicationIdAsync(applicationId);

            if (hold is not null &&
                hold.Status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                hold.Status = "Released";
                await _hostelRepository.UpdateHoldAsync(hold);
            }

            await CreateStudentNotificationAsync(
                application.StudentId,
                "Hostel Application Rejected",
                "Your hostel application has been rejected.",
                "HostelApplication",
                application.HostelApplicationId);
        }

        return true;
    }

    public async Task<HostelAllocationDto> AllocateApplicationAsync(
        int applicationId,
        AllocateHostelApplicationDto dto)
    {
        var now = DateTime.UtcNow;
        await _hostelRepository.ExpireActiveHoldsAsync(now);

        var application = await _hostelRepository
            .GetApplicationByIdAsync(applicationId);

        if (application is null)
        {
            throw new KeyNotFoundException("Hostel application not found.");
        }

        // Hostel payment uses the shared Fees tables.
        // Validate the fee type before changing allocation state.
        var hostelFeeType = await GetHostelFeeTypeAsync();

        var canApprove =
            application.Status.Equals(
                "Pending",
                StringComparison.OrdinalIgnoreCase) ||
            application.Status.Equals(
                "InProgress",
                StringComparison.OrdinalIgnoreCase);

        if (!canApprove)
        {
            throw new InvalidOperationException(
                "Only Pending or InProgress applications can be allocated.");
        }

        if (await _hostelRepository.HasActiveAllocationAsync(
                application.StudentId))
        {
            throw new InvalidOperationException(
                "Student already has an active hostel allocation.");
        }

        var bed = await _hostelRepository.GetRoomBedByIdAsync(dto.BedId);

        if (bed is null || !bed.IsActive)
        {
            throw new KeyNotFoundException("Bed not found or inactive.");
        }

        var bedHostelId = await _hostelRepository
            .GetHostelIdByBedIdAsync(dto.BedId);

        if (bedHostelId != application.HostelId)
        {
            throw new InvalidOperationException(
                "The selected bed does not belong to the application's hostel.");
        }

        if (!bed.Status.Equals(
                "Available",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Bed is not available.");
        }

        if (await _hostelRepository.IsBedActivelyAllocatedAsync(dto.BedId))
        {
            throw new InvalidOperationException("Bed is already occupied.");
        }

        var activeHold = await _hostelRepository
            .GetActiveHoldForBedAsync(dto.BedId, now);

        if (activeHold is not null &&
            activeHold.StudentId != application.StudentId)
        {
            throw new InvalidOperationException(
                "Bed is currently held by another student.");
        }

        var allocation = new HostelAllocation
        {
            ApplicationId = application.HostelApplicationId,
            StudentId = application.StudentId,
            BedId = dto.BedId,
            AllocatedAt = now,
            Status = "Active"
        };

        var createdAllocation = await _hostelRepository
            .CreateAllocationAsync(allocation);

        bed.Status = "Occupied";
        await _hostelRepository.UpdateRoomBedAsync(bed);

        application.Status = "Approved";
        application.UpdatedAt = now;
        await _hostelRepository.UpdateApplicationAsync(application);

        var applicationHold = await _hostelRepository
            .GetHoldByApplicationIdAsync(applicationId);

        if (applicationHold is not null &&
            applicationHold.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            applicationHold.Status = "Converted";
            await _hostelRepository.UpdateHoldAsync(applicationHold);
        }

        var hostelFee = await EnsureHostelAllocationFeeAsync(
            createdAllocation,
            hostelFeeType);

        await CreateStudentNotificationAsync(
            application.StudentId,
            "Hostel Application Approved",
            $"Your hostel application has been approved and a bed has been allocated. " +
            $"Hostel fee LKR {hostelFee.Amount:0.00} is now outstanding.",
            "HostelAllocation",
            createdAllocation.HostelAllocationId);

        return MapAllocation(createdAllocation);
    }

    public async Task<List<HostelAllocationDto>> GetMyAllocationsAsync(
        int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var allocations = await _hostelRepository
            .GetAllocationsByStudentAsync(student.StudentId);

        return allocations.Select(MapAllocation).ToList();
    }

    public async Task<List<HostelAllocationDto>> GetAllocationsAsync()
    {
        var allocations = await _hostelRepository.GetAllocationsAsync();
        return allocations.Select(MapAllocation).ToList();
    }

    public async Task<HostelPaymentDto> GetAllocationPaymentAsync(
        int userId,
        int allocationId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var allocation = await _hostelRepository
            .GetAllocationByIdAsync(allocationId);

        if (allocation is null)
        {
            throw new KeyNotFoundException("Hostel allocation not found.");
        }

        if (allocation.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot view another student's hostel payment.");
        }

        var feeType = await GetHostelFeeTypeAsync();
        var studentFee = await EnsureHostelAllocationFeeAsync(
            allocation,
            feeType);

        var payments = await _feePaymentRepository
            .GetByStudentFeeIdAsync(studentFee.StudentFeeId);

        var paidPayment = payments
            .Where(x => x.PaymentStatus == PaymentStatus.Paid)
            .OrderByDescending(x => x.PaidAt)
            .FirstOrDefault();

        return MapHostelPayment(
            allocation.HostelAllocationId,
            studentFee,
            paidPayment);
    }

    public async Task<HostelPaymentDto> PayAllocationAsync(
        int userId,
        int allocationId,
        PayHostelAllocationDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var allocation = await _hostelRepository
            .GetAllocationByIdAsync(allocationId);

        if (allocation is null)
        {
            throw new KeyNotFoundException("Hostel allocation not found.");
        }

        if (allocation.StudentId != student.StudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot pay another student's hostel allocation.");
        }

        if (!allocation.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only an active hostel allocation can be paid.");
        }

        var feeType = await GetHostelFeeTypeAsync();
        var studentFee = await EnsureHostelAllocationFeeAsync(
            allocation,
            feeType);

        var existingPayments = await _feePaymentRepository
            .GetByStudentFeeIdAsync(studentFee.StudentFeeId);

        if (studentFee.Status == StudentFeeStatus.Paid ||
            existingPayments.Any(x => x.PaymentStatus == PaymentStatus.Paid))
        {
            throw new InvalidOperationException(
                "Hostel accommodation fee has already been paid.");
        }

        var paymentReference = string.IsNullOrWhiteSpace(dto.PaymentReference)
            ? $"HOSTEL-{allocationId}-{DateTime.UtcNow:yyyyMMddHHmmss}"
            : dto.PaymentReference.Trim();

        var payment = new FeePayment
        {
            StudentFeeId = studentFee.StudentFeeId,
            Amount = studentFee.Amount,
            PaymentStatus = PaymentStatus.Paid,
            PaymentReference = paymentReference,
            PaidAt = DateTime.UtcNow,
            SourcePaymentId = null,
            SourceFeeId = null,
            TargetStudentFeeId = null
        };

        var createdPayment = await _feePaymentRepository
            .CreateAsync(payment);

        studentFee.Status = StudentFeeStatus.Paid;
        await _studentFeeRepository.UpdateAsync(studentFee);

        await CreateStudentNotificationAsync(
            student.StudentId,
            "Hostel Fee Paid",
            $"Your hostel accommodation fee of LKR {studentFee.Amount:0.00} has been paid successfully.",
            "FeePayment",
            createdPayment.FeePaymentId);

        return MapHostelPayment(
            allocation.HostelAllocationId,
            studentFee,
            createdPayment);
    }

    public async Task<bool> EndAllocationAsync(int allocationId)
    {
        var allocation = await _hostelRepository
            .GetAllocationByIdAsync(allocationId);

        if (allocation is null)
        {
            return false;
        }

        if (!allocation.Status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only an active allocation can be ended.");
        }

        allocation.Status = "Ended";
        await _hostelRepository.UpdateAllocationAsync(allocation);

        var bed = await _hostelRepository.GetRoomBedByIdAsync(allocation.BedId);

        if (bed is not null)
        {
            bed.Status = "Available";
            await _hostelRepository.UpdateRoomBedAsync(bed);
        }

        await CreateStudentNotificationAsync(
            allocation.StudentId,
            "Hostel Allocation Ended",
            "Your active hostel allocation has been ended.",
            "HostelAllocation",
            allocation.HostelAllocationId);

        return true;
    }

    private async Task<FeeType> GetHostelFeeTypeAsync()
    {
        var feeType = await _feeTypeRepository
            .GetByNameAsync(HostelFeeTypeName);

        if (feeType is null || !feeType.IsActive)
        {
            throw new InvalidOperationException(
                $"Active fee type '{HostelFeeTypeName}' is not configured.");
        }

        if (feeType.Amount <= 0)
        {
            throw new InvalidOperationException(
                "Hostel accommodation fee amount must be greater than zero.");
        }

        return feeType;
    }

    private async Task<StudentFee> EnsureHostelAllocationFeeAsync(
        HostelAllocation allocation,
        FeeType feeType)
    {
        var reference = BuildHostelFeeReference(
            allocation.HostelAllocationId);

        var existingFee = await _studentFeeRepository
            .GetByReferenceAsync(
                allocation.StudentId,
                reference);

        if (existingFee is not null)
        {
            return existingFee;
        }

        var studentFee = new StudentFee
        {
            StudentId = allocation.StudentId,
            FeeTypeId = feeType.FeeTypeId,
            Amount = feeType.Amount,
            DueDate = allocation.AllocatedAt
                .AddDays(HostelFeeDueDays),
            Status = StudentFeeStatus.Outstanding,
            ExamReference = reference,
            CreatedAt = DateTime.UtcNow
        };

        return await _studentFeeRepository
            .CreateAsync(studentFee);
    }

    private static string BuildHostelFeeReference(int allocationId)
    {
        return $"HOSTEL-ALLOCATION-{allocationId}";
    }

    private static HostelPaymentDto MapHostelPayment(
        int allocationId,
        StudentFee studentFee,
        FeePayment? payment)
    {
        return new HostelPaymentDto
        {
            HostelAllocationId = allocationId,
            StudentFeeId = studentFee.StudentFeeId,
            FeePaymentId = payment?.FeePaymentId,
            Amount = studentFee.Amount,
            FeeStatus = studentFee.Status.ToString(),
            PaymentStatus = payment?.PaymentStatus.ToString() ?? "Pending",
            PaymentReference = payment?.PaymentReference,
            DueDate = studentFee.DueDate,
            PaidAt = payment?.PaidAt
        };
    }

    private async Task<Student> GetStudentByUserIdAsync(int userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student is null || !student.IsActive)
        {
            throw new UnauthorizedAccessException(
                "An active student profile is required.");
        }

        return student;
    }

    private async Task CreateStudentNotificationAsync(
        int studentId,
        string title,
        string message,
        string referenceType,
        int referenceId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student?.UserId is null)
        {
            return;
        }

        await _notificationService.CreateAsync(
            new NotificationCreateDto
            {
                UserId = student.UserId.Value,
                Title = title,
                Message = message,
                ReferenceType = referenceType,
                ReferenceId = referenceId
            });
    }

    private async Task<List<HostelApplicationDto>> MapApplicationsAsync(
        IEnumerable<HostelApplication> applications)
    {
        var result = new List<HostelApplicationDto>();

        foreach (var application in applications)
        {
            var hold = await _hostelRepository
                .GetHoldByApplicationIdAsync(application.HostelApplicationId);

            result.Add(MapApplication(application, hold?.RoomBedId));
        }

        return result;
    }

    private static HostelHoldDto MapHold(HostelRoomHold hold)
    {
        return new HostelHoldDto
        {
            HostelRoomHoldId = hold.HostelRoomHoldId,
            StudentId = hold.StudentId,
            RoomBedId = hold.RoomBedId,
            ApplicationId = hold.ApplicationId,
            HeldAt = hold.HeldAt,
            ExpiresAt = hold.ExpiresAt,
            Status = hold.Status
        };
    }

    private static HostelApplicationDto MapApplication(
        HostelApplication application,
        int? requestedBedId)
    {
        return new HostelApplicationDto
        {
            HostelApplicationId = application.HostelApplicationId,
            StudentId = application.StudentId,
            HostelId = application.HostelId,
            RequestedBedId = requestedBedId,
            District = application.District,
            Province = application.Province,
            Reason = application.Reason,
            Status = application.Status,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    private static HostelAllocationDto MapAllocation(
        HostelAllocation allocation)
    {
        return new HostelAllocationDto
        {
            HostelAllocationId = allocation.HostelAllocationId,
            ApplicationId = allocation.ApplicationId,
            StudentId = allocation.StudentId,
            BedId = allocation.BedId,
            AllocatedAt = allocation.AllocatedAt,
            Status = allocation.Status
        };
    }
}
