using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SportsLeague.API.DTOs.Request;

using SportsLeague.API.DTOs.Response;

using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.Domain.Services;
namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;


        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }



        // CRUD del Sponsor

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()

        {

            var sponsors = await _sponsorService.GetAllAsync();

            var sponsorsDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);

            return Ok(sponsorsDto);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);
            if (sponsor == null)
            {
                return NotFound("$No existe un Sponsor con el ID {id}");
            }
            var sponsorDto = _mapper.Map<SponsorResponseDTO>(sponsor);
            return Ok(sponsorDto);
        }


        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO sponsorRequestDto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(sponsorRequestDto);
                var createdSponsor = await _sponsorService.CreateAsync(sponsor);
                var sponsorResponseDto = _mapper.Map<SponsorResponseDTO>(createdSponsor);
                return CreatedAtAction(nameof(GetById), new { id = sponsorResponseDto.Id }, sponsorResponseDto);

            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }


        }

        [HttpPut("{id}")]

        public async Task<ActionResult<SponsorResponseDTO>> Update(int id, SponsorRequestDTO sponsorRequestDto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(sponsorRequestDto);
                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // Metodos con Tournament
        [HttpGet("{id}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(int id)
        {
            try
            {
                var tournaments = await _sponsorService.GetTournamentsBySponsorsAsync(id);
                var response = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(tournaments);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("{id}/tournaments")]
        public async Task<ActionResult<TournamentSponsorResponseDTO>> AddToTournament(int id, [FromBody] TournamentSponsorRequestDTO dto)
        {
            try
            {
                var tournamentSponsor = _mapper.Map<TournamentSponsor>(dto);
                var created = await _sponsorService.AddToTournamentAsync(id, tournamentSponsor);
                var response = _mapper.Map<TournamentSponsorResponseDTO>(created);
                return CreatedAtAction(nameof(GetTournaments), new { id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<ActionResult> RemoveFromTournament(int id, int tid)
        {
            try
            {
                await _sponsorService.RemoveFromTournamentAsync(id, tid);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
