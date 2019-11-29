using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This interface was setup and written by Bob van Beek (610685) */
    public interface IDeliveriesService // Contract for DeliveriesService, repository may be changed
    {
        Task<Delivery> GetDelivery(Guid id);
        Task<Delivery> AddDelivery(Delivery delivery);
        Task<Delivery> UpdateDelivery(Delivery delivery);
        bool DeleteDelivery(Delivery delivery);
        Task<List<Delivery>> GetAllDeliveries();
        Task<Delivery> GetCurrentDeliveryForDeliverer(Guid delivererId);
        Task<List<Delivery>> GetDeliveriesForUser(Guid id);
        Task<Delivery> PatchDeliveryLocation(Delivery delivery, Location location);
        Task<Delivery> PatchDeliveryStatus(Delivery delivery, Enums.DeliveryStatus status, Guid delivererId);
        Task<Delivery> UpdateStatus(Delivery delivery, Guid delivererId);
    }
}