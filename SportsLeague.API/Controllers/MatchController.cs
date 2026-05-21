using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService = null!;
        private readonly IMatchLineupService _matchLineupService = null!;
        private readonly IMapper _mapper = null!;
        private readonly ILogger<MatchController> _logger = null!;

        public MatchController(
            IMatchService matchService,
            IMatchLineupService matchLineupService,
            IMapper mapper,
            ILogger<MatchController> logger)
        {
            _matchService = matchService;
            _matchLineupService = matchLineupService;
            _mapper = mapper;
            _logger = logger;
        }

        // ─── Match CRUD ──────────────────────────────────────────────

        [HttpGet("tournament/{tournamentId}")]
        public async Task<ActionResult<IEnumerable<MatchResponseDTO>>> GetByTournament(int tournamentId)
        {
            try
            {
                var matches = await _matchService.GetAllByTournamentAsync(tournamentId);
                return Ok(_mapper.Map<IEnumerable<MatchResponseDTO>>(matches));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResponseDTO>> GetById(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
                return NotFound(new { message = $"Partido con ID {id} no encontrado" });

            return Ok(_mapper.Map<MatchResponseDTO>(match));
        }

        [HttpPost]
        public async Task<ActionResult<MatchResponseDTO>> Create([FromBody] MatchRequestDTO dto)
        {
            try
            {
                var match = _mapper.Map<Match>(dto);
                var created = await _matchService.CreateAsync(match);
                var matchWithDetails = await _matchService.GetByIdAsync(created.Id);
                var response = _mapper.Map<MatchResponseDTO>(matchWithDetails);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] MatchRequestDTO dto)
        {
            try
            {
                var match = _mapper.Map<Match>(dto);
                await _matchService.UpdateAsync(id, match);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _matchService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateMatchStatusDTO dto)
        {
            try
            {
                await _matchService.UpdateStatusAsync(id, dto.Status);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // ─── MatchLineup ─────────────────────────────────────────────

        [HttpPost("{matchId}/lineup")]
        public async Task<ActionResult<MatchLineupResponseDTO>> AddToLineup(int matchId, [FromBody] MatchLineupRequestDTO dto)
        {
            try
            {
                var matchLineup = _mapper.Map<MatchLineup>(dto);
                var created = await _matchLineupService.AddPlayerAsync(matchId, matchLineup);
                var response = _mapper.Map<MatchLineupResponseDTO>(created);
                return CreatedAtAction(nameof(GetLineup), new { matchId }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{matchId}/lineup")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineup(int matchId)
        {
            try
            {
                var lineup = await _matchLineupService.GetByMatchAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{matchId}/lineup/team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByTeam(int matchId, int teamId)
        {
            try
            {
                var lineup = await _matchLineupService.GetByMatchAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{matchId}/lineup/{id}")]
        public async Task<ActionResult> DeleteFromLineup(int matchId, int id)
        {
            try
            {
                await _matchLineupService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}