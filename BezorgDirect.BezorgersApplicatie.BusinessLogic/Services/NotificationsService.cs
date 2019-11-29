using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class NotificationsService : INotificationsService // NotificationsService should contain everything in contract INotificationsService
    {
        private readonly IDeliverersService _deliverersService;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly ILocationsService _locationsService;
        private readonly IDeliveriesService _deliveriesService;
        private readonly ILogger _logger;
        private readonly Random rnd;
        private List<Deliverer> DeliverersPool;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="deliverersRepo"></param>
        /// <param name="notificationsRepo"></param>
        /// <param name="locationsRepo"></param>
        /// <param name="deliveriesRepo"></param>
        /// <param name="logger"></param>
        public NotificationsService(IDeliverersRepository deliverersRepo, IDeliveriesRepository deliveriesRepo, INotificationsRepository notificationsRepo, ILocationsRepository locationsRepo, ILogger logger)
        {
            this._notificationsRepo = notificationsRepo;      
            this._logger = logger;
            this.rnd = new Random();
            this._locationsService = new LocationsService(locationsRepo, _logger);
            this._deliverersService = new DeliverersService(deliverersRepo, _logger);
            this._deliveriesService = new DeliveriesService(deliveriesRepo, locationsRepo, _logger);
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
                _logger.Information($"A request has been made to add a Notification with id {id} to the context.");
                bool result =  await _notificationsRepo.AddNotification(id, delivererId, deliveryId, createdAt, expiredAt, status);
                return result ? result : throw new Exception($"Dependency failure: The repository returned false.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
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
                _logger.Information($"A request has been made to retrieve a Notification with id {id} from the context.");
                Notification n =  await _notificationsRepo.GetNotificationById(id);
                return n ?? throw new ArgumentNullException("Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that gets the first Notification record of a deliverer where the status is equal to send.
        /// </summary>
        /// <param name="delivererId"></param>
        /// <returns>Filled Notification class or null</returns>
        public async Task<Notification> GetFirstOpenNotificationByDelivererId(Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to retrieve the first open Notification for deliverer {delivererId} from the context.");
                Notification n = await _notificationsRepo.GetFirstOpenNotificationByDelivererId(delivererId);
                // Set navigational property to null as we do not want to pass more information then necessary to external applications
                if (n.Deliverer != null) n.Deliverer = null;
                return n ?? throw new ArgumentNullException("Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line " +
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
        public async Task<bool> UpdateNotificationStatusById(Guid id, bool accepted)
        {
            try
            {
                _logger.Information($"A request has been made to update the status of Notification with id {id} in the context.");
                Notification n = await _notificationsRepo.UpdateNotificationStatusById(id, accepted);
                if (n != null && !accepted)
                {
                    Delivery d = await _deliveriesService.GetDelivery(n.DeliveryId);
                    bool result = await CreateNotification(d.Id, d.Vehicle, d.DueDate, new LatLng(d.Warehouse.Latitude, d.Warehouse.Longitude), new LatLng(d.Customer.Latitude, d.Customer.Longitude));
                    return result ? result 
                        : throw new Exception($"Dependency failure: Could not create a new Notification record for Delivery {d.Id} in the context."); ;
                }
                return n != null ? true : throw new Exception($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /// <summary>
        /// Asynchronous method that updates all the expired records in the Notifications context.
        /// </summary>
        /// <returns>true or false</returns>
        public async Task<bool> UpdateExpiredNotifications()
        {
            try
            {
                _logger.Information($"A request has been made to update the expired Notification records in the context.");
                List<Notification> expired = await _notificationsRepo.GetAllExpiredNotifications();
                bool result = _notificationsRepo.UpdateExpiredNotifications(expired);
                // Check if notifications were updated to expired. If so, create new Notification record
                if (result)
                {
                    bool success;
                    foreach (Notification n in expired)
                    {
                        Delivery d = await _deliveriesService.GetDelivery(n.DeliveryId);
                        success = await CreateNotification(d.Id, d.Vehicle, d.DueDate, new LatLng(d.Warehouse.Latitude, d.Warehouse.Longitude), new LatLng(d.Customer.Latitude, d.Customer.Longitude));
                        if (!success) throw new Exception($"Dependency failure: Could not create a new Notification record for Delivery {d.Id} in the context.");
                    }
                    return result;
                }
                else throw new Exception($"Dependency failure: The repository returned false.");                
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsRepository says: {e.Message} Exception occured on line " +
                    $"{new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }    

        /// <summary>
        /// Asynchronous method which gets and then adds a new Notification record to the context.
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <param name="v"></param>
        /// <param name="dueDate"></param>
        /// <param name="warehouse"></param>
        /// <param name="customer"></param>
        /// <returns>true or false</returns>
        public async Task<bool> CreateNotification(Guid deliveryId, Vehicles v, DateTime dueDate, LatLng warehouse, LatLng customer)
        {
            Notification n = GetNewNotification(deliveryId, v, dueDate, warehouse, customer);
            if (n == null) return false;
            bool result = await AddNotification(n.Id, n.DelivererId, n.DeliveryId, n.CreatedAt, n.ExpiredAt, n.Status);
            return result;
        }

        /// <summary>
        /// Public method that creates a new Notification class based on the received parameters
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <param name="v">Best suited vehicle for delivery</param>
        /// <param name="dueDate"></param>
        /// <param name="warehouse"></param>
        /// <param name="customer"></param>
        /// <returns>Filled Notification class or null</returns>
        public Notification GetNewNotification(Guid deliveryId, Vehicles v, DateTime dueDate, LatLng warehouse, LatLng customer)
        {
            try
            {
                _logger.Information($"A request has been made to create a new Notification for delivery {deliveryId}.");
                // Get algorithm variables from JSON-file
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                GetDeliverersPool(v, dueDate, warehouse, customer, config);
                // Verify that the pool has items
                if (DeliverersPool == null || DeliverersPool.Count == 0) return null;
                SortDeliverersPool(config);
                Guid? delivererId = SelectDeliverer(config);
                int eon = (int)config["NotificationsService"]["ExpirationOfNotificationInMinutes"];
                return delivererId.HasValue
                    ? new Notification(Guid.NewGuid(), delivererId.Value, deliveryId, DateTime.Now, null, null, DateTime.Now.AddMinutes(eon), NotificationStatus.Verstuurd)
                    : null;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Private asynchronous method that creates a delivererspool based on the parameters it receives.
        /// </summary>
        /// <param name="v">Best suited vehicle for delivery</param>
        /// <param name="dueDate"></param>
        /// <param name="warehouse"></param>
        /// <param name="customer"></param>
        /// <param name="config">Configuration of variables</param>
        private void GetDeliverersPool(Vehicles v, DateTime dueDate, LatLng warehouse, LatLng customer, dynamic config)
        {
            try
            {
                _logger.Information($"A request has been made to get a list of all available deliverers for a new Notificaton.");
                DeliverersPool = _deliverersService.GetDeliverersPool(v, dueDate, config);
                List<Deliverer> outOfRange = new List<Deliverer>();
                // Filter pool to deliverers deliverrange
                foreach (Deliverer d in DeliverersPool)
                {                    
                    if (!_locationsService.IsDeliveryInDeliverersRange(new LatLng(d.Home.Latitude, d.Home.Longitude), 
                        warehouse, customer, d.Range))
                        outOfRange.Add(d);
                }
                DeliverersPool = DeliverersPool.Except(outOfRange).ToList();
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                DeliverersPool = null;
            }
        }

        /// <summary>
        /// Private method that sorts the delivererspool based on a random number and a percent chance for a sorting method.
        /// </summary>
        /// <param name="config">Configuration of variables</param>
        private void SortDeliverersPool(dynamic config)
        {
            try
            {
                _logger.Information($"A request has been made to sort the delivererspool. Sorting method pending...");
                // Get sorting chance from variables.json document. Chance of multiple requests at the same time, so store the values locally and not as properties
                int dtp = (int)config["NotificationsService"]["PercentChanceSortByDeliveryTime"];
                int arp = (int)config["NotificationsService"]["PercentChanceSortByNotificationAcceptRatio"];
                int rtp = (int)config["NotificationsService"]["PercentChanceSortByNotificationReactionTime"];
                int dtarrt = dtp + arp + rtp;
                // Clone DeliverersPool for sorting
                List<Deliverer> deliverers = DeliverersPool;
                int n = rnd.Next(0, dtarrt);
                // Calculate sorting data based on number and sort percentages
                foreach (Deliverer d in deliverers)
                {
                    if (n >= 0 && n <= rtp)
                        d.SetAverageNotificationReactionTime();
                    else if (n > rtp && n <= (rtp + arp))
                        d.SetNotificationAcceptRatio();
                    else if (n > (rtp + arp) && n <= dtarrt)
                        d.SetAverageDeliveryTime();
                }
                // Based on number and sort percentages from variables.json, choose a sorting method
                if (n >= 0 && n <= rtp)
                {
                    _logger.Information("Sorting method: Notification reaction time.");
                    DeliverersPool = deliverers.OrderBy(x => x.AverageNotificationReactionTime).ToList();
                }
                else if (n > rtp && n <= (rtp + arp))
                {
                    _logger.Information("Sorting method: Notification acceptratio.");
                    DeliverersPool = deliverers.OrderByDescending(x => x.NotificationAcceptRatio).ToList();
                }
                else if (n > (rtp + arp) && n <= dtarrt)
                {
                    _logger.Information("Sorting method: Delivery time.");
                    DeliverersPool = deliverers.OrderBy(x => x.AverageDeliveryTime).ToList();
                }
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                DeliverersPool = null;
            }
        }

        /// <summary>
        /// Private method that selects a deliverer via a selection algorithm.
        /// </summary>
        /// <param name="config">Configuration of variables</param>
        /// <returns>A nullable Guid of the deliverer</returns>
        private Guid? SelectDeliverer(dynamic config)
        {
            try
            {
                _logger.Information($"A request has been made to select a deliverer from the pool. Poolseed pending...");
                // Get selection variables from variables.json document. Chance of multiple requests at the same time, so store the values locally and not as properties
                int ts = (int)config["NotificationsService"]["PercentChanceOfTopSelection"];
                int ms = (int)config["NotificationsService"]["PercentChanceOfMiddleSelection"];
                int bs = (int)config["NotificationsService"]["PercentChanceOfBottomSelection"];
                int bass = (int)config["NotificationsService"]["BreakAtSelectionSize"];
                int tmbs = ts + ms + bs;
                double tr = (double)config["NotificationsService"]["TopRangeFromTotalInPercentage"] / 100;
                double mr = (double)config["NotificationsService"]["MiddleRangeFromTotalInPercentage"] / 100;
                double br = (double)config["NotificationsService"]["BottomRangeFromTotalInPercentage"] / 100;
                // Initialize local variables
                Deliverer chosenOne;
                int n, i, c; // random number, index and count for removing certain deliverers from the pool
                // Loop through list, selecting a section each time until there are 5 or less deliverers in the list
                while (DeliverersPool.Count() > bass)
                {
                    n = rnd.Next(0, tmbs);
                    if (n >= 0 && n <= ts)
                    {   // i equals the starting index in the lis, c equals the count of items after i
                        i = (int)Math.Ceiling((DeliverersPool.Count - 1) * tr); // Zero-based, so -1
                        c = DeliverersPool.Count - i;
                        // Remove bottom 75% of list
                        DeliverersPool.RemoveRange(i, c);
                    }
                    else if (n > ts && n <= (ts + ms))
                    {
                        c = (int)Math.Ceiling((DeliverersPool.Count - 1) * tr); // Zero-based, so -1
                        // Get the deliverer that marks the start of the bottom section
                        Deliverer d = DeliverersPool[(int)Math.Ceiling((DeliverersPool.Count - 1) * (tr + mr))];
                        // Remove top 25% of list
                        DeliverersPool.RemoveRange(0, c);
                        // Remove bottom 25% of list before the removal of the top, marked by variable d
                        DeliverersPool.RemoveRange(DeliverersPool.IndexOf(d), (DeliverersPool.Count() - DeliverersPool.IndexOf(d)));
                    }
                    else if (n > (ts + ms) && n <= tmbs)
                    {
                        c = (int)Math.Ceiling((DeliverersPool.Count - 1) * (tr + mr));
                        // Remove top 75% of list
                        DeliverersPool.RemoveRange(0, c);
                    }
                }
                // Randomly choose one deliverer out of remaining selection and return the identifier
                chosenOne = DeliverersPool[rnd.Next(0, DeliverersPool.Count() - 1)];
                _logger.Information($"Chosen deliverer has poolseed: {DeliverersPool.IndexOf(chosenOne)}.");
                return chosenOne.Id;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"INotificationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
