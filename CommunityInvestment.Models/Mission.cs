using System;
using System.Collections.Generic;

namespace CommunityInvestment.Models
{
    public partial class Mission
    {
        public Mission()
        {
            Comments = new HashSet<Comment>();
            FavoriteMissions = new HashSet<FavoriteMission>();
            GoalMissions = new HashSet<GoalMission>();
            MissionApplications = new HashSet<MissionApplication>();
            MissionDocuments = new HashSet<MissionDocument>();
            MissionInvites = new HashSet<MissionInvite>();
            MissionMedia = new HashSet<MissionMedium>();
            MissionRatings = new HashSet<MissionRating>();
            MissionSkills = new HashSet<MissionSkill>();
            Stories = new HashSet<Story>();
            Timesheets = new HashSet<Timesheet>();
        }

        public long MissionId { get; set; }
        public long CityId { get; set; }
        public long CountryId { get; set; }
        public long ThemeId { get; set; }
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string MissionType { get; set; } = null!;
        public string? Status { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationDetail { get; set; }
        public string? Availability { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual Country Country { get; set; } = null!;
        public virtual MissionTheme Theme { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FavoriteMission> FavoriteMissions { get; set; }
        public virtual ICollection<GoalMission> GoalMissions { get; set; }
        public virtual ICollection<MissionApplication> MissionApplications { get; set; }
        public virtual ICollection<MissionDocument> MissionDocuments { get; set; }
        public virtual ICollection<MissionInvite> MissionInvites { get; set; }
        public virtual ICollection<MissionMedium> MissionMedia { get; set; }
        public virtual ICollection<MissionRating> MissionRatings { get; set; }
        public virtual ICollection<MissionSkill> MissionSkills { get; set; }
        public virtual ICollection<Story> Stories { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}
