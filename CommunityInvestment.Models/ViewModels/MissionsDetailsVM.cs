using CommunityInvestment.Models;
using NuGet.ProjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace CommunityInvestment.Models.ViewModels
{
    public class MissionsDetailsVM
    {
        public long MissionId { get; set; }
        public long CityId { get; set; }
        public string CityName { get; set; } = null!;
        public long CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public long ThemeId { get; set; }
        public string ThemeTitle { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string MissionType { get; set; } = null!;
        public string? Status { get; set; }
        public string? OrganizationName { get; set; }
        public string? GoalObjectiveText { get; set; }
        public bool UserApplied { get; set; }
        public bool IsFavorite { get; set; }
        public int TotalVolunteers { get; set; }
        public int? GoalValue { get; set; }
        public int? GoalAchieved { get; set; }
        public float Ratings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual List<MissionSkill> Skills { get; set; }
        public virtual MissionMedium MissionCardMedia { get; set; }
    }
}
