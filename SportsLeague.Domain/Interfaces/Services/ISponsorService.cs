using System;
using System.Collections.Generic;
using System.Text;
using SportsLeague.Domain.Entities; 
namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();

        Task<Sponsor?> GetByIdAsync(int id);

        Task<Sponsor> CreateAsync(Sponsor Sponsor);

        Task UpdateAsync(int id, Sponsor Sponsor);

        Task DeleteAsync(int id);

        //TournamentSponsor Metodos

        Task<TournamentSponsor> AddToTournamentAsync(int sponsorId, TournamentSponsor tournamentSponsor);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorsAsync(int sponsorId);
        Task RemoveFromTournamentAsync(int sponsorId, int tournamentId);

    }
}
