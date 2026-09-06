using CampusServicePortal.Modules.Fees.DTOs;
using CampusServicePortal.Modules.Fees.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Fees.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> GetAllFeeTypes()
    {
        var result = await _feesService.GetAllFeeTypesAsync();

        return Ok(result);
    }

    [HttpGet("types/{feeTypeId}")]
    public async Task<IActionResult> GetFeeTypeById(int feeTypeId)
    {
        var result =
            await _feesService.GetFeeTypeByIdAsync(feeTypeId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("types")]
    public async Task<IActionResult> CreateFeeType(
        [FromBody] CreateFeeTypeDto dto)
    {
        var result =
            await _feesService.CreateFeeTypeAsync(dto);

        return CreatedAtAction(
            nameof(GetFeeTypeById),
            new { feeTypeId = result.FeeTypeId },
            result);
    }

    [HttpPut("types/{feeTypeId}")]
    public async Task<IActionResult> UpdateFeeType(
        int feeTypeId,
        [FromBody] CreateFeeTypeDto dto)
    {
        try
        {
            await _feesService.UpdateFeeTypeAsync(
                feeTypeId,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    // =========================================================
    // Student Fees
    // =========================================================

    [HttpGet("student-fees")]
    public async Task<IActionResult> GetAllStudentFees()
    {
        var result =
            await _feesService.GetAllStudentFeesAsync();

        return Ok(result);
    }

    [HttpGet("student-fees/{studentFeeId}")]
    public async Task<IActionResult> GetStudentFeeById(
        int studentFeeId)
    {
        var result =
            await _feesService.GetStudentFeeByIdAsync(
                studentFeeId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("students/{studentId}/fees")]
    public async Task<IActionResult> GetStudentFeesByStudentId(
        int studentId)
    {
        var result =
            await _feesService
                .GetStudentFeesByStudentIdAsync(studentId);

        return Ok(result);
    }

    [HttpPost("student-fees")]
    public async Task<IActionResult> CreateStudentFee(
        [FromBody] CreateStudentFeeDto dto)
    {
        try
        {
            var result =
                await _feesService.CreateStudentFeeAsync(dto);

            return CreatedAtAction(
                nameof(GetStudentFeeById),
                new { studentFeeId = result.StudentFeeId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("student-fees/{studentFeeId}")]
    public async Task<IActionResult> UpdateStudentFee(
        int studentFeeId,
        [FromBody] CreateStudentFeeDto dto)
    {
        try
        {
            await _feesService.UpdateStudentFeeAsync(
                studentFeeId,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // Fee Payments
    // =========================================================

    [HttpGet("payments")]
    public async Task<IActionResult> GetAllPayments()
    {
        var result =
            await _feesService.GetAllPaymentsAsync();

        return Ok(result);
    }

    [HttpGet("payments/{feePaymentId}")]
    public async Task<IActionResult> GetPaymentById(
        int feePaymentId)
    {
        var result =
            await _feesService.GetPaymentByIdAsync(
                feePaymentId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("student-fees/{studentFeeId}/payments")]
    public async Task<IActionResult> GetPaymentsByStudentFeeId(
        int studentFeeId)
    {
        var result =
            await _feesService
                .GetPaymentsByStudentFeeIdAsync(studentFeeId);

        return Ok(result);
    }

    [HttpPost("payments")]
    public async Task<IActionResult> CreatePayment(
        [FromBody] FeePaymentDto dto)
    {
        try
        {
            var result =
                await _feesService.CreatePaymentAsync(dto);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { feePaymentId = result.FeePaymentId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // Refund Requests
    // =========================================================

    [HttpGet("refunds")]
    public async Task<IActionResult> GetAllRefundRequests()
    {
        var result =
            await _feesService.GetAllRefundRequestsAsync();

        return Ok(result);
    }

    [HttpGet("refunds/{refundRequestId}")]
    public async Task<IActionResult> GetRefundRequestById(
        int refundRequestId)
    {
        var result =
            await _feesService.GetRefundRequestByIdAsync(
                refundRequestId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("payments/{paymentId}/refund")]
    public async Task<IActionResult> GetRefundRequestByPaymentId(
        int paymentId)
    {
        var result =
            await _feesService
                .GetRefundRequestByPaymentIdAsync(paymentId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("refunds")]
    public async Task<IActionResult> CreateRefundRequest(
        [FromBody] RefundRequestDto dto)
    {
        try
        {
            var result =
                await _feesService
                    .CreateRefundRequestAsync(dto);

            return CreatedAtAction(
                nameof(GetRefundRequestById),
                new { refundRequestId = result.RefundRequestId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("refunds/{refundRequestId}")]
    public async Task<IActionResult> UpdateRefundRequest(
        int refundRequestId,
        [FromBody] RefundRequestDto dto)
    {
        try
        {
            await _feesService.UpdateRefundRequestAsync(
                refundRequestId,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}