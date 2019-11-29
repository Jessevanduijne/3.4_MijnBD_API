using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class FeedbackService : IFeedbackService // FeedbackService should contain everything in contract IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="feedbackRepo"></param>
        public FeedbackService(IFeedbackRepository feedbackRepo, ILogger logger)
        {
            this._logger = logger;
            this._feedbackRepo = feedbackRepo;
        }       

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method that retrieves all Feedback records with a given deliveryId.
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <returns>List of Feedback classes or null</returns>
        public async Task<List<Feedback>> GetFeedbackByDeliveryId(Guid deliveryId)
        {
            try
            {
                _logger.Information($"A request has been made to get the Feedback records of Delivery {deliveryId} from the context.");
                List<Feedback> feedback = await _feedbackRepo.GetFeedbackByDeliveryId(deliveryId);
                if (feedback != null && feedback.Count > 0)
                    return feedback;
                else throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method that add a Feedback record to the context.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns>Filled Feedback class or null</returns>
        public async Task<Feedback> AddFeedback(Feedback feedback)
        {
            try
            {
                _logger.Information($"A request has been made to add a Feedback record with id {feedback.Id} to the context.");
                feedback.Id = Guid.NewGuid();
                Feedback f = await _feedbackRepo.AddFeedback(feedback);
                return feedback ?? throw new Exception($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method that adds multiple Feedback records to the context.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns>List of Feedback classes or null</returns>
        public async Task<List<Feedback>> AddFeedback(List<Feedback> feedback)
        {
            try
            {
                _logger.Information($"A request has been made to add multiple Feedback records to the context.");
                List<Feedback> processedFeedback = new List<Feedback>();
                foreach (Feedback f in feedback)
                {
                    processedFeedback.Add(await AddFeedback(f));
                }
                return processedFeedback != null && processedFeedback.Count > 0 
                    ? processedFeedback
                    : throw new Exception($"Dependency failure: AddFeedback returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that deletes multiple Feedback records to the context.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFeedback(Guid id)
        {
            try
            {
                _logger.Information($"A request has been made to delete all Feedback records for delivery with id {id} to the context.");
                await _feedbackRepo.DeleteFeedback(id);
                return true;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IFeedbackService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }
    }
}
