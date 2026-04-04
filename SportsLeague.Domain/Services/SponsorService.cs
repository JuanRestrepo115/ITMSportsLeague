using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces.Repositories;

using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }
        // CRUD de Sponsors

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID {Id}", id);
            var team = await _sponsorRepository.GetByIdAsync(id);

            if(team == null) {
                _logger.LogWarning("Sponsor with ID {Id} not found", id);
            }

            return team;
        }


        public async Task<Sponsor> CreateAsync(Sponsor Sponsor)
        {
            _logger.LogInformation("Creating new sponsor with name {Name}", Sponsor.Name);


            // Validaciones de negocio

            // Verificar que el nombre del sponsor no esté duplicado
            if (await _sponsorRepository.ExistsByNameAsync(Sponsor.Name)) {
                _logger.LogWarning("Sponsor with name {Name} already exists", Sponsor.Name);
                throw new InvalidOperationException($"Sponsor with name '{Sponsor.Name}' already exists.");
            }

            // Verificar si el formato de Email es valido

            try{

                var email = new System.Net.Mail.MailAddress(Sponsor.ContactEmail);

            }catch (FormatException){
                _logger.LogWarning("Invalid email format for sponsor {Name}: {Email}", Sponsor.Name, Sponsor.ContactEmail);
                throw new FormatException($"Invalid email format: '{Sponsor.ContactEmail}'");
            }

            return await _sponsorRepository.CreateAsync(Sponsor);

        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);

            if (!exists)

            {

                throw new KeyNotFoundException(

                $"No se encontró el jugador con ID {id}");

            }

            _logger.LogInformation("Deleting sponsor with ID {Id}", id);
            await _sponsorRepository.DeleteAsync(id);

        }

        public async Task UpdateAsync(int id, Sponsor Sponsor)
        {

            // Validaciones de negocio

            // Validar que el Sponsor a actualizar exista
            var existsSponsor = await _sponsorRepository.GetByIdAsync(id);
            if(existsSponsor == null) {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {id}");   
            }

            
            // Verificar que el nombre del sponsor no esté duplicado
            if (await _sponsorRepository.ExistsByNameAsync(Sponsor.Name))
            {
                _logger.LogWarning("Sponsor with name {Name} already exists", Sponsor.Name);
                throw new InvalidOperationException($"Sponsor with name '{Sponsor.Name}' already exists.");
            }

            // Verificar si el formato de Email es valido
            try
            {

                var email = new System.Net.Mail.MailAddress(Sponsor.ContactEmail);

            }
            catch (FormatException)
            {
                _logger.LogWarning("Invalid email format for sponsor {Name}: {Email}", Sponsor.Name, Sponsor.ContactEmail);
                throw new FormatException($"Invalid email format: '{Sponsor.ContactEmail}'");
            }

            existsSponsor.Name = Sponsor.Name;
            existsSponsor.ContactEmail = Sponsor.ContactEmail;
            existsSponsor.Phone = Sponsor.Phone;
            existsSponsor.Phone = Sponsor.WebsiteURl;
            existsSponsor.Category = Sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID {Id}", id);
            await _sponsorRepository.UpdateAsync(existsSponsor);

        }

        // Operaciones de relacion con los torneos

        public async Task<TournamentSponsor> AddToTournamentAsync(int sponsorId, TournamentSponsor tournamentSponsor)
        {
            _logger.LogInformation("Adding sponsor with ID {SponsorId} to tournament with ID {TournamentId}", sponsorId, tournamentSponsor.TournamentId);

            // Verificar que Sponsor exista
            var existsSponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (existsSponsor == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");
            }

            // Verificar que el torneo exista

            var existsTournament = await _tournamentRepository.GetByIdAsync(tournamentSponsor.TournamentId);
            if (existsTournament == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el torneo con ID {tournamentSponsor.TournamentId}");
            }

            // Verificar que la vinculacion no exista previamente (No este duplicada)

            var existsLink = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentSponsor.TournamentId, sponsorId);
            if (existsLink != null)
            {
                throw new InvalidOperationException(
                    $"La vinculacion entre el sponsor con ID {sponsorId} y el torneo con ID {tournamentSponsor.TournamentId} ya existe");
            }

            // Validar que el monto del contrato sea positivo

            if (tournamentSponsor.ContractAmount <= 0)
            {
                throw new InvalidOperationException("Contract amount debe ser mayor a 0.");
            }

            tournamentSponsor.SponsorId = sponsorId;
            tournamentSponsor.JoinedAt = DateTime.UtcNow;
            return await _sponsorRepository.AddToTournamentAsync(tournamentSponsor);

        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorsAsync(int sponsorId)
        {
            _logger.LogInformation("Retrieving tournaments for sponsor with ID {SponsorId}", sponsorId);

            // Verificar que el Sponsor exista
            var existsSponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (existsSponsor == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");
            }

            return await _sponsorRepository.GetTournamentsBySponsorIdAsync(sponsorId);
        }

        public async Task RemoveFromTournamentAsync(int sponsorId, int tournamentId)
        {
            _logger.LogInformation("Removing sponsor with ID {SponsorId} from tournament with ID {TournamentId}", sponsorId, tournamentId);

            var existing = await _sponsorRepository.GetTournamentSponsorAsync(sponsorId, tournamentId);
            if (existing == null) {
                throw new KeyNotFoundException(
                    $"No se encontró la vinculación entre el sponsor con ID {sponsorId} y el torneo con ID {tournamentId}");
            
            }

            await _sponsorRepository.RemoveFromTournamentAsync(existing);
        }

       


    }
}
