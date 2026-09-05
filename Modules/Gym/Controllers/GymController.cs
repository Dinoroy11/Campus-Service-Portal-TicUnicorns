using CampusServicePortal.Modules.Gym.DTOs;
using CampusServicePortal.Modules.Gym.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicePortal.Modules.Gym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymService;

        public GymController(IGymService gymService)
        {
            _gymService = gymService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GymDto>>> GetAllGyms()
        {
            var gyms = await _gymService.GetAllGymsAsync();

            return Ok(gyms);
        }

        [HttpGet("{gymId}")]
        public async Task<ActionResult<GymDto>> GetGymById(int gymId)
        {
            var gym = await _gymService.GetGymByIdAsync(gymId);

            if (gym == null)
            {
                return NotFound();
            }

            return Ok(gym);
        }

        [HttpPost]
        public async Task<ActionResult<GymDto>> CreateGym(
            CreateGymDto gymDto)
        {
            var gym = await _gymService.CreateGymAsync(gymDto);

            return Ok(gym);
        }

        [HttpPut("{gymId}")]
        public async Task<ActionResult<GymDto>> UpdateGym(
            int gymId,
            CreateGymDto gymDto)
        {
            var gym = await _gymService.UpdateGymAsync(gymId, gymDto);

            if (gym == null)
            {
                return NotFound();
            }

            return Ok(gym);
        }

        [HttpDelete("{gymId}")]
        public async Task<IActionResult> DeleteGym(int gymId)
        {
            var deleted = await _gymService.DeleteGymAsync(gymId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}