using CampusServicePortal.Modules.SystemSettings.DTOs;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.SystemSettings.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _service;

        public SystemSettingController(ISystemSettingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _service.GetAllSystemSettingsAsync();
            return Ok(settings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var setting = await _service.GetSystemSettingByIdAsync(id);

            if (setting == null)
                return NotFound();

            return Ok(setting);
        }

        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var setting = await _service.GetSystemSettingByKeyAsync(key);

            if (setting == null)
                return NotFound();

            return Ok(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSystemSettingDto dto)
        {
            var setting = await _service.CreateSystemSettingAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = setting.SettingId },
                setting);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            CreateSystemSettingDto dto)
        {
            var setting = await _service.UpdateSystemSettingAsync(id, dto);

            if (setting == null)
                return NotFound();

            return Ok(setting);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteSystemSettingAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}