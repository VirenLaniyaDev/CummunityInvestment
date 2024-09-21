using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CommunityInvestmentContext _context;
        internal DbSet<T> dbSet;
        public Repository(CommunityInvestmentContext context)
        {
            _context = context;
            this.dbSet = _context.Set<T>();
        }

        public List<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public T GetById(Expression<Func<T, bool>> filter) 
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Add or Update the User notification.
        /// </summary>
        /// <param name="userNotificationData">Takes UserNotificationVM Object.</param>
        /// <returns></returns>
        public bool AddOrUpdateNotification(UserNotificationVM userNotificationData)
        {
            try
            {
                UserNotification _userNotification;
                if (userNotificationData.UserNotificationId != null)
                {
                    _userNotification = _context.UserNotifications.Find(userNotificationData.UserNotificationId);
                    if (_userNotification == null)
                        throw new Exception("User Notification not found!");
                }
                else
                {
                    _userNotification = new UserNotification();
                    _userNotification.UserId = userNotificationData.UserId;
                    _userNotification.CreatedAt = DateTime.Now;
                }
                _userNotification.NotificationFor = userNotificationData.NotificationFor;
                _userNotification.NotificationMessage = userNotificationData.NotificationMessage;
                _userNotification.NotificationLink = userNotificationData.NotificationLink;
                _userNotification.IsRead = userNotificationData.IsRead;
                _userNotification.UpdatedAt = DateTime.Now;
                _context.UserNotifications.Update(_userNotification);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
