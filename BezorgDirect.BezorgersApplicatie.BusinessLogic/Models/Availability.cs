using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup by Lennart de Waart (563079) */
    public class Availability
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        [Required]
        public Guid DelivererId { get; set; } // NOT NULL

        [Required]
        public DateTime Date { get; set; } // NOT NULL

        [Required]
        public TimeSpan StartTime { get; set; } // NOT NULL

        [Required]
        public TimeSpan EndTime { get; set; } // NOT NULL

        // Navigational property (Availability belongs to 1 Deliverer)
        [JsonIgnore]
        public virtual Deliverer Deliverer { get; set; }

        /// <summary>
        /// Public constructor for the entire Availability model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="date"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="Deliverer"></param>
        public Availability(Guid id, Guid delivererId, DateTime date, TimeSpan startTime, TimeSpan endTime, Deliverer Deliverer)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.Date = date;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Deliverer = Deliverer;
        }

        /// <summary>
        /// Public constructor without navigational properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="date"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        [JsonConstructor] // This constructor was setup by Mats Webbers 484491
        public Availability(Guid id, Guid delivererId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.Date = date;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }        

        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (this.Id != null && this.Id != Guid.Empty &&
                this.DelivererId != null && this.DelivererId != Guid.Empty &&
                this.Date != null && this.StartTime != null &&
                this.StartTime != TimeSpan.Zero && this.EndTime != null &&
                this.EndTime != TimeSpan.Zero)
                return true;
            return false;
        }
    }
}