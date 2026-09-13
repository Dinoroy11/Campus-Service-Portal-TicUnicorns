using System.Security.Claims;
using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Controllers;

[ApiController]
[Route("api/canteen")]
[Authorize]
public class CanteenController : ControllerBase
{
    private readonly ICanteenService _canteenService;

    public CanteenController(ICanteenService canteenService)
    {
        _canteenService = canteenService;
    }

    // =========================================================
    // CANTEENS
    // =========================================================

    [HttpGet("canteens")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCanteens()
    {
        return Ok(await _canteenService.GetCanteensAsync());
    }

    [HttpGet("my/eligibility")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyEligibility()
    {
        return Ok(await _canteenService
            .GetMyEligibilityAsync(GetCurrentUserId()));
    }

    [HttpGet("my/canteens")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCanteens()
    {
        try
        {
            return Ok(await _canteenService.GetMyCanteensAsync(GetCurrentUserId()));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("canteens")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCanteen([FromBody] CreateCanteenDto dto)
    {
        try
        {
            var result = await _canteenService.CreateCanteenAsync(dto);
            return Created($"api/canteen/canteens/{result.CanteenId}", result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // =========================================================
    // MENU
    // =========================================================

    [HttpGet("canteens/{canteenId:int}/menu")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetMenu(int canteenId)
    {
        try
        {
            return Ok(await _canteenService.GetMenuAsync(canteenId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("menu")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMenuItem(
        [FromBody] CreateCanteenMenuItemDto dto)
    {
        try
        {
            var result = await _canteenService.CreateMenuItemAsync(dto);
            return Created($"api/canteen/menu/{result.MenuItemId}", result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // =========================================================
    // MEAL PLANS
    // =========================================================

    [HttpGet("packages")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetPackages([FromQuery] int? canteenId)
    {
        return Ok(await _canteenService.GetActivePackagesAsync(canteenId));
    }

    [HttpPost("packages")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePackage(
        [FromBody] CreateMealPackageDto dto)
    {
        try
        {
            var result = await _canteenService.CreatePackageAsync(dto);
            return Created($"api/canteen/packages/{result.MealPackageId}", result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // =========================================================
    // STUDENT SUBSCRIPTIONS
    // =========================================================

    [HttpPost("subscriptions")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateMealSubscriptionDto dto)
    {
        try
        {
            var result = await _canteenService.CreateSubscriptionAsync(
                GetCurrentUserId(),
                dto);

            return Created(
                $"api/canteen/subscriptions/{result.MealSubscriptionId}",
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("subscriptions/{mealSubscriptionId:int}/pay")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> PaySubscription(
        int mealSubscriptionId,
        [FromBody] SimulateMealSubscriptionPaymentDto dto)
    {
        try
        {
            return Ok(await _canteenService.PaySubscriptionAsync(
                GetCurrentUserId(),
                mealSubscriptionId,
                dto));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("my/subscription")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyActiveSubscription()
    {
        var result = await _canteenService
            .GetMyActiveSubscriptionAsync(GetCurrentUserId());

        if (result == null)
        {
            return NotFound(new { message = "No active meal subscription found." });
        }

        return Ok(result);
    }

    [HttpGet("my/subscriptions")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMySubscriptions()
    {
        return Ok(await _canteenService
            .GetMySubscriptionsAsync(GetCurrentUserId()));
    }

    [HttpGet("subscriptions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        return Ok(await _canteenService.GetAllSubscriptionsAsync());
    }

    [HttpGet("students/{studentId:int}/subscriptions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStudentSubscriptions(int studentId)
    {
        return Ok(await _canteenService.GetStudentSubscriptionsAsync(studentId));
    }

    // =========================================================
    // ABSENCE
    // =========================================================

    [HttpPost("absences")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ReportAbsence(
        [FromBody] ReportMealAbsenceDto dto)
    {
        try
        {
            await _canteenService.ReportAbsenceAsync(GetCurrentUserId(), dto);
            return Ok(new { message = "Meal absence reported successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("subscriptions/{mealSubscriptionId:int}/absences")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAbsences(int mealSubscriptionId)
    {
        try
        {
            return Ok(await _canteenService.GetAbsencesAsync(
                GetCurrentUserId(),
                mealSubscriptionId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // =========================================================
    // MEAL USAGE
    // =========================================================

    [HttpGet("my/usage")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyMealUsage(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        return Ok(await _canteenService.GetMyMealUsageAsync(
            GetCurrentUserId(),
            fromDate,
            toDate));
    }

    [HttpPut("usage/collect")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CollectMeal(
        [FromQuery] int mealSubscriptionId,
        [FromQuery] DateTime mealDate,
        [FromQuery] MealType mealType)
    {
        try
        {
            return Ok(await _canteenService.CollectMealAsync(
                mealSubscriptionId,
                mealDate,
                mealType));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}
