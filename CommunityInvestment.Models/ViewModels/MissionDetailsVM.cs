using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class MissionDetailsVM
    {
        public MissionDetailsVM()
        {
            //Comments = new HashSet<Comment>();
            //MissionApplications = new HashSet<MissionApplication>();
            //MissionDocuments = new HashSet<MissionDocument>();
            //MissionInvites = new HashSet<MissionInvite>();
            //MissionMedia = new HashSet<MissionMedium>();
            //MissionRatings = new HashSet<MissionRating>();
            //MissionSkills = new HashSet<MissionSkill>();
            //Skills = new HashSet<Skill>();
            //Timesheets = new HashSet<Timesheet>();
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
        public int TotalVolunteers { get; set; }
        public bool UserApplied { get; set; }
        public int? GoalAchieved { get; set; }
        public bool IsFavorite { get; set; }
        public float Ratings { get; set; }
        public MissionRating? UserRating { get; set; } = null;
        public int RatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual Country Country { get; set; } = null!;
        public virtual MissionTheme Theme { get; set; } = null!;
        public virtual GoalMission GoalMission { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<MissionApplication> MissionApplications { get; set; }
        public virtual List<MissionDocument> MissionDocuments { get; set; }
        public virtual List<MissionInvite> MissionInvites { get; set; }
        public virtual List<MissionMedium> MissionMedia { get; set; }
        public virtual List<MissionSkill> MissionSkills { get; set; }
        public virtual List<Timesheet> Timesheets { get; set; }

        public List<MissionsDetailsVM> RelatedMissions { get; set;}
        public List<User> CoWorkers { get; set; }

    }
}
