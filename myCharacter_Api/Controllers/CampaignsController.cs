using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myCharacter.Data;
using myCharacter.Models;

namespace myCharacter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CampaignsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CampaignsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Campaigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampaignDto>>> GetAllCampaings()
        {
            var user = GetUserId();
            var campaigns = await _context.Campaigns
                                    .Where(s => s.UserId == user)
                                    .Select(c => new CampaignDto
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                        Description = c.Description,
                                        RpgSystemId = c.RpgSystemId
                                    }).ToListAsync();
            return Ok(campaigns);
        }

        // Get: api/Campaigns/{id}
        [HttpGet("{id}", Name = "GetCampaignById")]
        public async Task<ActionResult<CampaignDetailDto>> GetCampaignById(Guid id)
        {
            var userId = GetUserId();
            var campaign = await _context.Campaigns.Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();

            if (campaign == null) return NotFound();
            return Ok(MapToDetailDto(campaign));
        }

        // POST: api/Campaigns
        [HttpPost]
        public async Task<ActionResult<CampaignDetailDto>> CreateCampaign(CreateCampaignDto createDto)
        {
            var userId = GetUserId();
            var newCampaign = new Campaign
            {
                UserId = userId,
                Name = createDto.Name,
                Description = createDto.Description,
                RpgSystemId = createDto.RpgSystemId
            };
            _context.Campaigns.Add(newCampaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaignById), new { id = newCampaign.Id }, MapToDetailDto(newCampaign));
        }

        // PUT: api/Campaigns/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CampaignDetailDto>> UpdateCampaign(Guid id, UpdateCampaignDto updateCampaignDto)
        {
            var userId = GetUserId();
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (campaign == null) return NotFound();
            campaign.Name = updateCampaignDto.Name;
            campaign.Description = updateCampaignDto.Description;

            _context.Campaigns.Update(campaign);
            await _context.SaveChangesAsync();
            return Ok(MapToDetailDto(campaign));
        }

        [HttpPost("{idCampaign}/characters/{idCharacter}")]
        public async Task<ActionResult<CampaignDetailDto>> AddNewCharacter(Guid idCampaign, Guid idCharacter)
        {
            var userId = GetUserId();
            var campaign = await _context.Campaigns
                            .Include(c => c.Characters)
                            .FirstOrDefaultAsync(c => c.Id == idCampaign && c.UserId == userId);
            if (campaign == null) return NotFound();

            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == idCharacter && c.UserId == userId);
            if (character == null) return NotFound();

            if (character.RpgSystemId == campaign.RpgSystemId)
                campaign.Characters.Add(character);

            await _context.SaveChangesAsync();
            return Ok(MapToDetailDto(campaign));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
        {
            var userId = GetUserId();
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (campaign == null) return NotFound();
            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{idCampaign}/characters/{idCharacter}")]
        public async Task<ActionResult<CampaignDetailDto>> RemoveCharacter(Guid idCampaign, Guid idCharacter)
        {
            var userId = GetUserId();

            var campaign = await _context.Campaigns
                                    .Include(c => c.Characters)
                                    .FirstOrDefaultAsync(c => c.Id == idCampaign && c.UserId == userId);

            if (campaign == null) return NotFound();

            var characterToRemove = await _context.Characters.FirstOrDefaultAsync(c => c.Id == idCharacter);

            if (characterToRemove == null) return NotFound();

            campaign.Characters.Remove(characterToRemove);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User ID not found in token.");
            return userId;
        }
        private CampaignDetailDto MapToDetailDto(Campaign campaign)
        {
            return new CampaignDetailDto
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                RpgSystemId = campaign.RpgSystemId,
                Characters = campaign.Characters
            };
        }
    }
}