using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories; 

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        // Metodos del Sponsor
        public async Task<bool> ExistsByNameAsync(string name)
                {
                    return await _dbSet.AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }


        //Metodos del TournamentSponsor

        public async Task<TournamentSponsor> AddToTournamentAsync(TournamentSponsor tournamentSponsor)
        {
            await _context.Set<TournamentSponsor>().AddAsync(tournamentSponsor);
            await _context.SaveChangesAsync();
            return tournamentSponsor;
        }


        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorIdAsync(int sponsorId)
        {
            return await _context.Set<TournamentSponsor>()
                .Include(ts => ts.Tournament)
                .Include(ts => ts.Sponsor)
                .Where(ts => ts.SponsorId == sponsorId)
                .ToListAsync();
        }

        public async Task<TournamentSponsor?> GetTournamentSponsorAsync(int sponsorId, int tournamentId)
        {
            return await _context.Set<TournamentSponsor>()
                .Include(ts => ts.Tournament)
                .Include(ts => ts.Sponsor)
                .FirstOrDefaultAsync(ts => ts.SponsorId == sponsorId && ts.TournamentId == tournamentId);
        }

        public async Task RemoveFromTournamentAsync(TournamentSponsor tournamentSponsor)
        {
            _context.Set<TournamentSponsor>().Remove(tournamentSponsor);
            await _context.SaveChangesAsync();
            
        }
    }
}
