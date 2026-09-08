
using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Enums;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Canteen.Controllers;

[ApiController]
[Route("api/canteen")]
public class CanteenController : ControllerBase
{
    private readonly ICanteenService _canteenService;

    public CanteenController(ICanteenService canteenService)
    {
        _canteenService = canteenService;
    }


    // =========================================================
    // MEAL PACKAGES
    // =========================================================

    // GET: api/canteen/packages
    [HttpGet("packages")]
    public async Task<IActionResult> GetPackages()
    {
        var packages =
            await _canteenService.GetActivePackagesAsync();

        return Ok(packages);
    }


    // POST: api/canteen/packages
    [HttpPost("packages")]
    public async Task<IActionResult> CreatePackage(
        [FromBody] CreateMealPackageDto dto)
    {
        try
        {
            var result =
                await _canteenService.CreatePackageAsync(dto);

            return Created(
                $"api/canteen/packages/{result.MealPackageId}",
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // SUBSCRIPTIONS
    // =========================================================

    // POST: api/canteen/subscriptions
    [HttpPost("subscriptions")]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateMealSubscriptionDto dto)
    {
        try
        {
            var result =
                await _canteenService
                    .CreateSubscriptionAsync(dto);

            return Created(
                $"api/canteen/subscriptions/{result.MealSubscriptionId}",
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }


    // GET: api/canteen/students/10/subscription
    [HttpGet("students/{studentId:int}/subscription")]
    public async Task<IActionResult> GetActiveSubscription(
        int studentId)
    {
        var result =
            await _canteenService
                .GetActiveSubscriptionAsync(studentId);

        if (result == null)
        {
            return NotFound(new
            {
                message =
                    "No active meal subscription found for this student."
            });
        }

        return Ok(result);
    }


    // GET: api/canteen/students/10/subscriptions
    [HttpGet("students/{studentId:int}/subscriptions")]
    public async Task<IActionResult> GetStudentSubscriptions(
        int studentId)
    {
        var result =
            await _canteenService
                .GetStudentSubscriptionsAsync(studentId);

        return Ok(result);
    }


    // =========================================================
    // ABSENCE
    // =========================================================

    // POST: api/canteen/absences
    [HttpPost("absences")]
    public async Task<IActionResult> ReportAbsence(
        [FromBody] ReportMealAbsenceDto dto)
    {
        try
        {
            await _canteenService.ReportAbsenceAsync(dto);

            return Ok(new
            {
                message =
                    "Meal absence reported successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }


    // GET: api/canteen/subscriptions/5/absences
    [HttpGet("subscriptions/{mealSubscriptionId:int}/absences")]
    public async Task<IActionResult> GetAbsences(
        int mealSubscriptionId)
    {
        var result =
            await _canteenService
                .GetAbsencesAsync(mealSubscriptionId);

        return Ok(result);
    }


    // =========================================================
    // MEAL USAGE
    // =========================================================

    // GET: api/canteen/students/10/usage
    [HttpGet("students/{studentId:int}/usage")]
    public async Task<IActionResult> GetStudentMealUsage(
        int studentId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result =
            await _canteenService
                .GetStudentMealUsageAsync(
                    studentId,
                    fromDate,
                    toDate);

        return Ok(result);
    }


    // PUT: api/canteen/usage/collect
    [HttpPut("usage/collect")]
    public async Task<IActionResult> CollectMeal(
        [FromQuery] int mealSubscriptionId,
        [FromQuery] DateTime mealDate,
        [FromQuery] MealType mealType)
    {
        try
        {
            var result =
                await _canteenService.CollectMealAsync(
                    mealSubscriptionId,
                    mealDate,
                    mealType);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}
