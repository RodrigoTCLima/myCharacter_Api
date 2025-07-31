using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myCharacter.Data;
using myCharacter.DTOs.RpgSystems;
using myCharacter.Models;


namespace myCharacter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RpgSystemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RpgSystemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RpgSystem 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RpgSystemDto>>> GetAllRpgSystems()
        {
            var rpgSystems = await _context.RpgSystems.Select(s => new RpgSystemDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            }).ToListAsync();

            return Ok(rpgSystems);
        }

        // GET: api/RpgSystem/{id}
        [HttpGet("{id}", Name = "GetRpgSystemById")]

        public async Task<ActionResult<RpgSystemDetailDto>> GetRpgSystemById(int id)
        {
            var rpgSystem = await _context.RpgSystems
                .Where(s => s.Id == id)
                .Select(s => new RpgSystemDetailDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CharacterSheetTemplate = s.CharacterSheetTemplate
                }
                ).FirstOrDefaultAsync();

            if (rpgSystem == null)
            {
                return NotFound();
            }

            return Ok(rpgSystem);
        }

        // POST: api/RpgSystem
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<RpgSystemDetailDto>> CreateRpgSystem(CreateRpgSystemDto createRpgSystemDto)
        {
            var rpgSystem = new RpgSystem
            {
                Name = createRpgSystemDto.Name,
                Description = createRpgSystemDto.Description,
                CharacterSheetTemplate = createRpgSystemDto.CharacterSheetTemplate
            };

            _context.RpgSystems.Add(rpgSystem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRpgSystemById), new { id = rpgSystem.Id }, MapToDetailDto(rpgSystem));
        }

        // PUT: api/RpgSystem/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RpgSystemDetailDto>> UpdateRpgSystem(int id, CreateRpgSystemDto updateRpgSystemDto)
        {
            var rpgSystem = await _context.RpgSystems.FindAsync(id);
            if (rpgSystem == null)
            {
                return NotFound();
            }
            rpgSystem.Name = updateRpgSystemDto.Name;
            rpgSystem.CharacterSheetTemplate = updateRpgSystemDto.CharacterSheetTemplate;
            rpgSystem.Description = updateRpgSystemDto.Description;

            _context.RpgSystems.Update(rpgSystem);
            await _context.SaveChangesAsync();

            return Ok(MapToDetailDto(rpgSystem));
        }

        // DELETE: api/RpgSystem/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRpgSystem(int id)
        {
            var rpgSystem = await _context.RpgSystems.FindAsync(id);

            if (rpgSystem == null) return NotFound();

            _context.RpgSystems.Remove(rpgSystem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private RpgSystemDetailDto MapToDetailDto(RpgSystem rpgSystem)
        {
            return new RpgSystemDetailDto
            {
                Id = rpgSystem.Id,
                Name = rpgSystem.Name,
                Description = rpgSystem.Description,
                CharacterSheetTemplate = rpgSystem.CharacterSheetTemplate
            };
        }
    }
}