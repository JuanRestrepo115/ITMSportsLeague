using System;
using System.Collections.Generic;
using System.Text;
using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ISponsorRepository:IGenericRepository<Sponsor>
    {
        Task<bool> ExistsByNameAsync(String name);
        Task<TournamentSponsor> AddToTournamentAsync(TournamentSponsor tournamentSponsor);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId);
        Task<TournamentSponsor?> GetTournamentSponsorAsync(int sponsorId, int tournamentId);
        Task RemoveFromTournamentAsync(TournamentSponsor tournamentSponsor);
    }
}
