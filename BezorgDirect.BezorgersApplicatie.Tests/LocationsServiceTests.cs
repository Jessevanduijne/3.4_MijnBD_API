using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using GoogleApi.Entities.Maps.Directions.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.Tests
{
    /* This class was setup and written by Lennart de Waart (563079) */
    [TestClass]
    public class LocationsServiceTests
    {
        private readonly ILocationsService ls = new LocationsService(null, new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($".../../../../../logs/Log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}",
                    rollingInterval: RollingInterval.Day)
                    .CreateLogger());

        [TestMethod]
        public void CalculateDistanceTest()
        {
            double d = ls.CalculateVastDistanceInKilometers(new LatLng(52.322404, 4.596926), new LatLng(52.389454, 4.612877)); // Bennebroek to Inholland Haarlem (7.53 KM, rounded down)
            Assert.IsTrue(d > 7.53 && d < 7.54);
        }

        [TestMethod]
        public async void GetLatLongFromAddressTest()
        {
            // Get Google Maps API-key from JSON-file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
            string address = "Lage Duin 35, Bennebroek";
            LatLng latLng = await ls.GetLatLongFromAddress(address, apiKey);
            Assert.IsTrue(latLng.IsValid());
        }

        [TestMethod]
        public async void GetAddressFromLatLongTest()
        {
            // Get Google Maps API-key from JSON-file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
            LatLng latLng = new LatLng(52.3220135, 4.5965738);
            AddressData a = await ls.GetAddressFromLatLong(latLng, apiKey);
            Assert.IsTrue(a.IsValid());
        }

        [TestMethod]
        public async void GetLocationTest()
        {
            string address = "Lage Duin 35, Bennebroek";
            Location l = await ls.GetLocation(address, false);
            Assert.IsTrue(l.IsValid());
        }

        [TestMethod]
        public async void TravelToTest()
        {
            // Get Google Maps API-key from JSON-file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
            LatLng from = new LatLng(52.322404, 4.596926);
            LatLng to = new LatLng(52.389454, 4.612877);
            Vehicles vehicle = Vehicles.Auto;
            DateTime departureTime = DateTime.Now;
            Leg l = await ls.TravelTo(from, to, vehicle, departureTime, apiKey);
            Assert.IsTrue(l != null);
        }

        [TestMethod]
        public async void GetETATest()
        {
            // Get Google Maps API-key from JSON-file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
            LatLng from = new LatLng(52.322404, 4.596926);
            LatLng to = new LatLng(52.389454, 4.612877);
            Vehicles vehicle = Vehicles.Auto;
            DateTime departureTime = DateTime.Now;
            DateTime d = await ls.GetETA(from, to, vehicle, departureTime, apiKey);
            Assert.IsTrue(d != DateTime.MinValue);
        }

        [TestMethod]
        public async void GetDistanceInKilometersTest()
        {
            // Get Google Maps API-key from JSON-file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string apiKey = config["LocationsService"]["GoogleMapsApiKey"];
            LatLng from = new LatLng(52.322404, 4.596926);
            LatLng to = new LatLng(52.389454, 4.612877);
            Vehicles vehicle = Vehicles.Auto;
            DateTime departureTime = DateTime.Now;
            double d = await ls.GetDistanceInKilometers(from, to, vehicle, departureTime, apiKey);
            Assert.IsTrue(d != 0);
        }
    }
}
