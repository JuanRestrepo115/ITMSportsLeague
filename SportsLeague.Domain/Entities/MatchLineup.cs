using SportsLeague.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Entities
{
    public class MatchLineup : AuditBase
    {

        public int MatchId { get; set; }

        public int PlayerId { get; set; }

        public bool IsStarter { get; set; } // True = Titular, False = Suplente

        public PlayerPosition Position{ get; set; }  

        //Navigation Propierties

        public Match Match { get; set; } = null!;

        public Player Player { get; set; } = null!;

    }
}
