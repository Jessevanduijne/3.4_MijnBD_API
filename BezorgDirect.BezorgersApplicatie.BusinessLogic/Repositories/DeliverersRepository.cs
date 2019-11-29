using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup by Lennart de Waart (563079) */
    public class DeliverersRepository : IDeliverersRepository // DeliverersRepository should contain everything in contract IDeliverersRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for DeliverersRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public DeliverersRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Asynchronous method that retrieves a list of deliverers who are available, do not have a current delivery and are in range of the customer.
        /// </summary>
        /// <param name="v">Best suited vehicle for delivery</param>
        /// <param name="dueDate"></param>
        /// <param name="config">Configuration of variables</param>
        /// <returns>List of Deliverer classes</returns>
        public List<Deliverer> GetDeliverersPool(Vehicles v, DateTime dueDate, dynamic config)
        {
            try
            {
                // Get variables from variables.json document
                int mttdim = (int)config["NotificationsService"]["MaximumTimeToDeliverInMinutes"];
                int sodtfaim = (int)config["NotificationsService"]["SubtractionOfDeliveryTimeForAvailabilityInMinutes"];
                int mattdim = mttdim - sodtfaim;
                var fullJoin = from deliverers in _context.Deliverers
                               join deliveries in _context.Deliveries on deliverers.Id equals deliveries.DelivererId
                               join availabilities in _context.Availabilities on deliverers.Id equals availabilities.DelivererId
                               join notifications in _context.Notifications on deliverers.Id equals notifications.DelivererId
                               where deliverers.Vehicle == v && deliveries.Status != DeliveryStatus.Bevestigd &&
                                     deliveries.Status != DeliveryStatus.Onderweg &&
                                     availabilities.Date == dueDate.Date &&
                                     availabilities.StartTime <= dueDate.Subtract(TimeSpan.FromMinutes(mattdim)).TimeOfDay &&
                                     availabilities.EndTime >= dueDate.Subtract(TimeSpan.FromMinutes(sodtfaim)).TimeOfDay &&
                                     notifications.Status != NotificationStatus.Verstuurd
                               group deliverers by deliverers.Id into g
                               select g.Key;
                var leftJoin = from deliverers in _context.Deliverers
                               join deliveries in _context.Deliveries on deliverers.Id equals deliveries.DelivererId into dd
                               from deliveries in dd.DefaultIfEmpty()
                               where deliveries.Id == null
                               select deliverers.Id;
                List<Deliverer> d = _context.Deliverers.Where(x => fullJoin.Contains(x.Id) || leftJoin.Contains(x.Id)).ToList();
                foreach (Deliverer deliverer in d)
                {
                    // Load navigational properties
                    deliverer.Home = _context.Locations.Find(deliverer.HomeId);
                    deliverer.Deliveries = _context.Deliveries.Where(x => x.DelivererId == deliverer.Id).ToList();
                    deliverer.Notifications = _context.Notifications.Where(x => x.DelivererId == deliverer.Id).ToList();
                }
                return d ?? throw new ArgumentNullException("The query result returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Asynchronous method that returns a Deliverer that has the passed token.
        /// </summary>
        /// <param name="token">Bearer token</param>
        /// <returns>Filled Deliverer class or null</returns>
        public async Task<Deliverer> GetDelivererByToken(string token)
        {
            try
            {
                Deliverer d = await _context.Deliverers.Where(x => x.Token == token).FirstOrDefaultAsync();
                if (d != null)
                {
                    _context.Entry(d).Reference(x => x.Home).Load();
                    return d;
                } 
                else throw new ArgumentNullException($"Could not retrieve a Deliverer with token {token}. Such an entity does not exist in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves a Deliverer with the given identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Deliverer class or null</returns>
        public async Task<Deliverer> GetDelivererById(Guid id)
        {
            try
            {
                Deliverer d = await _context.Deliverers.Where(x => x.Id == id).SingleAsync();
                if (d != null)
                {
                    _context.Entry(d).Collection(x => x.Deliveries).Load();
                    return d;
                }
                else throw new ArgumentNullException($"Could not retrieve a Deliverer with id {id}. Such an entity does not exist in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Save Deliverer,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="deliverer"></param>
        /// <returns></returns>
        public bool Create(Deliverer deliverer)
        {
            try
            {
                _context.Deliverers.Add(deliverer);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                return false;
            }
        }

        /// <summary>
        /// Get Deliverer by Email.
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Deliverer</returns>
        public Deliverer Read(string email)
        {
            try
            {
                Deliverer deliverer = _context.Deliverers.
                    Where(x => x.EmailAddress == email)
                    .Single();

                _context.Entry(deliverer).Reference(d => d.Home).Load();

                return deliverer;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                return null;
            }
        }

        /// <summary>
        /// Updates a Deliverer,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="updatedDeliverer"></param>
        /// <returns></returns>
        public bool Update(Deliverer updatedDeliverer)
        {
            try
            {
                _context.Update(updatedDeliverer);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                return false;
            }
        }

        /* This function was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves Deliverers.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Deliverer class or null</returns>
        public async Task<List<Deliverer>> GetDeliverers()
        {
            try
            {
                List<Deliverer> deliverers = await _context.Deliverers.ToListAsync();

                if (deliverers != null)
                {
                    foreach (var d in deliverers)
                        _context.Entry(d).Collection(x => x.Deliveries).Load();

                    return deliverers;
                }
                else throw new ArgumentNullException($"Could not retrieve all Deliverers. Such an entity does not exist in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
