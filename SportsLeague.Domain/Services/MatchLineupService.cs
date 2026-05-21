using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService: IMatchLineupService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<MatchLineupService> _logger;
   
        public MatchLineupService(
            ITeamRepository teamRepository,
            IMatchLineupRepository matchLineupRepository,
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository,
            ILogger<MatchLineupService> logger)
        {
            _teamRepository = teamRepository;
            _matchLineupRepository = matchLineupRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            _logger.LogInformation("Retrieving lineup for match {MatchId}", matchId);

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _matchLineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            _logger.LogInformation("Retrieving lineup for match {MatchId} and team {TeamId}", matchId, teamId);

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup matchLineup)
        {
            // Validaciones:
            // 1. Verificar que el partido exista:
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
            {
                throw new KeyNotFoundException("Match not found with ID {MatchId}");
            }
            //2. Verificar que el jugador exista:
            var player = await _playerRepository.GetByIdAsync(matchLineup.PlayerId);
            if (player == null)
            {
                throw new KeyNotFoundException("Player not found with ID {PlayerId}");
            }
            //3. Verificar que el jugador pertenezca a uno de los equipos del partido:
            if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
            {
                throw new InvalidOperationException("Player does not belong to either team in the match");
            }
            //4. Verificar que el jugador no esté ya en la alineación del partido:
            var existingLineup = await _matchLineupRepository.ExistByMatchAndPlayerAsync(matchId, matchLineup.PlayerId);
            if (existingLineup)
            {
                throw new InvalidOperationException("Player is already in the lineup for this match");
            }

            //5. Verificar que haya menos de 11 jugadores titulares por equipo:
            if(matchLineup.IsStarter)
            {
                var starterCount = await _matchLineupRepository.CountStartersByMatchAndTeamAsync(matchId, player.TeamId);
                if (starterCount >= 11)
                {
                    throw new InvalidOperationException("Maximum number of starters reached for this team in the match");
                }
            }
            //6. El partido debe estar en el estado "Scheduled" para poder agregar jugadores a la alineación:
            if (match.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException("Players can only be added to the lineup for matches that are scheduled");
            }

            _logger.LogInformation("Adding player {PlayerId} to lineup for match {MatchId}", matchLineup.PlayerId, matchId);

            matchLineup.MatchId = matchId;

            return await _matchLineupRepository.CreateWithDetailsAsync(matchLineup);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _matchLineupRepository.GetByIdAsync(id);
            if (existing == null) {
                throw new KeyNotFoundException("Match lineup not found");
            }
            _logger.LogInformation("Deleting match lineup with ID {Id}", id);
            await _matchLineupRepository.DeleteAsync(id);
        }
    }
}
