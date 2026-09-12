using System.Security.Claims;
using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly ICertificatesService _certificatesService;

    public CertificatesController(ICertificatesService certificatesService)
    {
        _certificatesService = certificatesService;
    }

    // =========================================================
    // ADMIN
    // =========================================================

    // GET: api/Certificates
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _certificatesService.GetAllAsync());
    }

    // GET: api/Certificates/1
    [HttpGet("{certificateId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int certificateId)
    {
        var certificate = await _certificatesService.GetByIdAsync(certificateId);

        return certificate == null
            ? NotFound(new { message = "Certificate request not found." })
            : Ok(certificate);
    }

    // PUT: api/Certificates/1/status
    [HttpPut("{certificateId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int certificateId,
        [FromBody] UpdateCertificateStatusDto dto)
    {
        try
        {
            var result = await _certificatesService.UpdateStatusAsync(
                certificateId,
                dto);

            return Ok(result);
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
    // STUDENT
    // =========================================================

    // GET: api/Certificates/my
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMy()
    {
        try
        {
            return Ok(await _certificatesService.GetMyAsync(GetCurrentUserId()));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }

    // GET: api/Certificates/my/1
    [HttpGet("my/{certificateId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyById(int certificateId)
    {
        try
        {
            var certificate = await _certificatesService.GetMyByIdAsync(
                GetCurrentUserId(),
                certificateId);

            return certificate == null
                ? NotFound(new { message = "Certificate request not found." })
                : Ok(certificate);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }

    // POST: api/Certificates
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create([FromBody] CreateCertificateDto dto)
    {
        try
        {
            var certificate = await _certificatesService.CreateAsync(
                GetCurrentUserId(),
                dto);

            return Ok(certificate);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
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

    // PUT: api/Certificates/1
    // Student can edit certificate type/reason only while request is Pending.
    [HttpPut("{certificateId:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateMy(
        int certificateId,
        [FromBody] UpdateCertificateDto dto)
    {
        try
        {
            var result = await _certificatesService.UpdateMyAsync(
                GetCurrentUserId(),
                certificateId,
                dto);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = ex.Message });
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

    private int GetCurrentUserId()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
