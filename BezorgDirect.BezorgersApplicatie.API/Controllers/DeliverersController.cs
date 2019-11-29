using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Newtonsoft.Json.Linq;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.Api.Controllers
{
    /* This class was setup by Tiamo Idzenga (582063) */
    public class DeliverersController
    {
        private readonly ILogger _logger;
        private readonly IDeliverersRepository _deliverersRepository;
        private readonly IAuthenticationsService _authService;
        private readonly IDeliverersService _deliverersService;

        public DeliverersController(
            ILogger logger,
            IDeliverersRepository deliverersRepository
        ) {
            _logger = logger;
            _deliverersRepository = deliverersRepository;
            _deliverersService = new DeliverersService(deliverersRepository, logger);
            _authService = new AuthenticationsService(logger);
        }

        [FunctionName("ReadMe")]
        public async Task<IActionResult> ReadMe(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "me")] 
                HttpRequest req
        ) {
            try {
                string token = _authService.getTokenFromHeader(req.Headers["Authorization"]);
                Deliverer deliverer = await _deliverersRepository.GetDelivererByToken(token);

                return new OkObjectResult(deliverer);
            } catch(Exception e) {
                _logger.Error(e.ToString());

                return new UnauthorizedResult();
            }
        }

        [FunctionName("UpdateMe")]
        public async Task<IActionResult> UpdateMe(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "me")]
                HttpRequest req
        ) {
            try {
                string token = _authService.getTokenFromHeader(req.Headers["Authorization"]);
                Deliverer deliverer = await _deliverersRepository.GetDelivererByToken(token);

                if (token == deliverer.Token) {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

                    deliverer.EmailAddress = (string)data["emailAddress"];
                    deliverer.PhoneNumber = (string)data["phoneNumber"];
                    deliverer.DateOfBirth = (DateTime)data["dateOfBirth"];
                    deliverer.Range = (int)data["range"];
                    deliverer.Vehicle = (Vehicles)(int)data["vehicle"];
                    deliverer.Fare = (double)data["fare"];

                    //TODO: Able to update Home

                    if (_deliverersRepository.Update(deliverer)) {
                        return new OkObjectResult(deliverer);
                    } else {
                        return new StatusCodeResult(500);
                    }
                } else {
                    throw new TokenException();
                }
            } catch (Exception e) {
                _logger.Error(e.ToString());

                if (e is TokenException) {
                    return new UnauthorizedResult();
                } else {
                    return new BadRequestResult();
                }
            }
        }

        /* This function was written by Bob van Beek (610685) */
        [FunctionName("GetDeliverers")]
        public async Task<HttpResponseMessage> GetDeliverers(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Deliverers")] HttpRequest req)
        {
            string token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_authService.IsTokenValid(token, false))
            {
                List<Deliverer> result = await _deliverersService.GetDeliverers();

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

    }
}
