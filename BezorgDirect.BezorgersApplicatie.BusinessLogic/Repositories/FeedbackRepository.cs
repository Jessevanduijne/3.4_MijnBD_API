using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup and written by Bob van Beek (610685) */
    public class FeedbackRepository : IFeedbackRepository // FeedbackRepository should contain everything in contract IFeedbackRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for DeliveriesRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public FeedbackRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronous method that retrieves all Feedback records with a given deliveryId.
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <returns>List of Feedback classes or null</returns>
        public async Task<List<Feedback>> GetFeedbackByDeliveryId(Guid deliveryId)
        {
            try
            {
                List<Feedback> feedback = await _context.Feedback.Where(x => x.DeliveryId == deliveryId).ToListAsync();
                if (feedback != null && feedback.Count > 0)
                    return feedback;
                else throw new ArgumentNullException($"Could not retrieve feedback for delivery {deliveryId} from the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that add a Feedback record to the context.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns>Filled Feedback class or null</returns>
        public async Task<Feedback> AddFeedback(Feedback feedback)
        {
            try
            {
                await _context.Feedback.AddAsync(feedback);
                if (_context.SaveChanges() > 0)
                    return feedback;
                else throw new Exception($"Feedback with id {feedback.Id} could not be added to the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that deletes multiple Feedback records from the context.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns>Filled Feedback class or null</returns>
        public async Task<bool> DeleteFeedback(Guid id)
        {
            try
            {
                _context.RemoveRange(await _context.Feedback.Where(x => x.DeliveryId == id).ToListAsync());

                if (_context.SaveChanges() > 0)
                    return true;
                else throw new Exception($"Feedback from Delivery with id {id} could not be deleted to the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }
    }
}
