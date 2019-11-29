using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class NotificationsRepository : INotificationsRepository // NotificationsRepository should contain everything in contract INotificationsRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for NotificationsRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public NotificationsRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronous method that retrieves a Notification class based on the Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Notification class or null</returns>
        public async Task<Notification> GetNotificationById(Guid id)
        {
            try
            {
                Notification n = await _context.Notifications.FindAsync(id);
                // Check if Notification could be retrieved
                if (n != null)
                    return n;
                else throw new ArgumentNullException($"Could not retrieve a Notification record with Id {id} from the context. Such an entity does not exist in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that adds a Notification class to the context
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="deliveryId"></param>
        /// <param name="createdAt"></param>
        /// <param name="expiredAt"></param>
        /// <param name="status"></param>
        /// <returns>true or false</returns>
        public async Task<bool> AddNotification(Guid id, Guid delivererId, Guid deliveryId, DateTime createdAt, DateTime expiredAt, NotificationStatus status)
        {
            try
            {
                await _context.AddAsync(new Notification(id, delivererId, deliveryId, createdAt, null, null, expiredAt, status));
                // Check if context was updated
                if (_context.SaveChanges() > 0)
                    return true;
                else throw new Exception("Notification could not be added to the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /// <summary>
        /// Public method that gets the first Notification record of a deliverer where the status is equal to send.
        /// </summary>
        /// <param name="delivererId"></param>
        /// <returns>Filled Notification class or null</returns>
        public async Task<Notification> GetFirstOpenNotificationByDelivererId(Guid delivererId)
        {
            try
            {
                Notification n = await _context.Notifications.Where(x => x.DelivererId == delivererId && x.Status == NotificationStatus.Verstuurd)
                    .FirstOrDefaultAsync();
                // Check if Notification could be retrieved
                if (n != null)
                    return n;
                else throw new ArgumentNullException($"Could not retrieve an open Notification record for deliverer {delivererId} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that updates a Notifications records status and returned whether it succeeded or not
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accepted"></param>
        /// <returns>true of false</returns>
        public async Task<Notification> UpdateNotificationStatusById(Guid id, bool accepted)
        {
            try
            {
                Notification n = await _context.Notifications.FindAsync(id);
                // You shouldn't be able to patch a record with a status already set
                if (n == null || n.Status != NotificationStatus.Verstuurd)
                    throw new Exception($"Notification returned null or the status has already been set.");
                switch (accepted)
                {
                    case true:
                        n.Status = NotificationStatus.Geaccepteerd;
                        n.AcceptedAt = DateTime.Now;
                        break;
                    case false:
                        n.Status = NotificationStatus.Geweigerd;
                        n.RefusedAt = DateTime.Now;
                        break;
                }
                // Check if context was updated
                if (_context.SaveChanges() > 0)
                    return n;
                else throw new Exception($"Context could not update Notification with id {n.Id}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that retrieves a list of all expired notifications
        /// </summary>
        /// <returns>List of Notification classes or null</returns>
        public async Task<List<Notification>> GetAllExpiredNotifications()
        {
            try
            {
                // Get a list of all Notification record which are expired, but don't have the right status
                List<Notification> ns = await _context.Notifications.Where(x => x.Status == NotificationStatus.Verstuurd && x.ExpiredAt < DateTime.Now).ToListAsync();
                return ns;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that updates all the expired records in the Notifications context.
        /// </summary>
        /// <returns>true or false</returns>
        public bool UpdateExpiredNotifications(List<Notification> expiredNotifications)
        {
            try
            {                
                foreach (Notification n in expiredNotifications)
                {
                    // Set Notification to expired
                    n.Status = NotificationStatus.Verlopen;
                }
                // Check if context was updated
                if (_context.SaveChanges() > 0)
                    return true;
                else throw new Exception("Context could not update the expired notifications.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }
    }
}
