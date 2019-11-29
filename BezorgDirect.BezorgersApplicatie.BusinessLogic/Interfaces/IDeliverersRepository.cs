using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup by Lennart de Waart (563079) */
    public interface IDeliverersRepository // Contract for DeliverersRepository, repository may be changed
    {
        List<Deliverer> GetDeliverersPool(Vehicles v, DateTime dueDate, dynamic config);
        Task<Deliverer> GetDelivererByToken(string token);
        Task<Deliverer> GetDelivererById(Guid id);
        bool Create(Deliverer deliverer);
        Deliverer Read(string email);
        bool Update(Deliverer updatedDeliverer);
        Task<List<Deliverer>> GetDeliverers();
    }
}
