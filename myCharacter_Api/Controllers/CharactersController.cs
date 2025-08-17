using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myCharacter.Data;
using myCharacter.DTOs.Characters;
using myCharacter.DTOs;
using myCharacter.Models;
using myCharacter.Helpers;

namespace myCharacter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CharactersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CharacterDto>>> GetAllCharacters([FromQuery] QueryParameters queryParameters)
        {
            var userId = GetUserId();
            var term = queryParameters.SearchTerm;
            var query = _context.Characters.Where(c => c.UserId == userId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(c =>
                    c.Name.Contains(term) ||
                    (c.Race != null && c.Race.Contains(term)) ||
                    (c.Class != null && c.Class.Contains(term))
                    );
            }
            query = query.ApplySorting(queryParameters);
            var charactersDtoQuery = query.Select(c => new CharacterDto
            {
                Id = c.Id,
                Name = c.Name,
                Race = c.Race,
                Class = c.Class,
                Level = c.Level,
                RpgSystemId = c.RpgSystemId,
                CampaignId = c.CampaignId
            });
            var pagedResult = await charactersDtoQuery.ToPagedResultAsync(queryParameters);
            return Ok(pagedResult);
        }

        [HttpGet("{id}", Name = "GetCharacterById")]
        public async Task<ActionResult<CharacterDetailDto>> GetCharacterById(Guid id)
        {
            var userId = GetUserId();
            var character = await _context.Characters.Include(c => c.RpgSystem).FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (character == null) return NotFound();
            return Ok(MapToDetailDto(character));
        }

        [HttpPost]
        public async Task<ActionResult<CharacterDetailDto>> CreateCharacter(CreateCharacterDto createCharacter)
        {
            var userId = GetUserId();
            var systemExists = await _context.RpgSystems.AnyAsync(s => s.Id == createCharacter.RpgSystemId);

            if (!systemExists) { return BadRequest("Invalid RPG System ID."); }

            if (createCharacter.CampaignId != null)
            {
                var campaingIsValid = await _context.Campaigns.AnyAsync(c => c.Id == createCharacter.CampaignId && c.UserId == userId);

                if (!campaingIsValid)
                {
                    return BadRequest("Invalid Campaign ID or you are not the owner of the campaign.");
                }
            }

            var character = new Character
            {
                Name = createCharacter.Name,
                Race = createCharacter.Race,
                Class = createCharacter.Class,
                Level = createCharacter.Level,
                SystemSpecificData = createCharacter.SystemSpecificData,
                RpgSystemId = createCharacter.RpgSystemId,
                UserId = userId,
                CampaignId = createCharacter.CampaignId
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, MapToDetailDto(character));
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<CharacterDetailDto>> DuplicateCharacter(Guid id)
        {
            var userId = GetUserId();
            var originalCharacter = await _context.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (originalCharacter == null) return NotFound();

            var newCharacter = new Character()
            {
                Name = originalCharacter.Name + " (Copy)",
                Race = originalCharacter.Race,
                Class = originalCharacter.Class,
                Level = originalCharacter.Level,
                SystemSpecificData = originalCharacter.SystemSpecificData,
                RpgSystemId = originalCharacter.RpgSystemId,
                UserId = userId
            };

            _context.Characters.Add(newCharacter);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCharacterById), new { id = newCharacter.Id }, MapToDetailDto(newCharacter));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CharacterDetailDto>> UpdateCharacter(Guid id, UpdateCharacterDto updateCharacter)
        {
            var userId = GetUserId();
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (character == null) return NotFound();

            if (updateCharacter.CampaignId != null)
            {
                var campaingIsValid = await _context.Campaigns.AnyAsync(c => c.Id == updateCharacter.CampaignId && c.UserId == userId);

                if (!campaingIsValid)
                {
                    return BadRequest("Invalid Campaign ID or you are not the owner of the campaign.");
                }
            }

            character.Name = updateCharacter.Name;
            character.Race = updateCharacter.Race;
            character.Class = updateCharacter.Class;
            character.Level = updateCharacter.Level;
            character.SystemSpecificData = updateCharacter.SystemSpecificData;
            character.CampaignId = updateCharacter.CampaignId;

            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
            return Ok(MapToDetailDto(character));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(Guid id)
        {
            var userId = GetUserId();
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);

            if (character == null) return NotFound();
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private CharacterDetailDto MapToDetailDto(Character character)
        {
            return new CharacterDetailDto
            {
                Id = character.Id,
                Name = character.Name,
                Race = character.Race,
                Class = character.Class,
                Level = character.Level,
                SystemSpecificData = character.SystemSpecificData,
                RpgSystemId = character.RpgSystemId,
                CampaignId = character.CampaignId,
                CharacterSheetTemplate = character.RpgSystem?.CharacterSheetTemplate
            };
        }
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User ID not found in token.");
            return userId;
        }
    }
}