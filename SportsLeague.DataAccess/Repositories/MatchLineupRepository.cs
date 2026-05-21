using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Enums;

using SportsLeague.Domain.Helpers;

using SportsLeague.Domain.Interfaces.Repositories;

using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchLineupRepository : GenericRepository<MatchLineup>,IMatchLineupRepository
    {
        
        public MatchLineupRepository(LeagueDbContext context) : base(context)
        {}
        public async Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _dbSet.CountAsync(ml => ml.MatchId == matchId &&
                                                 ml.IsStarter &&
                                                 ml.Player.TeamId == teamId);
        }

        public async Task<bool> ExistByMatchAndPlayerAsync(int matchId, int playerId)
        {
            return await _dbSet
                .AnyAsync(ml => ml.MatchId == matchId && ml.PlayerId == playerId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _dbSet
                .Where(ml => ml.MatchId == matchId && ml.Player.TeamId == teamId)
                .Include(ml => ml.Match)
                .Include(ml => ml.Player)
                    .ThenInclude(p => p.Team)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            return await _dbSet
                .Where(ml => ml.MatchId == matchId)
                .Include(ml => ml.Match)
                .Include(ml => ml.Player)
                    .ThenInclude(p => p.Team)
                .ToListAsync();
        }

        public async Task<MatchLineup> CreateWithDetailsAsync(MatchLineup matchLineup)
        {
            await _context.MatchLineups.AddAsync(matchLineup);
            await _context.SaveChangesAsync();

            return await _context.MatchLineups
                .Include(ml => ml.Player)
                    .ThenInclude(p => p.Team)
                .Include(ml => ml.Match)
                .FirstOrDefaultAsync(ml => ml.Id == matchLineup.Id);
        }

    }
}
