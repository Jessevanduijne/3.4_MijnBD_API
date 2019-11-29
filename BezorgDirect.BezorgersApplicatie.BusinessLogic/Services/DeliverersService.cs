using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class DeliverersService : IDeliverersService // DeliverersService should contain everything in contract IDeliverersService
    {
        private readonly IDeliverersRepository _deliverersRepo;
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="deliverersRepo"></param>
        /// <param name="logger"></param>
        public DeliverersService(IDeliverersRepository deliverersRepo, ILogger logger)
        {
            this._deliverersRepo = deliverersRepo;
            this._logger = logger;
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
                _logger.Information($"A request has been made to create a Deliverers pool from the available deliverers for a delivery from the context.");
                List<Deliverer> d = _deliverersRepo.GetDeliverersPool(v, dueDate, config);
                return d ?? throw new ArgumentNullException($"Dependency failure: The repository result returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                _logger.Information($"A request has been made to get a Deliverer with token {token} from the context.");
                Deliverer d = await _deliverersRepo.GetDelivererByToken(token);
                return d ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
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
                _logger.Information($"A request has been made to get a Deliverer with id {id} from the context.");
                Deliverer d = await _deliverersRepo.GetDelivererById(id);
                return d ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves all Deliverers.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Deliverer class or null</returns>
        public async Task<List<Deliverer>> GetDeliverers()
        {
            try
            {
                _logger.Information($"A request has been made to get all Deliverers from the context.");
                List<Deliverer> d = await _deliverersRepo.GetDeliverers();
                return d ?? throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IDeliverersService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
