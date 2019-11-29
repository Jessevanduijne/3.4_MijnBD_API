using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class AvailabilitiesService : IAvailabilitiesService // AvailabilitiesService should contain everything in contract IAvailabilitiesService
    {
        private readonly IAvailabilitiesRepository _availabilitiesRepo;
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="availabilitiesRepo"></param>
        /// <param name="logger"></param>
        public AvailabilitiesService(IAvailabilitiesRepository availabilitiesRepo, ILogger logger)
        {
            this._availabilitiesRepo = availabilitiesRepo;
            this._logger = logger;
        }
        
        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// Public method that deletes a availability record from the database
        /// The record that is deleted is given to the function parameter
        /// </summary>
        /// <param name="availability"></param>
        /// <returns>
        /// A HttpResponseMessage that can be given to the caller
        /// </returns>
        public async Task<bool> DeleteAvailability(Guid id)
        {
            try
            {
                _logger.Information($"A request has been made to delete the Availability with id {id} from the context.");
                Availability a = await _availabilitiesRepo.GetAvailabilityById(id);
                bool result = _availabilitiesRepo.DeleteAvailability(a);
                return result ? result : throw new Exception($"Dependency failure: The repository returned false.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// Public method that retrieves a list of all Availability records in the database.
        /// </summary>
        /// <param name="delivererId"></param>
        /// <returns>List of Availability classes or null</returns>
        public async Task<List<Availability>> GetAllAvailabilitiesOfDeliverer(Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to get all Availability records of Deliverer {delivererId} from the context.");
                List<Availability> a = await _availabilitiesRepo.GetAllAvailabilitiesOfDeliverer(delivererId);
                return a != null && a.Count > 0 ? a
                    : throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// This function selects a availability record from the database from its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The availability record that belongs to the id of this functions parameter.
        /// If none matches it, it will return null
        /// </returns>
        public async Task<Availability> GetAvailabilityById(Guid id)
        {
            try
            {
                _logger.Information($"A request has been made to get the Availability record with id {id} from the context.");
                Availability a = await _availabilitiesRepo.GetAvailabilityById(id);
                return a ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// This function posts a new availability record to the database
        /// The parameter of this function contains the new database record
        /// </summary>
        /// <param name="availability"></param>
        /// <returns>
        /// The availability record that was posted without its navigational properties
        /// </returns>
        public async Task<Availability> AddAvailability(Availability availability)
        {
            try
            {
                _logger.Information($"A request has been made to add an Availability with id {availability.Id} to the context.");
                Availability a = await _availabilitiesRepo.AddAvailability(availability);
                return a ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// This function updates a availability record in the database
        /// The parameter of this function contains the updated database record
        /// </summary>
        /// <param name="availability"></param>
        /// <returns>
        /// The availability model without the navigational properties
        /// </returns>
        public Availability UpdateAvailability(Availability availability)
        {
            try
            {
                _logger.Information($"A request has been made to update the Availability record with id {availability.Id} in the context.");
                Availability a = _availabilitiesRepo.UpdateAvailability(availability);
                return a ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// Asynchronous method that adds multiple Availability records to the context.
        /// </summary>
        /// <param name="availabilities"></param>
        /// <param name="delivererId"></param>
        /// <returns>List of Availability classes or null</returns>
        public async Task<List<Availability>> AddAvailabilities(List<Availability> availabilities, Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to add multiple Availability records for deliverer {delivererId} to the context.");
                List<Availability> result = new List<Availability>();
                foreach (Availability availability in availabilities)
                {
                    availability.DelivererId = delivererId;
                    result.Add(await AddAvailability(availability));
                }
                return result != null && result.Count > 0 ? result 
                    : throw new ArgumentNullException($"Dependency failure: AddAvailability returned null.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Mats Webbers (484491) */
        /// <summary>
        /// Asynchronous method that updates multiple Availability records in the context.
        /// </summary>
        /// <param name="availabilities"></param>
        /// <param name="delivererId"></param>
        /// <returns>List of Availability classes or null</returns>
        public List<Availability> UpdateAvailabilities(List<Availability> availabilities, Guid delivererId)
        {
            try
            {
                _logger.Information($"A request has been made to update multiple Availability records for deliverer {delivererId} to the context.");
                List<Availability> result = new List<Availability>();
                foreach (Availability availability in availabilities)
                {
                    if (availability.DelivererId == delivererId)
                        result.Add(UpdateAvailability(availability));
                    else
                        _logger.Warning("The Availability with id: " + availability.Id + " does not belong to the authenticated in user!");
                }
                return result != null && result.Count > 0 ? result
                    : throw new ArgumentNullException($"Dependency failure: UpdateAvailability returned null.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
