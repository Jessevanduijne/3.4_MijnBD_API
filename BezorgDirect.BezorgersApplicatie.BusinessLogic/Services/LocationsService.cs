using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Entities.Maps.Geocoding;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class LocationsService : ILocationsService // LocationsService should contain everything in contract ILocationsService
    {
        private readonly ILogger _logger;
        private readonly ILocationsRepository _locationsRepo;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// <param name="logger"></param>
        /// </summary>
        public LocationsService(ILocationsRepository locationsRepo, ILogger logger)
        {
            _locationsRepo = locationsRepo;
            this._logger = logger;
        }
        
        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method that calculates the distance between two locations in double format.
        /// Code source: https://github.com/sahgilbert/parallel-haversine-formula-dotnetcore
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Distance between two locations in double format or 0</returns>
        public double CalculateVastDistanceInKilometers(LatLng from, LatLng to)
        {
            try
            {
                _logger.Information("A request has been made to calculate the vast distance in kilometers between two points.");
                const double EquatorialRadiusOfEarth = 6371D;
                const double DegreesToRadians = (Math.PI / 180D);

                var deltalat = (to.Latitude - from.Latitude) * DegreesToRadians;
                var deltalong = (to.Longitude - from.Longitude) * DegreesToRadians;

                var a = Math.Pow(
                    Math.Sin(deltalat / 2D), 2D) +
                    Math.Cos(from.Latitude * DegreesToRadians) *
                    Math.Cos(to.Latitude * DegreesToRadians) *
                    Math.Pow(Math.Sin(deltalong / 2D), 2D);

                var c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));

                var d = EquatorialRadiusOfEarth * c;

                return d;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return 0;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method which indicated whether a home point, warehouse point and customer point all lay inside a given range.
        /// </summary>
        /// <param name="home"></param>
        /// <param name="warehouse"></param>
        /// <param name="customer"></param>
        /// <param name="range"></param>
        /// <returns>true or false</returns>
        public bool IsDeliveryInDeliverersRange(LatLng home, LatLng warehouse, LatLng customer, int range)
        {
            /* The distance from the home location of the deliverer to the warehouse, as well as
               the distance from the warehouse to the customer and the home location of the deliverer to the customer
               have to be in the selected range of a deliverer */
            if (range < CalculateVastDistanceInKilometers(home, warehouse) ||
                range < CalculateVastDistanceInKilometers(warehouse, customer) ||
                range < CalculateVastDistanceInKilometers(home, customer))
                return false;
            else
                return true;
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public recipe method for retrieving a complete Location class based on an address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isWarehouse">Is the address a warehouse?</param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Models.Location> GetLocation(string address, bool isWarehouse)
        {
            try
            {
                _logger.Information($"A request has been made to get the location for the address {address} from the Google Maps API.");
                // Get Google Maps API-key from JSON-file
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
                LatLng latLng = await GetLatLongFromAddress(address, apiKey);
                if (latLng == null) throw new ArgumentNullException($"Dependency failure: GetLatLongFromAddress returned null for address {address}.");
                AddressData a = await GetAddressFromLatLong(latLng, apiKey);
                return a != null
                    ? new Models.Location(Guid.NewGuid(), latLng.Latitude, latLng.Longitude, address, a.PostalCode, a.Place, isWarehouse)
                    : throw new ArgumentNullException($"Dependency failure: GetAddressFromLatLong returned null for latitude {latLng.Latitude} and longitude {latLng.Longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Get latitude and longitude from an address.
        /// GoogleApi source: https://github.com/vivet/GoogleApi
        /// </summary>
        /// <param name="address"></param>
        /// <param name="apiKey"></param>
        /// <returns>Filled LatLng class or null</returns>
        public async Task<LatLng> GetLatLongFromAddress(string address, string apiKey)
        {
            try
            {
                _logger.Information($"A request has been made to get the latitude and longitude of the address {address} from the Google Geocode API.");
                // Create a GeocodeRequest
                AddressGeocodeRequest a = new AddressGeocodeRequest
                {
                    Address = address,
                    Key = apiKey
                };
                // POST request to Google Maps API
                GeocodeResponse r = await GoogleMaps.AddressGeocode.QueryAsync(a);
                // Response contains a Geometry class
                Geometry result = r.Results.ToList()[0].Geometry;
                // Geometry contains a Location class consisting of a latitude and longitude
                return result != null && result.Location != null
                    ? new LatLng(result.Location.Latitude, result.Location.Longitude)
                    : throw new ArgumentNullException($"Dependency failure: Google Geocode API request returned null for address {address}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public asynchronous method for retrieving the address from the latitude and longitude.
        /// GoogleApi source: https://github.com/vivet/GoogleApi
        /// </summary>
        /// <param name="latLng"></param>
        /// <param name="apiKey"></param>
        /// <returns>Filled AddressData class or null</returns>
        public async Task<AddressData> GetAddressFromLatLong(LatLng latLng, string apiKey)
        {
            try
            {
                _logger.Information($"A request has been made to get the addres of latitude {latLng.Latitude} and longitude {latLng.Longitude} from the Google Geocode API.");
                // Create a GeocodeRequest
                LocationGeocodeRequest l = new LocationGeocodeRequest
                {
                    Location = new GoogleApi.Entities.Common.Location(latLng.Latitude, latLng.Longitude),
                    Key = apiKey
                };
                // POST request to Google Maps API
                GeocodeResponse r = await GoogleMaps.LocationGeocode.QueryAsync(l);
                // Response contains a list of addresses around the location, get the first address and get the address components
                List<AddressComponent> results = r.Results.ToList()[0].AddressComponents.ToList();
                // Google API response results differs in length, so to correctly assign the properties of AddressData in a foreach loop
                AddressData a = AssignAddressDataPropertiesFromGoogle(results);
                // AddressComponent consists of multiple parts of the address, 0 equals the home number, 1 equals the street, 6 equals the postal code and 2 equals the place
                return a ?? throw new ArgumentNullException($"Dependency failure: Google Geocode API request returned null for lat {latLng.Latitude} and long {latLng.Longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// This private method creates a filled AddressData class from a list of AddressComponents usualy generated by the Google Geocode API.
        /// </summary>
        /// <param name="results"></param>
        /// <returns>Filled AddressData or null</returns>
        private AddressData AssignAddressDataPropertiesFromGoogle(List<AddressComponent> results)
        {
            try
            {
                AddressData a = new AddressData();
                if (results != null && results.Count > 0)
                {                    
                    foreach (AddressComponent aC in results)
                    {
                        if (aC.Types.ToList()[0].ToString() == "Route")
                            a.Address = aC.LongName;
                        else if (aC.Types.ToList()[0].ToString() == "Street_Number")
                            a.HouseNumber = aC.LongName;
                        else if (aC.Types.ToList()[0].ToString() == "Postal_Code")
                            a.PostalCode = aC.LongName;
                        else if (aC.Types.ToList()[0].ToString() == "Locality")
                            a.Place = aC.LongName;
                    }
                    a.Address = a.Address + " " + a.HouseNumber;                    
                }
                return !string.IsNullOrEmpty(a.Address) ? a
                        : throw new ArgumentNullException($"Address property returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public asynchronous method that returns the optimal Leg class between two points.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="vehicle"></param>
        /// <param name="departureTime">Time when person starts driving</param>
        /// <param name="apiKey"></param>
        /// <returns>Filled Leg class or null</returns>
        public async Task<Leg> TravelTo(LatLng from, LatLng to, Vehicles vehicle, DateTime departureTime, string apiKey)
        {
            try
            {
                _logger.Information($"A request has been made to get the Leg of a route between latFrom {from.Latitude}, longFrom {from.Longitude}" +
                    $"and latTo {to.Latitude}, longTo {to.Longitude} from the Google Directions API.");
                // Set travel mode based on vehicle parameter
                TravelMode t = TravelMode.Driving;
                if (vehicle == Vehicles.Fiets) t = TravelMode.Bicycling;
                // Create a DirectionsRequest
                DirectionsRequest d = new DirectionsRequest
                {
                    TravelMode = t,
                    DepartureTime = departureTime,
                    Origin = new GoogleApi.Entities.Common.Location(from.Latitude, from.Longitude),
                    Destination = new GoogleApi.Entities.Common.Location(to.Latitude, to.Longitude),
                    Key = apiKey
                };
                // POST request to Google Directions API
                DirectionsResponse r = await GoogleMaps.Directions.QueryAsync(d);
                // Response contains a list of routes to the destination. A route contains multiple legs of ways to get to the destination. Get the first (and best)
                Leg results = r.Routes.ToList()[0].Legs.ToList()[0];
                return results ?? throw new Exception($"Dependency failure: Google Directions API request returned null for latFrom " +
                    $"{from.Latitude}, longFrom {from.Longitude} to latTo {to.Latitude}, longTo {to.Longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public asynchronous method that calls the TravelTo method to gather a Leg class and then returns the estimated arrival time
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="vehicle"></param>
        /// <param name="departureTime">Time when person starts driving</param>
        /// <param name="apiKey"></param>
        /// <returns>ETA in DateTime or DateTime.MinValue</returns>
        public async Task<DateTime> GetETA(LatLng from, LatLng to, Vehicles vehicle, DateTime departureTime, string apiKey)
        {
            try
            {
                _logger.Information($"A request has been made to get the ETA of a route between latFrom {from.Latitude}, longFrom {from.Longitude}" +
                    $"and latTo {to.Latitude}, longTo {to.Longitude} from the Google Directions API.");
                Leg l = await TravelTo(from, to, vehicle, departureTime, apiKey);
                return l != null
                    ? departureTime.AddSeconds(0)
                    : throw new Exception($"Dependency failure: TravelTo returned null for latFrom " +
                    $"{from.Latitude}, longFrom {from.Longitude} to latTo {to.Latitude}, longTo {to.Longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return DateTime.MinValue;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public asynchronous method that calls the TravelTo method to gather a Leg class and then returns the distance in kilometers between two points.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="vehicle"></param>
        /// <param name="departureTime">Time when person starts driving</param>
        /// <param name="apiKey"></param>
        /// <returns>Double or 0</returns>
        public async Task<double> GetDistanceInKilometers(LatLng from, LatLng to, Vehicles vehicle, DateTime departureTime, string apiKey)
        {
            try
            {
                _logger.Information($"A request has been made to get the distance in kilometers of a route between latFrom {from.Latitude}, longFrom {from.Longitude}" +
                    $"and latTo {to.Latitude}, longTo {to.Longitude} from the Google Directions API.");
                Leg l = await TravelTo(from, to, vehicle, departureTime, apiKey);
                return l == null
                    ? l.Distance.Value / 1000 // Distance is measured in meters
                    : throw new Exception($"Dependency failure: TravelTo returned null for latFrom " +
                    $"{from.Latitude}, longFrom {from.Longitude} to latTo {to.Latitude}, longTo {to.Longitude}.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return 0;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
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
                _logger.Information($"A request has been made to add a Location with latitude {latitude} and longitude {longitude} to the context.");
                bool result = await _locationsRepo.AddLocation(id, latitude, longitude, address, postalCode, place, isWarehouse);
                return result ? result : throw new Exception($"Dependency failure: The repository returned false.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Asynchronous method that gets a location based on the given parameters.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Models.Location> GetLocationByLatLong(double latitude, double longitude, bool isWarehouse)
        {
            try
            {
                _logger.Information($"A request has been made to get the Location with latitude {latitude} and longitude {longitude} from the context.");
                Models.Location l = await _locationsRepo.GetLocationByLatLong(latitude, longitude, isWarehouse);
                if (l != null) return l;
                else throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Asynchronous method that gets a location based on the passed parameters.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isWarehouse"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Models.Location> GetLocationByAddress(string address, bool isWarehouse)
        {
            try
            {
                _logger.Information($"A request has been made to get the Location with address {address} from the context.");
                Models.Location l = await _locationsRepo.GetLocationByAddress(address, isWarehouse);
                if (l != null) return l;
                else throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
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
                _logger.Information($"A request has been made to check if a Location with latitude {latitude} and longitude {longitude} exists in the context.");
                bool result = await _locationsRepo.DoesLocationExist(latitude, longitude, isWarehouse);
                return result;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
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
                _logger.Information($"A request has been made to check if a Location with address {address} exists in the context.");
                bool result = await _locationsRepo.DoesLocationExist(address, isWarehouse);
                return result;
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Asynchronous method that retrieves a location based on the given identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filled Location class or null</returns>
        public async Task<Models.Location> GetLocationById(Guid id)
        {
            try
            {
                _logger.Information($"A request has been made to get a Location with id {id} from the context.");
                Models.Location l = await _locationsRepo.GetLocationById(id);
                if (l != null) return l;
                else throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which retrieves all Warehouses
        /// </summary>
        /// <returns>Filled Delivery class or null</returns>
        public async Task<List<Models.Location>> GetAllWarehouses()
        {
            try
            {
                _logger.Information($"A request has been made to get all warehouses from the context.");
                List<Models.Location> l = await _locationsRepo.GetAllWarehouses();
                if (l != null) return l;
                else throw new ArgumentNullException($"Dependency failure: The repository returned null.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"ILocationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return null;
            }
        }

        /* This function was written by Bob van Beek (610685) */
        /// <summary>
        /// Asynchronous method which set all navigational properties of a Delivery based on a given address
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Filled Delivery class or null</returns>
        public async Task<Delivery> SetLocationFromAddress(dynamic data)
        {
            _logger.Information($"A request has been made to set the navigational Location properties of a Delivery in the context.");
            Delivery testDelivery = new Delivery(Guid.NewGuid(), (string)data?.customerPhoneNumber, (DateTime)data?.dueDate, (Vehicles)data?.vehicle, (string)data?.vehicleDisplayName, (double)data?.price, (PaymentMethods)data?.paymentMethod, (DeliveryStatus)data?.status);

            Models.Location customer = await GetLocation((string)data?.customerAddress, false);
            Models.Location warehouse = await GetLocation((string)data?.warehouseAddress, true);

            //Customer
            if (!await DoesLocationExist(customer.Address, false))
            {
                await AddLocation(customer.Id, customer.Latitude, customer.Longitude, customer.Address, customer.PostalCode, customer.Place, false);
                testDelivery.CustomerId = customer.Id;
            }
            else
            {
                Models.Location newloc = await GetLocationByLatLong(customer.Latitude, customer.Longitude, false);
                testDelivery.CustomerId = newloc.Id;
            }

            //Warehouse
            if (!await DoesLocationExist(warehouse.Address, true))
            {
                await _locationsRepo.AddLocation(warehouse.Id, warehouse.Latitude, warehouse.Longitude, warehouse.Address, warehouse.PostalCode, warehouse.Place, true);
                testDelivery.WarehouseId = warehouse.Id;
            }
            else
            {
                var newloc = await GetLocationByLatLong(warehouse.Latitude, warehouse.Longitude, true);
                testDelivery.WarehouseId = newloc.Id;
            }

            return testDelivery;
        }
    }
}
