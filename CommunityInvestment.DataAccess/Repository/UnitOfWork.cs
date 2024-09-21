using CommunityInvestment.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommunityInvestmentContext _db;
        public UnitOfWork(CommunityInvestmentContext db, IConfiguration configuration)
        {
            _db = db;
            User = new UserRepository(_db, configuration);
            Filter = new FilterRepository(_db);
            Mission = new MissionRepository(_db, configuration);
            VolunteerStory = new VolunteerStoryRepository(_db, configuration);
            Admin = new AdminRepository(_db, configuration);
            AdminMission = new AdminMissionRepository(_db);
        }

        public UserRepository User { get; private set; }

        public FilterRepository Filter { get; private set; }
        public MissionRepository Mission { get; private set; }
        public VolunteerStoryRepository VolunteerStory { get; private set; }
        public AdminRepository Admin { get; private set; }
        public AdminMissionRepository AdminMission { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
