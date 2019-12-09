using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using System.Net.Http;
using System.Net;
using System.Text;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using Serilog;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;
using System.Collections.Generic;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup by Lennart de Waart (563079) */
    public class DeliveriesController
    {
        private readonly INotificationsService _notificationsService;
        private readonly IDeliverersService _deliverersService;
        private readonly IDeliveriesService _deliveriesService;
        private readonly IAuthenticationsService _authorizationsService;
        private readonly ILocationsService _locationsService;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the DeliveriesController class.
        /// </summary>
        /// <param name="notificationsRepo"></param>
        /// <param name="deliveriesRepo"></param>
        /// <param name="deliverersRepo"></param>
        /// <param name="locationsRepo"></param>
        /// <param name="feedbackRepo"></param>
        /// <param name="logger"></param>
        public DeliveriesController(INotificationsRepository notificationsRepo, IDeliveriesRepository deliveriesRepo, IDeliverersRepository deliverersRepo, ILocationsRepository locationsRepo, IFeedbackRepository feedbackRepo, ILogger logger)
        {
            _logger = logger;
            _notificationsService = new NotificationsService(deliverersRepo, deliveriesRepo, notificationsRepo, locationsRepo, _logger);
            _deliverersService = new DeliverersService(deliverersRepo, _logger);
            _authorizationsService = new AuthenticationsService(_logger);
            _deliveriesService = new DeliveriesService(deliveriesRepo, locationsRepo, _logger);
            _locationsService = new LocationsService(locationsRepo, _logger);
            _feedbackService = new FeedbackService(feedbackRepo, _logger);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to retrieve a Delivery based on a given id. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("GetDelivery")]
        public async Task<HttpResponseMessage> GetDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Deliveries/{id}")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Delivery d = await _deliveriesService.GetDelivery(Guid.Parse(id));
                return d != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(d, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to add a Delivery record to the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("PostDelivery")]
        public async Task<HttpResponseMessage> AddDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Deliveries")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                // Get Delivery from JSON object
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore    
                dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);

                Delivery filledDelivery = await _locationsService.SetLocationFromAddress(data);
                // Add Delivery
                Delivery d = await _deliveriesService.AddDelivery(filledDelivery);
                // Creating new notification
                bool result = await _notificationsService.CreateNotification(d.Id, d.Vehicle, d.DueDate,
                    new LatLng(d.Warehouse.Latitude, d.Warehouse.Longitude), new LatLng(d.Customer.Latitude, d.Customer.Longitude));
                if (!result)
                    return new HttpResponseMessage(HttpStatusCode.FailedDependency);
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../variables.json"));
                int expirationInSeconds = ((int)config["NotificationsService"]["ExpirationOfNotificationInMinutes"] * 60);
                // Create task in scheduler to check for an expired Notification record
                Scheduler.Scheduler.Instance().ScheduleJob("CheckNotificationContextForExpiration",
                    "trigger_CheckNotificationContextForExpiration", DateTime.Now, (expirationInSeconds+60), 0);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(d), Encoding.UTF8, "application/json")
                };
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to update a Delivery record in the context.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("PutDelivery")]
        public async Task<HttpResponseMessage> UpdateDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Deliveries/{id}")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore 
                Delivery result = await _deliveriesService.UpdateDelivery(JsonConvert.DeserializeObject<Delivery>(requestBody));
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to delete a Delivery record from the context.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("DeleteDelivery")]
        public async Task<HttpResponseMessage> DeleteDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Deliveries/{id}")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                Delivery d = await _deliveriesService.GetDelivery(Guid.Parse(id));
                bool result = _deliveriesService.DeleteDelivery(d);
                return result
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to retrieve all Delivery records from the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetAllDeliveries")]
        public async Task<HttpResponseMessage> GetAllDeliveries(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Deliveries/admin")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                List<Delivery> result = await _deliveriesService.GetAllDeliveries();
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with a valid token to retrieve a list of Delivery records for a Deliverer from the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetDeliveriesForUser")]
        public async Task<HttpResponseMessage> GetDeliveriesForUser(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Deliveries")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                List<Delivery> result = await _deliveriesService.GetDeliveriesForUser(deliverer.Id);
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with a valid token to get the current Delivery record for a Deliverer from the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetCurrentDelivery")]
        public async Task<HttpResponseMessage> GetCurrentDelivery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Delivery")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                Delivery result = await _deliveriesService.GetCurrentDeliveryForDeliverer(deliverer.Id);
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with a valid token to patch a location to a Delivery record in the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("PatchDeliveryLocation")]
        public async Task<HttpResponseMessage> PatchDeliveryLocation(
            [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "Delivery/{id}/Location")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                // Getting data from JSON and putting it in a LatLng object
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore
                dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);

                //Get info and fron data and parameter
                LatLng latLng = new LatLng((double)data?.latitude, (double)data?.longitude);
                Guid deliveryId = Guid.Parse(id);

                // Get Google API key
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                string apiKey = config["LocationsService"]["GoogleMapsApiKey"];

                // Creating location object from LatLng object and AddressData object
                AddressData addressData = await _locationsService.GetAddressFromLatLong(latLng, apiKey);
                Location l = new Location(Guid.NewGuid(), latLng.Latitude, latLng.Longitude, addressData.Address, addressData.PostalCode, addressData.Place, false);

                // If Location exists, replace it with existing one
                if (await _locationsService.DoesLocationExist(latLng.Latitude, latLng.Longitude, false))
                    l = await _locationsService.GetLocationByLatLong(latLng.Latitude, latLng.Longitude, false);
                // Getting logged in deliverer and the CurrentDelivery to patch
                Delivery result = await _deliveriesService.PatchDeliveryLocation(await _deliveriesService.GetDelivery(deliveryId), l);

                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with a valid token to patch the status of a Delivery record in the context.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("PatchDeliveryStatus")]
        public async Task<HttpResponseMessage> PatchDeliveryStatus(
                    [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "Delivery/{id}/Status")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore

                dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);

                //Get info and fron data and parameter
                DeliveryStatus status = (DeliveryStatus)Enum.ToObject(typeof(DeliveryStatus), (int)data?.status);
                LatLng latLng = new LatLng((double)data?.latitude, (double)data?.longitude);
                Guid deliveryId = Guid.Parse(id);

                // Get Google API key
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                string apiKey = config["LocationsService"]["GoogleMapsApiKey"];

                // Creating location object from LatLng object and AddressData object
                AddressData addressData = await _locationsService.GetAddressFromLatLong(latLng, apiKey);
                Location l = new Location(Guid.NewGuid(), latLng.Latitude, latLng.Longitude, addressData.Address, addressData.PostalCode, addressData.Place, false);

                // If Location exists, replace it with existing one
                if (await _locationsService.DoesLocationExist(latLng.Latitude, latLng.Longitude, false))
                    l = await _locationsService.GetLocationByLatLong(latLng.Latitude, latLng.Longitude, false);
                // Getting delivery with new Current Location before calculating ETA's and setting status
                Delivery deliveryWithLocation = await _deliveriesService.PatchDeliveryLocation(await _deliveriesService.GetDelivery(deliveryId), l);

                // Patch the delivery status
                Delivery result = await _deliveriesService.PatchDeliveryStatus(deliveryWithLocation, status, deliverer.Id);
                //If status is reset to Aangekondigd, create a new Notification
                if (status == DeliveryStatus.Afgemeld)
                {
                    bool success = await _notificationsService.CreateNotification(result.Id, result.Vehicle, result.DueDate,
                        new LatLng(result.Warehouse.Latitude, result.Warehouse.Longitude), new LatLng(result.Customer.Latitude, result.Customer.Longitude));
                    if (!success)
                        return new HttpResponseMessage(HttpStatusCode.FailedDependency);
                    var expirationInSeconds = ((int)config["NotificationsService"]["ExpirationOfNotificationInMinutes"] * 60);
                    // Create task in scheduler to check for an expired Notification record
                    Scheduler.Scheduler.Instance().ScheduleJob("CheckNotificationContextForExpiration",
                        "trigger_CheckNotificationContextForExpiration", DateTime.Now, (expirationInSeconds+60), 0);
                }

                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to retrieve all the Feedback records for a Delivery from the context.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("GetFeedback")]
        public async Task<HttpResponseMessage> GetFeedback(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Deliveries/{id}/Feedback")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                List<Feedback> result = await _feedbackService.GetFeedbackByDeliveryId(Guid.Parse(id));
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result, new JsonSerializerSettings()
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to add Feedback records for a Delivery to the context.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("PostFeedback")]
        public async Task<HttpResponseMessage> PostFeedback(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Deliveries/{id}/Feedback")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore
                List<Feedback> feedback = JsonConvert.DeserializeObject<List<Feedback>>(requestBody);
                List<Feedback> result = await _feedbackService.AddFeedback(feedback);
                return result != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This method was written by Bob van Beek (610685) */
        /// <summary>
        /// This function allows a user with an admin token to delete Feedback records for a Delivery to the context.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("DeleteFeedback")]
        public async Task<HttpResponseMessage> DeleteFeedback(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Deliveries/{id}/Feedback")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, true))
            {
                var result = await _feedbackService.DeleteFeedback(Guid.Parse(id));
                return result == true
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}