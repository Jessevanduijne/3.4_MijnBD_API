using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Mats Webbers (484491) */
    public interface IAvailabilitiesService // Contract for AvailabilitiesService, service may be changed
    {
        Task<Availability> AddAvailability(Availability availability);
        Task<bool> DeleteAvailability(Guid id);
        Task<List<Availability>> GetAllAvailabilitiesOfDeliverer(Guid delivererId);
        Task<Availability> GetAvailabilityById(Guid id);
        Availability UpdateAvailability(Availability availability);
        Task<List<Availability>> AddAvailabilities(List<Availability> availabilities, Guid delivererId);
        List<Availability> UpdateAvailabilities(List<Availability> availabilities, Guid delivererId);
    }
}