using System;
using System.Collections.Generic;
using System.Text;
using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRepository: IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);

        Task<bool> ExistByMatchAndPlayerAsync(int matchId, int playerId);

        Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId);

        Task<MatchLineup> CreateWithDetailsAsync(MatchLineup matchLineup);
    }
}
