using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Request
{
    public class SponsorRequestDTO
    {

        public string Name { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? WebsiteURl { get; set; }

        public SponsorCategory Category { get; set; }


    }
}
