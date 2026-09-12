using System.Security.Claims;
using CampusServicePortal.Modules.Fees.DTOs;
using CampusServicePortal.Modules.Fees.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Fees.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeesController : ControllerBase
{
    private readonly IFeesService _feesService;

    public FeesController(IFeesService feesService)
    {
        _feesService = feesService;
    }

    // =========================================================
    // Fee Types
    // =========================================================

    [HttpGet("types")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetAllFeeTypes()
        => Ok(await _feesService.GetAllFeeTypesAsync());

    [HttpGet("types/{feeTypeId:int}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetFeeTypeById(int feeTypeId)
    {
        var result = await _feesService.GetFeeTypeByIdAsync(feeTypeId);
        return result == null
            ? NotFound(new { message = "Fee type not found." })
            : Ok(result);
    }

    [HttpPost("types")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFeeType([FromBody] CreateFeeTypeDto dto)
    {
        try
        {
            return Ok(await _feesService.CreateFeeTypeAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("types/{feeTypeId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFeeType(
        int feeTypeId,
        [FromBody] CreateFeeTypeDto dto)
    {
        try
        {
            await _feesService.UpdateFeeTypeAsync(feeTypeId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("types/{feeTypeId:int}/active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetFeeTypeActive(
        int feeTypeId,
        [FromBody] SetFeeTypeActiveDto dto)
    {
        try
        {
            await _feesService.SetFeeTypeActiveAsync(feeTypeId, dto.IsActive);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // =========================================================
    // Admin Student Fee Management
    // =========================================================

    [HttpGet("student-fees")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStudentFees()
        => Ok(await _feesService.GetAllStudentFeesAsync());

    [HttpGet("student-fees/{studentFeeId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStudentFeeById(int studentFeeId)
    {
        var result = await _feesService.GetStudentFeeByIdAsync(studentFeeId);
        return result == null
            ? NotFound(new { message = "Student fee not found." })
            : Ok(result);
    }

    [HttpGet("students/{studentId:int}/fees")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStudentFeesByStudentId(int studentId)
    {
        try
        {
            return Ok(await _feesService.GetStudentFeesByStudentIdAsync(studentId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("student-fees")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateStudentFee([FromBody] CreateStudentFeeDto dto)
    {
        try
        {
            return Ok(await _feesService.CreateStudentFeeAsync(dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("student-fees/{studentFeeId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStudentFee(
        int studentFeeId,
        [FromBody] CreateStudentFeeDto dto)
    {
        try
        {
            await _feesService.UpdateStudentFeeAsync(studentFeeId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // =========================================================
    // Student Self-Service
    // =========================================================

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyFees()
    {
        try
        {
            return Ok(await _feesService.GetMyFeesAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("my/{studentFeeId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyFeeById(int studentFeeId)
    {
        try
        {
            var result = await _feesService.GetMyFeeByIdAsync(GetCurrentUserId(), studentFeeId);
            return result == null
                ? NotFound(new { message = "Fee not found." })
                : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpGet("my/payments")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyPayments()
    {
        try
        {
            return Ok(await _feesService.GetMyPaymentsAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost("my/{studentFeeId:int}/pay")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> PayMyFee(
        int studentFeeId,
        [FromBody] SimulateFeePaymentDto dto)
    {
        try
        {
            return Ok(await _feesService.PayMyFeeAsync(
                GetCurrentUserId(),
                studentFeeId,
                dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // =========================================================
    // Admin Payment View + Carry Forward
    // =========================================================

    [HttpGet("payments")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPayments()
        => Ok(await _feesService.GetAllPaymentsAsync());

    [HttpGet("payments/{feePaymentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPaymentById(int feePaymentId)
    {
        var result = await _feesService.GetPaymentByIdAsync(feePaymentId);
        return result == null
            ? NotFound(new { message = "Payment not found." })
            : Ok(result);
    }

    [HttpGet("student-fees/{studentFeeId:int}/payments")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPaymentsByStudentFeeId(int studentFeeId)
    {
        try
        {
            return Ok(await _feesService.GetPaymentsByStudentFeeIdAsync(studentFeeId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("carry-forward")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CarryForward([FromBody] CarryForwardFeeDto dto)
    {
        try
        {
            return Ok(await _feesService.CarryForwardExamFeeAsync(
                GetCurrentUserId(),
                dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // =========================================================
    // Refunds
    // =========================================================

    [HttpGet("refunds")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRefunds()
        => Ok(await _feesService.GetAllRefundRequestsAsync());

    [HttpGet("refunds/{refundRequestId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRefundById(int refundRequestId)
    {
        var result = await _feesService.GetRefundRequestByIdAsync(refundRequestId);
        return result == null
            ? NotFound(new { message = "Refund request not found." })
            : Ok(result);
    }

    [HttpGet("my/refunds")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyRefunds()
    {
        try
        {
            return Ok(await _feesService.GetMyRefundRequestsAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost("my/payments/{paymentId:int}/refund")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestRefund(
        int paymentId,
        [FromBody] CreateRefundRequestDto dto)
    {
        try
        {
            return Ok(await _feesService.CreateMyRefundRequestAsync(
                GetCurrentUserId(),
                paymentId,
                dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("refunds/{refundRequestId:int}/decision")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReviewRefund(
        int refundRequestId,
        [FromBody] ReviewRefundRequestDto dto)
    {
        try
        {
            return Ok(await _feesService.ReviewRefundRequestAsync(
                GetCurrentUserId(),
                refundRequestId,
                dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refunds/{refundRequestId:int}/process")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ProcessRefund(int refundRequestId)
    {
        try
        {
            return Ok(await _feesService.ProcessRefundAsync(
                GetCurrentUserId(),
                refundRequestId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
