using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using Serilog;
using System.Net.Http;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class LocationsController
    {
        private readonly ILocationsService _locationsService;
        private readonly IAuthenticationsService _authorizationsService;
        private readonly IDeliverersService _deliverersService;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the LocationsController class.
        /// </summary>
        /// <param name="locationsRepo"></param>
        /// <param name="logger"></param>
        public LocationsController(ILocationsRepository locationsRepo, IDeliverersRepository deliverersRepo, ILogger logger)
        {
            _logger = logger;
            _locationsService = new LocationsService(locationsRepo, _logger);
            _deliverersService = new DeliverersService(deliverersRepo, _logger);
            _authorizationsService = new AuthenticationsService(_logger);
        }

        /// <summary>
        /// This endpoint functions as a controller to get a list of all the warehouses for an authenticated user.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Unauthorized if token is invalid</returns>
        /// <returns>No Content if no warehouses were found</returns>
        /// <returns>OK if warehouses were found</returns>
        [FunctionName("GetAllWarehouses")]
        public async Task<HttpResponseMessage> GetAllWarehouses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Locations/Warehouses")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authorizationsService.IsTokenValid(token, false))
            {
                Deliverer d = await _deliverersService.GetDelivererByToken(token);
                if (d == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                List<Location> warehouses = await _locationsService.GetAllWarehouses();
                return warehouses != null && warehouses.Count > 0
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(warehouses), Encoding.UTF8, "application/json")
                    } : new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // Authorized access only
            _logger.Warning($"GetAllWarehouses was invoked by an unauthorized entity.\n");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
