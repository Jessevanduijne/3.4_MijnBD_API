using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class Notification
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        [Required]
        public Guid DelivererId { get; set; } // NOT NULL

        [Required]
        public Guid DeliveryId { get; set; } // NOT NULL

        [Required]
        public DateTime CreatedAt { get; set; } // NOT NULL

        public DateTime? AcceptedAt { get; set; } // IS NULL

        public DateTime? RefusedAt { get; set; } // IS NULL

        [Required]
        public DateTime ExpiredAt { get; set; } // NOT NULL

        [Required]
        public NotificationStatus Status { get; set; } // NOT NULL

        [NotMapped] // Not in database
        public string StatusDisplayName { get; set; } // IS NULL

        // Navigational property (Notification has 1 Delivery record)
        [JsonIgnore]
        public virtual Delivery Delivery { get; set; }

        // Navigational property (Notification has 1 Deliverer record)
        [JsonIgnore]
        public virtual Deliverer Deliverer { get; set; }

        /// <summary>
        /// Public constructor for the entire Notification model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="deliveryId"></param>
        /// <param name="createdAt"></param>
        /// <param name="acceptedAt"></param>
        /// <param name="refusedAt"></param>
        /// <param name="expiredAt"></param>
        /// <param name="delivery"></param>
        /// <param name="deliverer"></param>
        /// <param name="status"></param>
        public Notification(Guid id, Guid delivererId, Guid deliveryId, DateTime createdAt, DateTime? acceptedAt, DateTime? refusedAt, DateTime expiredAt, NotificationStatus status, Delivery delivery, Deliverer deliverer)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.DeliveryId = deliveryId;
            this.CreatedAt = createdAt;
            this.AcceptedAt = acceptedAt;
            this.RefusedAt = refusedAt;
            this.ExpiredAt = expiredAt;
            this.Status = status;
            this.StatusDisplayName = Instance().GetDisplayName(status.GetType(), status.ToString());
            this.Delivery = delivery;
            this.Deliverer = deliverer;
        }

        /// <summary>
        /// Public constructor without navigational properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="deliveryId"></param>
        /// <param name="createdAt"></param>
        /// <param name="acceptedAt"></param>
        /// <param name="refusedAt"></param>
        /// <param name="expiredAt"></param>
        /// <param name="status"></param>
        public Notification(Guid id, Guid delivererId, Guid deliveryId, DateTime createdAt, DateTime? acceptedAt, DateTime? refusedAt, DateTime expiredAt, NotificationStatus status)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.DeliveryId = deliveryId;
            this.CreatedAt = createdAt;
            this.AcceptedAt = acceptedAt;
            this.RefusedAt = refusedAt;
            this.ExpiredAt = expiredAt;
            this.Status = status;
            this.StatusDisplayName = Instance().GetDisplayName(status.GetType(), status.ToString());
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
                this.CreatedAt != null && this.RefusedAt != null && this.Status >= 0)
                return true;
            return false;
        }
    }
}
