using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Bob van Beek (610685) */
    public interface IFeedbackRepository // Contract for FeedbackRepository, service may be changed
    {
        Task<List<Feedback>> GetFeedbackByDeliveryId(Guid deliveryId);
        Task<Feedback> AddFeedback(Feedback feedback);
        Task<bool> DeleteFeedback(Guid id);
    }
}
