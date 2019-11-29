using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Bob van Beek (610685) */
    public interface IDeliveriesRepository // Contract for DeliveriesRepository, repository may be changed
    {
        Task<Delivery> AddDelivery(Delivery delivery);
        bool DeleteDelivery(Delivery delivery);
        Task<List<Delivery>> GetAllDeliveries();
        Task<Delivery> GetCurrentDeliveryForDeliverer(Guid delivererId);
        Task<List<Delivery>> GetDeliveriesForUser(Guid delivererId);
        Task<Delivery> GetDelivery(Guid id);
        Delivery UpdateDelivery(Delivery delivery);
    }
}