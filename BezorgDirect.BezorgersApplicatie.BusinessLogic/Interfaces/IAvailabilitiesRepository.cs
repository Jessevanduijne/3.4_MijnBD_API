using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Mats Webbers (484491) */
    public interface IAvailabilitiesRepository // Contract for AvailabilitiesRepository, repository may be changed
    {
        Task<Availability> AddAvailability(Availability availability);
        bool DeleteAvailability(Availability availability);
        Task<List<Availability>> GetAllAvailabilitiesOfDeliverer(Guid delivererId);
        Task<Availability> GetAvailabilityById(Guid id);
        Availability UpdateAvailability(Availability availability);
    }
}