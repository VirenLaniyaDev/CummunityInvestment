using CommunityInvestment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IFilterRepository
    {
        public IEnumerable<Country> GetAllCountry();
        public IEnumerable<City> GetAllCity();
        public IEnumerable<City> GetCitiesByCountry(string[] Country);
        public IEnumerable<MissionTheme> GetAllMissionTheme();
        public IEnumerable<Skill> GetAllSkills();
        public MissionTheme GetMissionThemeById(long MissionThemeId);
        public Skill GetSkillById(long SkillId);
    }
}
