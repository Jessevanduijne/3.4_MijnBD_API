using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using Serilog;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System.Net.Http;
using System.Net;
using System.Text;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using System.Diagnostics;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class NotificationsController
    {
        private readonly INotificationsService _notificationsService;
        private readonly IDeliverersService _deliverersService;
        private readonly IAuthenticationsService _authorizationsService;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the NotificationsController class.
        /// </summary>
        /// <param name="notificationsRepo"></param>
        /// <param name="deliverersRepo"></param>
        /// <param name="locationsRepo"></param>
        /// <param name="logger"></param>
        public NotificationsController(INotificationsRepository notificationsRepo, IDeliveriesRepository deliveriesRepo, IDeliverersRepository deliverersRepo, ILocationsRepository locationsRepo, ILogger logger)
        {            
            _logger = logger;
            _notificationsService = new NotificationsService(deliverersRepo, deliveriesRepo, notificationsRepo, locationsRepo, _logger);
            _deliverersService = new DeliverersService(deliverersRepo, _logger);
            _authorizationsService = new AuthenticationsService(_logger);
        }

        /// <summary>
        /// This endpoint functions as a controller to get the first open Notification record for a deliverer.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Unauthorized if token is invalid</returns>
        /// <returns>No Content if no open Notifications for the authenticated user were found</returns>
        /// <returns>OK if a Notification for the authenticated user was found</returns>
        [FunctionName("GetNotification")]
        public async Task<HttpResponseMessage> GetNotification(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "notification")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer d = await _deliverersService.GetDelivererByToken(token);
                if (d == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                Notification n = await _notificationsService.GetFirstOpenNotificationByDelivererId(d.Id);
                return n != null
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(n, new JsonSerializerSettings()
                        {   // Ignore loops in properties of the class
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }), Encoding.UTF8, "application/json")
                    } : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            _logger.Warning($"GetNotification was invoked by an unauthorized entity.\n");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// This endpoint functions as a controller to patch a specific Notification record.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Unauthorized if token is invalid</returns>
        /// <returns>BadRequest if the body parameters are invalid</returns>
        /// <returns>FailedDependency if the Notification record couldn't be patched</returns>
        /// <returns>OK if the Notification record was patched</returns>
        [FunctionName("PatchNotification")]
        public async Task<HttpResponseMessage> PatchNotification(
            [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "notifications/{id}")] HttpRequest req, string id)
        {
            try
            {
                string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (_authorizationsService.IsTokenValid(token, false))
                {
                    Deliverer d = await _deliverersService.GetDelivererByToken(token);
                    if (d == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    // Get notification accepted/not accepted boolean and notificationId from requestBody
                    StreamReader r = new StreamReader(req.Body);
                    string requestBody = await r.ReadToEndAsync();
                    r.Dispose(); // Dispose StreamReader so it cannot be used anymore
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);
                    Guid? notificationId = Guid.Parse(id);
                    bool accepted = (bool)data?.accepted;
                    // Check dependency variables
                    if (!notificationId.HasValue) return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    bool result = await _notificationsService.UpdateNotificationStatusById(notificationId.Value, accepted);
                    if (!result) return new HttpResponseMessage(HttpStatusCode.FailedDependency);
                    dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                    var expirationInSeconds = ((int)config["NotificationsService"]["ExpirationOfNotificationInMinutes"] * 60);
                    // Create task in scheduler to check for an expired Notification record
                    Scheduler.Scheduler.Instance().ScheduleJob("CheckNotificationContextForExpiration",
                        "trigger_CheckNotificationContextForExpiration", DateTime.Now, 90, 0);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                // Authorized access only
                _logger.Warning($"PatchNotification was invoked by an unauthorized entity.\n");
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"NotificationsController says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return new HttpResponseMessage(HttpStatusCode.FailedDependency);
            }
        }

        /// <summary>
        /// This endpoint serves as a controller to update expired notification records.
        /// This function should only be called upon by a task in the scheduler.
        /// Therefore, only a unique access token will allow you to execute this method.
        /// This function was created due to the failure of DI of the NotificationsRepository in the Job of the Scheduler.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Unauthorized if token is invalid</returns>
        /// <returns>FailedDependency if the Notification records couldn't be patched</returns>
        /// <returns>OK if the Notification records were patched</returns>
        [FunctionName("UpdateExpiredNotifications")]
        public async Task<HttpResponseMessage> UpdateExpiredNotifications(
           [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "updateExpiredNotifications")] HttpRequest req)
        {
            string token = req.Query["Authorization"].ToString();
            if (_authorizationsService.IsTokenValid(token, false))
            {
                // Get unique access token from variables.json file and compare it to the included token
                dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../variables.json"));
                if (token == (string)config["Scheduler"]["AuthToken"])
                {
                    bool r = await _notificationsService.UpdateExpiredNotifications();
                    return r == true
                        ? new HttpResponseMessage(HttpStatusCode.OK)
                        : new HttpResponseMessage(HttpStatusCode.FailedDependency);
                }
            }
            // Authorized access only
            _logger.Warning($"UpdateExpiredNotifications was invoked by an unauthorized entity.\n");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);            
        }
    }
}
