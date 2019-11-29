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

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup by Lennart de Waart (563079) */
    public class AvailabilitiesRepository : IAvailabilitiesRepository // AvailabilitiesRepository should contain everything in contract IAvailabilitiesRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for AvailabilitiesRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public AvailabilitiesRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
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
        public bool DeleteAvailability(Availability availability)
        {
            try
            {
                _context.Availabilities.Remove(availability);
                if (_context.SaveChanges() > 0)
                    return true;
                else
                    throw new Exception($"Could not remove availability with id {availability.Id} from the context.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                List<Availability> a = await _context.Availabilities.Where(x => x.DelivererId == delivererId).ToListAsync();
                if (a != null && a.Count > 0)
                    return a;
                else throw new ArgumentNullException($"Could not retrieve the availability records of deliverer {delivererId} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IAvailabilitiesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                Availability a = await _context.Availabilities.FindAsync(id);
                if (a != null) return a;
                else throw new ArgumentNullException($"Could not retrieve the Availability record with id {id}. There is no such entity in the context.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                await _context.Availabilities.AddAsync(availability);
                if (_context.SaveChanges() > 0)
                    return availability;
                else
                    throw new Exception($"Could not add availability with id {availability.Id} to the context.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                _context.Update(availability);
                if (_context.SaveChanges() > 0)
                    return availability;
                else
                    throw new Exception($"Could not update availability with id {availability.Id} in the context.");
            }
            catch (Exception e)
            {
                _logger.Error($"IAvailabilitiesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}