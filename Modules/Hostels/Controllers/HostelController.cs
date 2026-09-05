using CampusServicePortal.Modules.Hostels.DTOs;
using CampusServicePortal.Modules.Hostels.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Hostels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HostelController : ControllerBase
{
    private readonly IHostelService _hostelService;

    public HostelController(IHostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // GET: api/Hostel
    [HttpGet]
    public async Task<IActionResult> GetHostels()
    {
        var hostels = await _hostelService.GetHostelsAsync();

        return Ok(hostels);
    }

    // GET: api/Hostel/{hostelId}/blueprint
    [HttpGet("{hostelId}/blueprint")]
    public async Task<IActionResult> GetHostelBlueprint(int hostelId)
    {
        var hostel = await _hostelService
            .GetHostelBlueprintAsync(hostelId);

        if (hostel == null)
        {
            return NotFound(new
            {
                message = "Hostel not found."
            });
        }

        return Ok(hostel);
    }

    // POST: api/Hostel
    [HttpPost]
    public async Task<IActionResult> CreateHostel(
        [FromBody] CreateHostelDto dto)
    {
        var hostel = await _hostelService
            .CreateHostelAsync(dto);

        return Ok(hostel);
    }

    // POST: api/Hostel/floors
    [HttpPost("floors")]
    public async Task<IActionResult> CreateFloor(
        [FromBody] CreateFloorDto dto)
    {
        await _hostelService.CreateFloorAsync(dto);

        return Ok(new
        {
            message = "Floor created successfully."
        });
    }

    // POST: api/Hostel/rooms
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(
        [FromBody] CreateRoomDto dto)
    {
        await _hostelService.CreateRoomAsync(dto);

        return Ok(new
        {
            message = "Room created successfully."
        });
    }

    // POST: api/Hostel/beds
    [HttpPost("beds")]
    public async Task<IActionResult> CreateRoomBed(
        [FromBody] CreateRoomBedDto dto)
    {
        await _hostelService.CreateRoomBedAsync(dto);

        return Ok(new
        {
            message = "Bed created successfully."
        });
    }

    // PUT: api/Hostel/{hostelId}
    [HttpPut("{hostelId}")]
    public async Task<IActionResult> UpdateHostel(
        int hostelId,
        [FromBody] UpdateHostelDto dto)
    {
        var updated = await _hostelService
            .UpdateHostelAsync(hostelId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Hostel not found."
            });
        }

        return Ok(new
        {
            message = "Hostel updated successfully."
        });
    }

    // PUT: api/Hostel/floors/{floorId}
    [HttpPut("floors/{floorId}")]
    public async Task<IActionResult> UpdateFloor(
        int floorId,
        [FromBody] UpdateFloorDto dto)
    {
        var updated = await _hostelService
            .UpdateFloorAsync(floorId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Floor not found."
            });
        }

        return Ok(new
        {
            message = "Floor updated successfully."
        });
    }

    // PUT: api/Hostel/rooms/{roomId}
    [HttpPut("rooms/{roomId}")]
    public async Task<IActionResult> UpdateRoom(
        int roomId,
        [FromBody] UpdateRoomDto dto)
    {
        var updated = await _hostelService
            .UpdateRoomAsync(roomId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Room not found."
            });
        }

        return Ok(new
        {
            message = "Room updated successfully."
        });
    }

    // PUT: api/Hostel/beds/{bedId}
    [HttpPut("beds/{bedId}")]
    public async Task<IActionResult> UpdateRoomBed(
        int bedId,
        [FromBody] UpdateRoomBedDto dto)
    {
        var updated = await _hostelService
            .UpdateRoomBedAsync(bedId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Bed not found."
            });
        }

        return Ok(new
        {
            message = "Bed updated successfully."
        });
    }
}