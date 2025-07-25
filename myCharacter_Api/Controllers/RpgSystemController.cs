using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myCharacter.Data;


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
        [HttpGet("{id}")]
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
    }
}