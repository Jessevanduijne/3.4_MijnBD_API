using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Lennart de Waart (563079) */
    public interface ILocationsRepository // Contract for LocationsRepository, repository may be changed
    {
        Task<bool> AddLocation(Guid id, double latitude, double longitude, string address, string postalCode, string place, bool isWarehouse);
        Task<bool> DoesLocationExist(double latitude, double longitude, bool isWarehouse);
        Task<bool> DoesLocationExist(string address, bool isWarehouse);
        Task<Location> GetLocationByLatLong(double latitude, double longitude, bool isWarehouse);
        Task<Location> GetLocationByAddress(string address, bool isWarehouse);
        Task<Location> GetLocationById(Guid id);
        Task<List<Location>> GetAllWarehouses();
    }
}
