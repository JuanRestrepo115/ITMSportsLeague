using System;
using System.Collections.Generic;
using System.Text;
using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);

        Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup matchLineup);

        Task DeleteAsync(int id);

    }
}
