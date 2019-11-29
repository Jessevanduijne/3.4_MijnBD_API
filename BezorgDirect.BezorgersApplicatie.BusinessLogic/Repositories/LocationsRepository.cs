using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class LocationsRepository : ILocationsRepository // LocationsRepository should contain everything in contract ILocationsRepository
    {
        private readonly Context _context; // Not accessible outside this class and not editable
        private readonly ILogger _logger;

        /// <summary>
        /// Public constructor for FeedbackRepository
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public LocationsRepository(Context context, ILogger logger)
        {
            // Set context at initialization
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronous method that adds a Location object to the context.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="address"></param>
        /// <param name="postalCode"></param>
        /// <param name="place"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>true or false</returns>
        public async Task<bool> AddLocation(Guid id, double latitude, double longitude, string address, string postalCode, string place, bool isWarehouse)
        {
            try
            {
                await _context.Locations.AddAsync(new Location(id, latitude, longitude, address, postalCode.Replace(" ", ""), place, isWarehouse));
                if (_context.SaveChanges() > 0)
                    return true;
                else
                    throw new Exception("Context could not be updated.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /// <summary>
        /// Asynchronous method that gets a location based on the given parameters
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Location> GetLocationByLatLong(double latitude, double longitude, bool isWarehouse)
        {
            try
            {
                Location l = await _context.Locations.Where(x => x.Latitude == latitude && x.Longitude == longitude && x.IsWarehouse == isWarehouse).FirstOrDefaultAsync();
                if (l != null) return l;
                else throw new ArgumentNullException($"Could not find location with latitude {latitude} and longitude {longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that gets a location based on the passed parameters
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Location> GetLocationByAddress(string address, bool isWarehouse)
        {
            try
            {
                Location l = await _context.Locations.Where(x => x.Address == address && x.IsWarehouse == isWarehouse).FirstOrDefaultAsync();
                if (l != null) return l;
                else throw new ArgumentNullException($"Could not find location with address {address}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that checks if a location with the passed latitude and longitude exists in the database.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>true or false</returns>
        public async Task<bool> DoesLocationExist(double latitude, double longitude, bool isWarehouse)
        {
            try
            {
                Location l = await _context.Locations.Where(x => x.Latitude == latitude && x.Longitude == longitude && x.IsWarehouse == isWarehouse).FirstOrDefaultAsync();
                if (l != null) return true;
                else return false;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /// <summary>
        /// Asynchronous method that checks if a location with the passed address exists in the database.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>true or false</returns>
        public async Task<bool> DoesLocationExist(string address, bool isWarehouse)
        {
            try
            {
                Location l = await _context.Locations.Where(x => x.Address.Contains(address) && x.IsWarehouse == isWarehouse).FirstOrDefaultAsync();
                if (l != null) return true;
                else return false;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /// <summary>
        /// Asynchronous method that retrieves a location based on the given identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Location> GetLocationById(Guid id)
        {
            try
            {
                Location l = await _context.Locations.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (l != null) return l;
                else throw new ArgumentNullException($"Could not find a location with id {id} in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /// <summary>
        /// Asynchronous method that retrieves all warehouses.
        /// </summary>
        /// <returns>Filled Location class or null</returns>
        public async Task<List<Location>> GetAllWarehouses()
        {
            try
            {
                List<Location> l = await _context.Locations.Where(x => x.IsWarehouse == true).ToListAsync();
                if (l != null) return l;
                else throw new ArgumentNullException($"Could not find any warehouses in the context.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsRepository says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }
    }
}
