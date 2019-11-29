using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.Tests
{
    /* This class was setup and written by Lennart de Waart (563079) */
    [TestClass]
    public class EnumsTest
    {
        [TestMethod]
        public void GetDisplayNameTest()
        {
            Vehicles vehicle = Vehicles.Auto;
            string displayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            Assert.IsTrue(displayName == "Auto");
        }
    }
}
