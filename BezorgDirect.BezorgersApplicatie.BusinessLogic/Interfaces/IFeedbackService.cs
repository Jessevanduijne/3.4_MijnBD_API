using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Bob van Beek (610685) */
    public interface IFeedbackService // Contract for FeedbackService, service may be changed
    {
        Task<Feedback> AddFeedback(Feedback feedback);
        Task<List<Feedback>> GetFeedbackByDeliveryId(Guid deliveryId);
        Task<List<Feedback>> AddFeedback(List<Feedback> feedback);
        Task<bool> DeleteFeedback(Guid id);
    }
}