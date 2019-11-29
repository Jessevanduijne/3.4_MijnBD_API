using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using Serilog;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;
using Newtonsoft.Json.Linq;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup by Tiamo Idzenga (582063) */
    public class AuthController
    {
        private readonly ILogger _logger;
        private readonly IDeliverersRepository _deliverersRepository;
        private readonly ILocationsService _locationsService;
        private readonly IAuthenticationsService _authenticationsService;

        public AuthController(
            ILogger logger,
            IDeliverersRepository deliverersRepository,
            ILocationsRepository locationsRepository
        ) {
            _logger = logger;
            _deliverersRepository = deliverersRepository;
            _locationsService = new LocationsService(locationsRepository, logger);
            _authenticationsService = new AuthenticationsService(logger);
        }

        //can pull this into LocationsService
        private string getLocationsServiceApiKey()
        {
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText(
                "../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"
            ));

            return config["LocationsService"]["GoogleMapsApiKey"];
        }

        [FunctionName("Register")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] 
                HttpRequest req
        ) {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

            try {
                //need to get Location is this weird way because GetLocation in LocationService didnt work properly
                AddressData homeAdress = new AddressData(
                    (string)data["home"]["address"],
                    ((string)data["home"]["postalCode"]).Replace(" ", ""),
                    (string)data["home"]["place"]
                );

                LatLng homePoint = await _locationsService.GetLatLongFromAddress(
                    homeAdress.AsString(), getLocationsServiceApiKey()
                );

                Deliverer deliverer = new Deliverer(
                    (string)data["emailAddress"],
                    (string)data["password"],
                    (DateTime)data["dateOfBirth"],
                    (double)data["fare"],
                    new Location(homeAdress, homePoint),
                    (string)data["phoneNumber"],
                    (int)data["range"],
                    (Vehicles)(int)data["vehicle"]
                );

                if (_deliverersRepository.Create(deliverer)) {
                    return new OkObjectResult(deliverer);
                } else {
                    return new StatusCodeResult(500);
                }
            } catch (Exception e) {
                _logger.Error(e.ToString());

                return new BadRequestResult();
            }
        }

        [FunctionName("Login")]
        public async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")] 
                HttpRequest req
        ) {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

            Deliverer deliverer = _deliverersRepository.Read((string)data["emailAddress"]);

            if (
                deliverer is object &&
                !deliverer.IsLoggedIn() &&
                deliverer.PasswordMatches((string)data["password"])
            ) {
                deliverer.LogIn();

                if (_deliverersRepository.Update(deliverer)) {
                    return new OkObjectResult(deliverer);
                } else {
                    return new StatusCodeResult(500);
                }
            } else {
                return new UnauthorizedResult();
            }
        }

        [FunctionName("Logout")]
        public async Task<IActionResult> Logout(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "logout")]
                HttpRequest req
        ) {
            try {
                string token = _authenticationsService.getTokenFromHeader(req.Headers["Authorization"]);

                Deliverer deliverer = await _deliverersRepository.GetDelivererByToken(token);
                deliverer.LogOut();

                if (_deliverersRepository.Update(deliverer)){
                    return new OkResult();
                } else {
                    return new StatusCodeResult(500);
                }
            } catch(Exception e) {
                _logger.Error(e.ToString());

                return new UnauthorizedResult();
            }
        }
    }
}
