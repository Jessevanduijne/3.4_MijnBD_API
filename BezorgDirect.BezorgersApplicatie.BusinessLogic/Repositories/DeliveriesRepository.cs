using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Serilog;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup by Lennart de Waart (563079) */
    public class DeliveriesRepository : IDeliveriesRepository // DeliveriesRepository should contain everything in contract IDeliveriesRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for DeliveriesRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public DeliveriesRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
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
                Delivery d = await _context.Deliveries.FindAsync(id);
                return d ?? throw new ArgumentNullException($"Could not retrieve a Delivery with id {id} from the context. Such an entity does not exist in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                await _context.Deliveries.AddAsync(delivery);
                if (_context.SaveChanges() > 0)
                    return delivery;
                else throw new Exception($"Could not add Delivery with id {delivery.Id} to the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Public method that updates a Delivery record in the context.
        /// </summary>
        /// <param name="delivery"></param>
        /// <returns></returns>
        public Delivery UpdateDelivery(Delivery delivery)
        {
            try
            {
                _context.Deliveries.Update(delivery);
                if (_context.SaveChanges() > 0)
                    return delivery;
                else throw new Exception($"Could not update Delivery with id {delivery.Id} in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                _context.Deliveries.Remove(delivery);
                if (_context.SaveChanges() > 0)
                    return true;
                else throw new Exception($"Could not delete Delivery with id {delivery.Id} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                List<Delivery> deliveries = await _context.Deliveries.ToListAsync();
                return deliveries != null && deliveries.Count > 0
                    ? deliveries
                    : throw new ArgumentNullException("Could not retrieve all Delivery records from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                Delivery currentDelivery = await _context.Deliveries.Where(x => x.DeliveredAt == null && x.DelivererId == delivererId).FirstOrDefaultAsync();
                return currentDelivery ?? throw new ArgumentNullException($"Could not retrieve the current Delivery for Deliverer {delivererId} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves all deliveries for an Deliverer from the context.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Delivery>> GetDeliveriesForUser(Guid delivererId)
        {
            try
            {
                List<Delivery> deliveries = await _context.Deliveries.Where(x => x.DelivererId == delivererId).ToListAsync();
                return deliveries != null && deliveries.Count > 0
                    ? deliveries
                    : throw new ArgumentNullException($"Could not retrieve all Delivery records for Deliverer {delivererId} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliveriesRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
