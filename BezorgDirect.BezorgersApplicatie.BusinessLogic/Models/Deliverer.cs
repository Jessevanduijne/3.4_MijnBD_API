using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;
using System.Linq;
using Newtonsoft.Json;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup by Lennart de Waart (563079) */
    public class Deliverer : User // Deliverer inherits from the User class
    {
        [Required, StringLength(10)]
        public string PhoneNumber { get; set; } // NOT NULL

        [Required]
        public Guid HomeId { get; set; } // NOT NULL

        [Required]
        public DateTime DateOfBirth { get; set; } // NOT NULL

        [Required]
        public int Range { get; set; } // NOT NULL

        [Required]
        public Vehicles Vehicle { get; set; } // NOT NULL

        [NotMapped] // Not in database
        public string VehicleDisplayName { get; set; } // IS NULL

        [Required]
        public double Fare { get; set; } // NOT NULL

        [NotMapped]
        public double? TotalEarnings { get; set; }

        /* NAVIGATIONAL PROPERTIES */
        // Navigational property (Deliverer has 1 or more Availability record(s))
        [JsonIgnore]
        public virtual ICollection<Availability> Availabilities { get; set; }

        // Navigational property (Deliverer has 0 or more Delivery records)
        [JsonIgnore]
        public virtual ICollection<Delivery> Deliveries { get; set; }

        // Navigational property (Deliverer has 0 or more Feedback records)
        [JsonIgnore]
        public virtual ICollection<Feedback> Feedback { get; set; }

        // Navigational property (Deliverer has 0 or more Notification records)
        [JsonIgnore]
        public virtual ICollection<Notification> Notifications { get; set; }

        // Navigational properties (Deliverer has 0 or more Location records)
        [JsonIgnore]
        public virtual ICollection<Location> Warehouses { get; set; }

        // Navigational property (Deliverer has 1 Home record)
        public virtual Location Home { get; set; }

        /// <summary>
        /// Constructor for the entire Deliverer model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="emailAddress"></param>
        /// <param name="hash"></param>
        /// <param name="token"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="homeId"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="range"></param>
        /// <param name="vehicle"></param>
        /// <param name="fare"></param>
        /// <param name="availabilities"></param>
        /// <param name="deliveries"></param>
        /// <param name="feedback"></param>
        /// <param name="notifications"></param>
        /// <param name="warehouses"></param>
        /// <param name="home"></param>
        [JsonConstructor]
        public Deliverer(Guid id, string emailAddress, string hash, string token, string phoneNumber, Guid homeId, DateTime dateOfBirth, int range, Vehicles vehicle, double fare, ICollection<Availability> availabilities, ICollection<Delivery> deliveries, ICollection<Feedback> feedback, ICollection<Notification> notifications, ICollection<Location> warehouses, Location home)
            : base(id, emailAddress, hash, token)
        {
            this.PhoneNumber = phoneNumber;
            this.HomeId = homeId;
            this.DateOfBirth = dateOfBirth;
            this.Range = range;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.Fare = fare;
            this.Availabilities = availabilities;
            this.Deliveries = deliveries;
            this.Feedback = feedback;
            this.Notifications = notifications;
            this.Warehouses = warehouses;
            this.Home = home;

            if (deliveries != null)
                this.TotalEarnings = CalculateTotalEarnings(deliveries);
        }

        /// <summary>
        /// Public constructor without navigational properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="homeId"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="range"></param>
        /// <param name="vehicle"></param>
        /// <param name="fare"></param>
        public Deliverer(Guid id, string emailAddress, string hash, string token, string phoneNumber, Guid homeId, DateTime dateOfBirth, int range, Vehicles vehicle, double fare)
            : base(id, emailAddress, hash, token)
        {
            this.PhoneNumber = phoneNumber;
            this.HomeId = homeId;
            this.DateOfBirth = dateOfBirth;
            this.Range = range;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.Fare = fare;
        }

        /// <summary>
        /// Register a Deliverer,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="fare"></param>
        /// <param name="home"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="range"></param>
        /// <param name="vehicle"></param>
        public Deliverer(
            string emailAddress,
            string password,
            DateTime dateOfBirth, 
            double fare,
            Location home,
            string phoneNumber, 
            int range, 
            Vehicles vehicle
        ) : base(emailAddress, password)
        {
            this.DateOfBirth = dateOfBirth;
            this.Fare = fare;
            this.Home = home;
            this.PhoneNumber = phoneNumber;
            this.Range = range;
            this.Vehicle = vehicle;

            this.HomeId = home.Id;
        }
        
        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (this.Id != null && this.Id != Guid.Empty &&
                this.HomeId != null && this.HomeId != Guid.Empty &&
                this.DateOfBirth != null && this.Range > 0 &&
                this.Vehicle != 0 && this.Fare >= 0 && !string.IsNullOrEmpty(this.PhoneNumber))
                return true;
            return false;
        }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method that calculates the total earnings (price + tip) of a Deliverer
        /// </summary>
        /// <param name="deliveries"></param>
        /// <returns>Total earnings in double format</returns>
        public double CalculateTotalEarnings(ICollection<Delivery> deliveries)
        {
            double e = 0;
            foreach (Delivery d in deliveries)
            {
                e += d.Price;
                if (d.Tip.HasValue)
                    e += d.Tip.Value;
            }
            return e;
        }

        [NotMapped]
        [JsonIgnore]
        public TimeSpan AverageNotificationReactionTime { get; set; }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method for calculating the average reaction time of a deliverer for a notification
        /// </summary>
        public void SetAverageNotificationReactionTime()
        {
            if (this.Notifications != null && this.Notifications.Count > 0)
            {
                List<TimeSpan> nrts = new List<TimeSpan>();
                foreach (Notification n in this.Notifications)
                {
                    // Run through all scenarios
                    if (n.AcceptedAt.HasValue) nrts.Add(n.AcceptedAt.Value.TimeOfDay.Subtract(n.CreatedAt.TimeOfDay));
                    else if (n.RefusedAt.HasValue) nrts.Add(n.RefusedAt.Value.TimeOfDay.Subtract(n.CreatedAt.TimeOfDay));
                    else nrts.Add(n.ExpiredAt.TimeOfDay.Subtract(n.CreatedAt.TimeOfDay));
                }
                this.AverageNotificationReactionTime = new TimeSpan(Convert.ToInt64(nrts.Average(t => t.Ticks))); 
            }
            else if (this.Notifications != null && this.Notifications.Count == 0)
                this.AverageNotificationReactionTime = TimeSpan.MinValue;
            else
                this.AverageNotificationReactionTime = TimeSpan.MaxValue;
        }


        [NotMapped]
        [JsonIgnore]
        public double NotificationAcceptRatio { get; set; }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method for calculating the acceptratio of a deliverer for his notifications.
        /// </summary>
        public void SetNotificationAcceptRatio()
        {
            if (this.Notifications != null && this.Notifications.Count > 0 && this.Notifications.Where(x => x.AcceptedAt.HasValue).Count() > 0)
                this.NotificationAcceptRatio = this.Notifications.Count() / this.Notifications.Where(x => x.AcceptedAt.HasValue).Count();
            else
                this.NotificationAcceptRatio = 0;
        }

        [NotMapped]
        [JsonIgnore]
        public TimeSpan AverageDeliveryTime { get; set; }

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method for calculating the average deliverytime for a deliverer
        /// </summary>
        public void SetAverageDeliveryTime()
        {
            if (this.Deliveries != null && this.Deliveries.Count > 0)
            {
                List<TimeSpan> dts = new List<TimeSpan>();
                foreach (Delivery d in this.Deliveries)
                {
                    if (d.DeliveredAt.HasValue && d.WarehousePickUpAt.HasValue)
                        dts.Add(d.DeliveredAt.Value.TimeOfDay - d.WarehousePickUpAt.Value.TimeOfDay);
                }
                this.AverageDeliveryTime = new TimeSpan(Convert.ToInt64(dts.Average(t => t.Ticks)));
            }
            else if (this.Deliveries != null && this.Deliveries.Count == 0)
                this.AverageDeliveryTime = TimeSpan.MinValue;
            else
            this.AverageDeliveryTime = TimeSpan.MaxValue;            
        }
    }
}