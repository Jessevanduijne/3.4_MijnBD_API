using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class DeliveriesService : IDeliveriesService // DeliveriesService should contain everything in contract IDeliveriesService
    {
        private readonly ILogger _logger;
        private readonly IDeliveriesRepository _deliveriesRepository;
        private readonly ILocationsService _locationsService;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="deliveriesRepo"></param>
        /// <param name="locationsRepo"></param>
        /// <param name="logger"></param>
        public DeliveriesService(IDeliveriesRepository deliveriesRepo, ILocationsRepository locationsRepo, ILogger logger)
        {
            _logger = logger;
            _deliveriesRepository = deliveriesRepo;            
            _locationsService = new LocationsService(locationsRepo, _logger);            
        }       

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous recepe method for updating the status of a Delivery in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="delivererId"></param>
        /// <returns>Filled Delivery class or null</returns>
        public async Task<Delivery> UpdateStatus(Delivery delivery, Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to update the status of Delivery {delivery.Id} in the context.");
                switch (delivery.Status)
                {
                    case DeliveryStatus.Afgemeld:
                        delivery = SetCanceled(delivery);
                        break;

                    case DeliveryStatus.Bevestigd:
                        delivery = await SetWarehouseData(delivery, delivererId);

                        break;

                    case DeliveryStatus.Onderweg:
                        delivery = await SetCustomerData(delivery);
                        break;

                    case DeliveryStatus.Afgeleverd:
                        delivery = SetFinalData(delivery);
                        break;
                }
                await UpdateDelivery(delivery);

                return delivery ?? throw new ArgumentNullException("Delivery is null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Private method to set a Delivery in cancellation mode.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        private Delivery SetCanceled(Delivery delivery)
        {
            delivery.CurrentId = null;
            delivery.CustomerETA = null;
            delivery.WarehouseETA = null;
            delivery.WarehouseDistanceInKilometers = null;
            delivery.WarehousePickUpAt = null;
            delivery.StartedAtId = null;
            delivery.DeliveredAt = null;
            delivery.CustomerDistanceInKilometers = null;
            delivery.DelivererId = null;
            delivery.Status = Enums.DeliveryStatus.Aangekondigd;

            return delivery;
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method to set the properties for a delivery status change to confirmed.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="delivererId"></param>
        /// <returns></returns>
        private async Task<Delivery> SetWarehouseData(Delivery delivery, Guid delivererId)
        {
            try
            {
                // Get Google API key
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
                // Calculate and set the WarehouseETA
                delivery.WarehouseETA = await _locationsService.GetETA(new LatLng(delivery.Current.Latitude, delivery.Current.Longitude), new LatLng(delivery.Warehouse.Latitude, delivery.Warehouse.Longitude), delivery.Vehicle, DateTime.Now, apiKey);
                // Set the distance between the Current location and Warehouse
                delivery.WarehouseDistanceInKilometers = _locationsService.CalculateVastDistanceInKilometers(new LatLng(delivery.Current.Latitude, delivery.Current.Longitude), new LatLng(delivery.Warehouse.Latitude, delivery.Warehouse.Longitude));
                // Set the startedAtId
                delivery.StartedAtId = delivery.CurrentId;
                delivery.DelivererId = delivererId;
                return delivery != null && delivery.WarehouseETA != DateTime.MinValue && delivery.WarehouseDistanceInKilometers > 0
                    ? delivery
                    : throw new ArgumentNullException("Dependency failure: Required properties of Delivery weren't filled.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method to set the properties for a delivery status change to driving.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        private async Task<Delivery> SetCustomerData(Delivery delivery)
        {
            try
            {
                // Get Google API key
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
                // Calculate and set the CustomerETA
                delivery.CustomerETA = await _locationsService.GetETA(new LatLng(delivery.Warehouse.Latitude, delivery.Warehouse.Longitude), new LatLng(delivery.Customer.Latitude, delivery.Customer.Longitude), delivery.Vehicle, DateTime.Now, apiKey);
                // Set the distance between the Warehouse and Customer
                delivery.CustomerDistanceInKilometers = _locationsService.CalculateVastDistanceInKilometers(new LatLng(delivery.Warehouse.Latitude, delivery.Warehouse.Longitude), new LatLng(delivery.Customer.Latitude, delivery.Customer.Longitude));
                // Set the pickuptime at the Warehouse
                delivery.WarehousePickUpAt = DateTime.Now;
                return delivery != null && delivery.CustomerETA != DateTime.MinValue && delivery.CustomerDistanceInKilometers > 0
                        ? delivery
                        : throw new ArgumentNullException("Dependency failure: Required properties of Delivery weren't filled.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method to set the properties for a delivery status change to delivered.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        private Delivery SetFinalData(Delivery delivery)
        {
            // Set the delivered time at the Customer
            delivery.DeliveredAt = DateTime.Now;
            return delivery;
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method for getting a single delivery with a given id from the context.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Delivery> GetDelivery(Guid id)
        {
            try
            {
                _logger.Information($"A request has been made to retieve a Delivery with id {id} from the context.");
                Delivery d = await _deliveriesRepository.GetDelivery(id);
                if (d == null) throw new ArgumentNullException("Dependency failure: The repository returned null.");
                Delivery d2 = await AggregateLocations(d);
                return d2 ?? throw new ArgumentNullException("Dependency failure: AggregateLocations returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method that adds new Delivery record to the context. (Try to create a workaround for getting the new delivery based on Id)
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        public async Task<Delivery> AddDelivery(Delivery delivery)
        {
            try
            {
                _logger.Information($"A request has been made to add a Delivery to the context.");
                Delivery d = await _deliveriesRepository.AddDelivery(delivery);
                if (d == null) throw new ArgumentNullException("Dependency failure: The repository returned null.");
                Delivery d2 = await AggregateLocations(d);
                return d2 ?? throw new ArgumentNullException("Dependency failure: AggregateLocations returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Public method that updates a Delivery record in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        public async Task<Delivery> UpdateDelivery(Delivery delivery)
        {
            try
            {
                _logger.Information($"A request has been made to update a Delivery record with id {delivery.Id} in the context.");
                Delivery d = _deliveriesRepository.UpdateDelivery(delivery);
                if (d == null) throw new ArgumentNullException("Dependency failure: The repository returned null.");
                Delivery d2 = await AggregateLocations(d);
                return d2 ?? throw new ArgumentNullException("Dependency failure: AggregateLocations returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Public method for deleting a single delivery in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns>true or false</returns>
        public bool DeleteDelivery(Delivery delivery)
        {
            try
            {
                _logger.Information($"A request has been made to delete a Delivery with id {delivery.Id} from the context.");
                bool result = _deliveriesRepository.DeleteDelivery(delivery);
                return result ? result : throw new Exception("Dependency failure: The repository returned false.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method for getting all Deliveries from the context.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Delivery>> GetAllDeliveries()
        {
            try
            {
                _logger.Information($"A request has been made to get all Delivery records from the context.");
                List<Delivery> deliveries = await _deliveriesRepository.GetAllDeliveries();
                foreach (Delivery d in deliveries)
                {
                    await AggregateLocations(d);
                }
                return deliveries != null && deliveries.Count > 0
                    ? deliveries
                    : throw new ArgumentNullException("Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves the current Delivery of a Deliverer from the context.
        /// </summary>
        /// <param name="delivererId"></param>
        /// <returns></returns>
        public async Task<Delivery> GetCurrentDeliveryForDeliverer(Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to get the current Delivery for Deliverer {delivererId} from the context.");
                Delivery d = await _deliveriesRepository.GetCurrentDeliveryForDeliverer(delivererId);
                if (d == null) throw new ArgumentNullException("Dependency failure: The repository returned null");
                Delivery d2 = await AggregateLocations(d);
                return d2 ?? throw new ArgumentNullException("Dependency failure: AggregateLocations returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves all deliveries for an Deliverer from the context.
        /// </summary>
        /// <param name="delivererId"></param>
        /// <returns></returns>
        public async Task<List<Delivery>> GetDeliveriesForUser(Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to get all Delivery records for Deliverer {delivererId} from the context.");
                List<Delivery> deliveries = await _deliveriesRepository.GetDeliveriesForUser(delivererId);
                foreach (Delivery d in deliveries)
                {
                    await AggregateLocations(d);
                }
                return deliveries != null && deliveries.Count > 0
                    ? deliveries
                    : throw new ArgumentNullException("Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which patches a Location record for current Delivery in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<Delivery> PatchDeliveryLocation(Delivery delivery, Location location)
        {
            try
            {
                _logger.Information($"A request has been made to patch the location of Delivery {delivery.Id} in the context.");
                bool exists = await _locationsService.DoesLocationExist(location.Latitude, location.Longitude, location.IsWarehouse);
                if (!exists)
                    await _locationsService.AddLocation(location.Id, location.Latitude, location.Longitude, location.Address, location.PostalCode, location.Place, location.IsWarehouse);
                delivery.CurrentId = location.Id;
                delivery = await UpdateDelivery(delivery);
                return delivery ?? throw new ArgumentNullException("Dependency failure: Delivery is null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method that patches the status of the current Delivery in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Delivery> PatchDeliveryStatus(Delivery delivery, DeliveryStatus status, Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to patch the status of Delivery {delivery.Id} in the context.");
                delivery.Status = status;

                switch (status)
                {
                    case DeliveryStatus.Aangekondigd:
                        delivery.StatusDisplayName = "Aangekondigd";
                        break;
                    case DeliveryStatus.Afgeleverd:
                        delivery.StatusDisplayName = "Afgeleverd";
                        break;
                    case DeliveryStatus.Onderweg:
                        delivery.StatusDisplayName = "Onderweg";
                        break;
                    case DeliveryStatus.Bevestigd:
                        delivery.StatusDisplayName = "Bevestigd";
                        break;
                }
                delivery = await UpdateStatus(delivery, delivererId);
                //delivery = await UpdateDelivery(delivery);
                return delivery;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        private async Task<Delivery> AggregateLocations(Delivery delivery)
        {
            try
            {
                delivery.Warehouse = await _locationsService.GetLocationById(delivery.WarehouseId);
                delivery.Customer = await _locationsService.GetLocationById(delivery.CustomerId);
                if (delivery.CurrentId != null)
                    delivery.Current = await _locationsService.GetLocationById(delivery.CurrentId.Value);
                return delivery != null && delivery.Warehouse != null && delivery.Customer != null
                    ? delivery
                    : throw new ArgumentNullException("Dependency failure: The required properties weren't filled.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
