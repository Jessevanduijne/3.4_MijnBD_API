using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BezorgDirect.BezorgersApplicatie.Tests
{
    /* This class was setup and written by Lennart de Waart (563079) */
    [TestClass]
    public class NotificationsServiceTests
    {
        [TestMethod]
        public void IsSortingMethodEvenlyDistributed()
        {
            // Get sorting chance from variables.json document
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            int percentChanceSortByDeliveryTime = (int)config["NotificationAlgorithm"]["PercentChanceSortByDeliveryTime"];
            int percentChanceSortByNotificationAcceptRatio = (int)config["NotificationAlgorithm"]["PercentChanceSortByNotificationAcceptRatio"];
            int percentChanceSortByNotificationReactionTime = (int)config["NotificationAlgorithm"]["PercentChanceSortByNotificationReactionTime"];
            int chanceInTotal = percentChanceSortByNotificationReactionTime + percentChanceSortByNotificationAcceptRatio + percentChanceSortByDeliveryTime;
            // Initialize variables
            int choice;
            int loopCount = 100;
            int listCount = 100;
            Random rnd = new Random();
            List<double> percentageIsTop = new List<double>();
            List<double> percentageIsMiddle = new List<double>();
            List<double> percentageIsBottom = new List<double>();
            List<string> sortingMethods = new List<string>();            
            
            // Testscenario for sorting calls
            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < listCount; j++)
                {
                    choice = rnd.Next(0, chanceInTotal);
                    if (choice >= 0 && choice <= percentChanceSortByNotificationReactionTime) 
                        sortingMethods.Add("average notification reaction time"); // Represents sorting method 
                    else if (choice > percentChanceSortByNotificationReactionTime && choice <= (percentChanceSortByNotificationReactionTime + percentChanceSortByNotificationAcceptRatio)) 
                        sortingMethods.Add("notification acceptratio"); // Represents sorting method
                    else if (choice > (percentChanceSortByNotificationReactionTime + percentChanceSortByNotificationAcceptRatio) && choice <= chanceInTotal) 
                        sortingMethods.Add("average delivery time"); // Represents sorting method

                }

                // Calculate count of sortingmethods in list
                int t = sortingMethods.Where(x => x == "average notification reaction time").Count();
                int m = sortingMethods.Where(x => x == "notification acceptratio").Count();
                int b = sortingMethods.Where(x => x == "average delivery time").Count();                
                int c = sortingMethods.Count();
                // Calculate percentage of method from total
                double tc = (double)t / (double)c;
                double mc = (double)m / (double)c;
                double bc = (double)b / (double)c;
                // Add percentage to lists
                percentageIsTop.Add(tc);
                percentageIsMiddle.Add(mc);
                percentageIsBottom.Add(bc);
                // Clear sortingmethod so we can start all over again
                sortingMethods.Clear();
            }
            // Calculate averages of all lists
            double ta = percentageIsTop.Average();
            double ma = percentageIsMiddle.Average();
            double ba = percentageIsBottom.Average();
            double tmba = ta + ma + ba;
            // Assert that the choice of sortingmethod is evenly distributed
            Assert.IsTrue(ta >= 0.2 && ta <= 0.4 &&
                ma >= 0.2 && ma <= 0.4 &&
                ba >= 0.2 && ba <= 0.4 &&
                tmba > 0.999);
        }

        [TestMethod]
        public void IsSelectionMethodValid()
        {
            // Get selection variables from variables.json document
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            int percentChanceOfTopSelection = (int)config["NotificationAlgorithm"]["PercentChanceOfTopSelection"];
            int percentChanceOfMiddleSelection = (int)config["NotificationAlgorithm"]["PercentChanceOfMiddleSelection"];
            int percentChanceOfBottomSelection = (int)config["NotificationAlgorithm"]["PercentChanceOfBottomSelection"];
            double topRangeFromTotalInPercentage = (double)config["NotificationAlgorithm"]["TopRangeFromTotalInPercentage"] / 100;
            double middleRangeFromTotalInPercentage = (double)config["NotificationAlgorithm"]["MiddleRangeFromTotalInPercentage"] / 100;
            double bottomRangeFromTotalInPercentage = (double)config["NotificationAlgorithm"]["BottomRangeFromTotalInPercentage"] / 100;
            int chanceInTotal = percentChanceOfTopSelection + percentChanceOfMiddleSelection + percentChanceOfBottomSelection;
            // Initialize variables
            int choice, id;
            int loopCount = 100;
            int listCount = 100;
            Random rnd = new Random();
            List<int> selectedIds = new List<int>();

            // Testscenario for selecting a deliverer
            for (int i = 0; i <= loopCount; i++)
            {
                List<int> deliverersPool = new List<int>();
                for (int j = 1; j < (listCount + 1); j++)
                {
                    deliverersPool.Add(j);
                }

                if (i < loopCount)
                {
                    do
                    {
                        choice = rnd.Next(0, chanceInTotal);
                        if (choice >= 0 && choice <= percentChanceOfTopSelection)
                        {
                            int min = (int)Math.Ceiling((deliverersPool.Count - 1) * topRangeFromTotalInPercentage);
                            int max = deliverersPool.Count - min;
                            // Remove bottom 75% of list
                            deliverersPool.RemoveRange(min, max); // Represents selection method 
                        }
                        else if (choice > percentChanceOfTopSelection && choice <= (percentChanceOfTopSelection + percentChanceOfMiddleSelection))
                        {
                            // Remove top 25% of list
                            int min1 = 0;
                            int max1 = (int)Math.Ceiling((deliverersPool.Count - 1) * topRangeFromTotalInPercentage);
                            int min2 = deliverersPool[(int)Math.Ceiling((deliverersPool.Count - 1) * (topRangeFromTotalInPercentage + middleRangeFromTotalInPercentage))];
                            // Remove bottom 75% of list
                            deliverersPool.RemoveRange(min1, max1); // Represents selection method
                            deliverersPool.RemoveRange(deliverersPool.IndexOf(min2), (deliverersPool.Count() - deliverersPool.IndexOf(min2)));
                        }
                        else if (choice > (percentChanceOfTopSelection + percentChanceOfMiddleSelection) && choice <= chanceInTotal)
                        {
                            int min = 0;
                            int max = (int)Math.Ceiling((deliverersPool.Count - 1) * (topRangeFromTotalInPercentage + middleRangeFromTotalInPercentage));
                            // Remove top 75% of list
                            deliverersPool.RemoveRange(min, max); // Represents selection method
                        }
                    }
                    while (deliverersPool.Count() > 5);

                    id = deliverersPool[rnd.Next(0, deliverersPool.Count - 1)];
                    selectedIds.Add(id);
                }
                
                // When finished check the results
                if (i == loopCount)
                {
                    int t = selectedIds.Where(x => x <= (int)Math.Ceiling(deliverersPool.Count() * topRangeFromTotalInPercentage)).Count();
                    int m = selectedIds.Where(x => x > (int)Math.Ceiling(deliverersPool.Count() * topRangeFromTotalInPercentage) && x < (int)Math.Ceiling(deliverersPool.Count() * (topRangeFromTotalInPercentage + middleRangeFromTotalInPercentage))).Count();
                    int b = selectedIds.Where(x => x >= (int)Math.Ceiling(deliverersPool.Count() * (topRangeFromTotalInPercentage + middleRangeFromTotalInPercentage))).Count();
                    int tmbc = t + m + b;

                    // Assert that the selection method is valid by checking the selection ratios
                    Assert.IsTrue(t > Math.Ceiling(loopCount * topRangeFromTotalInPercentage) && t < loopCount &&
                        m >= 0 && m <= Math.Ceiling(loopCount * middleRangeFromTotalInPercentage) &&
                        b >= 0 &&  b <= Math.Ceiling(loopCount * middleRangeFromTotalInPercentage) &&
                        tmbc == loopCount);
                }
            }            
        }
    }
}

