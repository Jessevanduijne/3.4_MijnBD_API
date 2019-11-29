using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using Serilog;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup by Lennart de Waart (563079) */
    public class AvailabilitiesController
    {
        private readonly IAvailabilitiesService _availabilitiesService;
        private readonly IAuthenticationsService _authorizationsService;
        private readonly IDeliverersService _deliverersService;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the AvailabilitiesController class.
        /// </summary>
        /// <param name="availabilitiesRepo"></param>
        /// <param name="deliverersRepo"></param>
        /// <param name="logger"></param>
        public AvailabilitiesController(IAvailabilitiesRepository availabilitiesRepo, IDeliverersRepository deliverersRepo, ILogger logger)
        {            
            _logger = logger;
            _deliverersService = new DeliverersService(deliverersRepo, _logger);
            _availabilitiesService = new AvailabilitiesService(availabilitiesRepo, _logger);
            _authorizationsService = new AuthenticationsService(_logger);
        }

        /* This function was written by Mats Webbers (484491) */
        /// <summary>
        /// This function gets the call to find all the availabilities of the user that the bearer token belongs to
        /// It sends a request to the repository to find these records
        /// </summary>
        /// <param name="req"></param>
        /// <returns>
        /// A list of availability objects without their navigational properties
        /// </returns>
        [FunctionName("Availabilities")]
        public async Task<HttpResponseMessage> GetAllAvailabilities(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Availabilities")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                List<Availability> availabilities = await _availabilitiesService.GetAllAvailabilitiesOfDeliverer(deliverer.Id);
                return availabilities != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(availabilities), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This function was written by Mats Webbers (484491) */
        /// <summary>
        /// This function posts a new availability to the database
        /// It uses the bearer token to determine the logged in user
        /// It uses this user his id for the deliverer id of the new availability post
        /// If there is no bearer token or its not in the database it will return a unauthorized request error
        /// It sends a request to the repository to add a new record to the database
        /// </summary>
        /// <param name="req"></param>
        /// <returns>
        /// A list of the posted availability records without their navigational properties
        /// </returns>
        [FunctionName("PostAvailabilities")]
        public async Task<HttpResponseMessage> PostAvailability(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Availabilities")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore                
                List<Availability> availabilities = JsonConvert.DeserializeObject<List<Availability>>(requestBody);
                if (availabilities == null || availabilities.Count == 0)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                List<Availability> result = await _availabilitiesService.AddAvailabilities(availabilities, deliverer.Id);
                return result != null && result.Count > 0
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
                    }
                    : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This function was written by Mats Webbers (484491) */
        /// <summary>
        /// This function deletes a availability record from the database
        /// It uses the string parameter to find the corrosponding availability record and deletes it if its found on the database
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns>
        /// A statuscode to determine if it was successfull or not
        /// </returns>
        [FunctionName("DeleteAvailabilities")]
        public async Task<HttpResponseMessage> DeleteAvailability(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Availabilities/{id}")] HttpRequest req, string id)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                bool result = await _availabilitiesService.DeleteAvailability(Guid.Parse(id));
                return result
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    : new HttpResponseMessage(HttpStatusCode.FailedDependency);
            }
            // Authorized access only
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /* This function was written by Mats Webbers (484491) */
        /// <summary>
        /// This function updates a list of availability records in the database
        /// It receives these records from the body of the request
        /// Uses the repository to update these records
        /// If one of the given records does not belong to the logged in user it is ignored and NOT updated
        /// </summary>
        /// <param name="req"></param>
        /// <returns>
        /// A list of updated availability records without their navigational properties
        /// </returns>
        [FunctionName("PutAvailabilities")]
        public async Task<HttpResponseMessage> PutAvailability(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Availabilities")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer deliverer = await _deliverersService.GetDelivererByToken(token);
                if (deliverer == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                StreamReader r = new StreamReader(req.Body);
                string requestBody = await r.ReadToEndAsync();                
                r.Dispose(); // Dispose StreamReader so it cannot be used anymore
                List<Availability> availabilities = JsonConvert.DeserializeObject<List<Availability>>(requestBody);                
                if (availabilities == null || availabilities.Count == 0)
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                List<Availability> result = _availabilitiesService.UpdateAvailabilities(availabilities, deliverer.Id);
                return result != null
                     ? new HttpResponseMessage(HttpStatusCode.OK)
                     {
                         Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
                     } : new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
