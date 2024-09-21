using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository
{
    public class FilterRepository : IFilterRepository
    {
        public readonly CommunityInvestmentContext _db;
        public FilterRepository(CommunityInvestmentContext db)
        {
            _db = db; 
        }

        public IEnumerable<Country> GetAllCountry()
        {
            return _db.Countries;
        }
        
        public IEnumerable<City> GetAllCity()
        {
            return _db.Cities;
        }
        
        public IEnumerable<City> GetCitiesByCountry(string[] Country)
        {
            var _Cities = _db.Cities.Include(m=>m.Country);
            return _Cities.Where(city => Country.Contains(city.Country.Name));
        }

        public IEnumerable<City> GetCitiesByCountryId(long CountryId)
        {
            var _Cities = _db.Cities;
            return _Cities.Where(city => city.CountryId == CountryId);
        }
        
        public IEnumerable<MissionTheme> GetAllMissionTheme()
        {
            return _db.MissionThemes;
        } 
        
        public IEnumerable<Skill> GetAllSkills()
        {
            var _Skills = _db.Skills;
            return _Skills;
        } 

        public MissionTheme GetMissionThemeById(long MissionThemeId)
        {
            return _db.MissionThemes.Find(MissionThemeId);  
        }

        public Skill GetSkillById(long SkillId)
        {
            return _db.Skills.Find(SkillId);
        }
    }
}
