using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup by Lennart de Waart (563079) */
    public class Feedback
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        [Required]
        public Guid DelivererId { get; set; } // NOT NULL

        [Required]
        public Guid DeliveryId { get; set; } // NOT NULL              

        [Required]
        public FeedbackCategories Category { get; set; } // NOT NULL

        [NotMapped] // Not in database
        public string CategoryDisplayName { get; set; } // IS NULL

        [Required]
        public int Rating { get; set; } // NOT NULL

        // Navigational property (Feedback belongs to 1 Delivery record)
        [JsonIgnore]
        public virtual Delivery Delivery { get; set; }

        // Navigational property (Feedback belongs to 1 Deliverer record)
        [JsonIgnore]
        public virtual Deliverer Deliverer { get; set; }

        /// <summary>
        /// Public constructor for the entire Feedback model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="deliveryId"></param>
        /// <param name="category"></param>
        /// <param name="rating"></param>
        /// <param name="deliverer"></param>
        /// <param name="delivery"></param>
        [JsonConstructor]
        public Feedback(Guid id, Guid delivererId, Guid deliveryId, FeedbackCategories category, int rating, Deliverer deliverer, Delivery delivery)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.DeliveryId = deliveryId;
            this.Delivery = delivery;
            this.Deliverer = deliverer;
            this.Category = category;
            this.Rating = rating;
            this.CategoryDisplayName = Instance().GetDisplayName(category.GetType(), category.ToString());
        }

        /// <summary>
        /// Public constructor for the entire Feedback model without navigational properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="deliveryId"></param>
        /// <param name="category"></param>
        /// <param name="rating"></param>
        public Feedback(Guid id, Guid delivererId, Guid deliveryId, FeedbackCategories category, int rating)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.DeliveryId = deliveryId;
            this.Category = category;
            this.Rating = rating;
            this.CategoryDisplayName = Instance().GetDisplayName(category.GetType(), category.ToString());
        }

        public Feedback(Guid delivererId, Guid deliveryId, FeedbackCategories category, int rating)
        {
            this.DelivererId = delivererId;
            this.DeliveryId = deliveryId;
            this.Category = category;
            this.Rating = rating;
            this.CategoryDisplayName = Instance().GetDisplayName(category.GetType(), category.ToString());
        }

        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (this.Id != null && this.Id != Guid.Empty &&
                this.DelivererId != null && this.DelivererId != Guid.Empty &&
                this.DeliveryId != null && this.DeliveryId != Guid.Empty &&
                this.Category > 0 && this.Rating > 0)
                return true;
            return false;
        }
    }
}