using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using GoogleApi.Entities.Maps.Directions.Response;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Lennart de Waart (563079) */
    public interface ILocationsService // Contract for LocationsService, service may be changed
    {
        Task<List<Location>> GetAllWarehouses();
        Task<bool> AddLocation(Guid id, double latitude, double longitude, string address, string postalCode, string place, bool isWarehouse);
        double CalculateVastDistanceInKilometers(LatLng from, LatLng to);
        Task<bool> DoesLocationExist(double latitude, double longitude, bool isWarehouse);
        Task<bool> DoesLocationExist(string address, bool isWarehouse);
        Task<AddressData> GetAddressFromLatLong(LatLng latLng, string apiKey);
        Task<double> GetDistanceInKilometers(LatLng from, LatLng to, Enums.Vehicles vehicle, DateTime departureTime, string apiKey);
        Task<DateTime> GetETA(LatLng from, LatLng to, Enums.Vehicles vehicle, DateTime departureTime, string apiKey);
        Task<LatLng> GetLatLongFromAddress(string address, string apiKey);
        Task<Location> GetLocation(string address, bool isWarehouse);
        Task<Location> GetLocationByAddress(string address, bool isWarehouse);
        Task<Location> GetLocationById(Guid id);
        Task<Location> GetLocationByLatLong(double latitude, double longitude, bool isWarehouse);
        bool IsDeliveryInDeliverersRange(LatLng home, LatLng warehouse, LatLng customer, int range);
        Task<Delivery> SetLocationFromAddress(dynamic data);
        Task<Leg> TravelTo(LatLng from, LatLng to, Enums.Vehicles vehicle, DateTime departureTime, string apiKey);
    }
}