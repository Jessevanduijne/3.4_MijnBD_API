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
    public class Delivery
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        public Guid? DelivererId { get; set; } // IS NULL        

        [Required, StringLength(10, ErrorMessage = "Telefoonnummer moet 10 nummers lang zijn en mag geen spaties bevatten")]
        public string CustomerPhoneNumber { get; set; } // IS NULL 

        [Required]
        public DateTime DueDate { get; set; } // NOT NULL

        [Required]
        public Vehicles Vehicle { get; set; } // NOT NULL

        [NotMapped] // Not in database
        public string VehicleDisplayName { get; set; }

        public Guid? StartedAtId { get; set; } // IS NULL
        
        public double? WarehouseDistanceInKilometers { get; set; } // IS NULL, distance measured from startedAt
        
        public DateTime? WarehouseETA { get; set; } // IS NULL, time measured from startedAt

        [Required]
        public Guid WarehouseId { get; set; } // NOT NULL

        public DateTime? WarehousePickUpAt { get; set; } // IS NULL

        public double? CustomerDistanceInKilometers { get; set; } // IS NULL, distance measured from warehouse

        public DateTime? CustomerETA { get; set; } // IS NULL, time measured from warehouse

        [Required]
        public Guid CustomerId { get; set; } // NOT NULL

        public Guid? CurrentId { get; set; } // IS NULL  

        public DateTime? DeliveredAt { get; set; } // IS NULL

        [Required]
        public double Price { get; set; } // NOT NULL

        public double? Tip { get; set; } // IS NULL

        [Required]
        public PaymentMethods PaymentMethod { get; set; } // NOT NULL

        [NotMapped] // Not in database
        public string PaymentMethodDisplayName { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; }

        [NotMapped] // Not in database
        public string StatusDisplayName { get; set; }

        /* NAVIGATIONAL PROPERTIES */
        // Navigational property (Delivery has 1 Deliverer record)
        [JsonIgnore]
        public virtual Deliverer Deliverer { get; set; }

        // Navigational property (Delivery has 1 Location record for Warehouse)
        public virtual Location Warehouse { get; set; }

        // Navigational property (Delivery has 1 Location record for Customer)
        public virtual Location Customer { get; set; }

        // Navigational property (Delivery has 1 Location record for Current)
        public virtual Location Current { get; set; }

        // Navigational property (Review has 0 or more Feedback record(s))
        [JsonIgnore]
        public virtual ICollection<Feedback> Feedback { get; set; }

        /// <summary>
        /// Public constructor for the entire Delivery model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="customerPhoneNumber"></param>
        /// <param name="dueDate"></param>
        /// <param name="vehicle"></param>
        /// <param name="startedAtId"></param>
        /// <param name="warehouseDistanceInKilometers">Distance measured from startedAt location</param>
        /// <param name="warehouseETA">Time measured from startedAt location</param>
        /// <param name="warehouseId"></param>
        /// <param name="warehousePickUpAt"></param>
        /// <param name="customerDistanceInKilometers">Distance measured from warehouse location</param>
        /// <param name="customerETA">Time measured from warehouse location</param>
        /// <param name="customerId"></param>
        /// <param name="currentId"></param>
        /// <param name="deliveredAt"></param>
        /// <param name="price"></param>
        /// <param name="tip"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="status"></param>
        /// <param name="deliverer"></param>
        /// <param name="warehouse"></param>
        /// <param name="customer"></param>
        /// <param name="current"></param>
        /// <param name="feedback"></param>
        [JsonConstructor]
        public Delivery(Guid id, Guid? delivererId, string customerPhoneNumber, DateTime dueDate, Vehicles vehicle, Guid? startedAtId, double? warehouseDistanceInKilometers,
            DateTime? warehouseETA, Guid warehouseId, DateTime? warehousePickUpAt, double? customerDistanceInKilometers, DateTime? customerETA,
            Guid customerId, Guid? currentId, DateTime? deliveredAt, double price, double? tip, PaymentMethods paymentMethod, DeliveryStatus status, 
            Deliverer deliverer, Location warehouse, Location customer, Location current, ICollection<Feedback> feedback)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.CustomerPhoneNumber = customerPhoneNumber;
            this.DueDate = dueDate;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.StartedAtId = startedAtId;
            this.WarehouseDistanceInKilometers = warehouseDistanceInKilometers;
            this.WarehouseETA = warehouseETA;
            this.WarehouseId = warehouseId;
            this.WarehousePickUpAt = warehousePickUpAt;
            this.CustomerDistanceInKilometers = customerDistanceInKilometers;
            this.CustomerETA = customerETA;
            this.CustomerId = customerId;
            this.CurrentId = currentId;
            this.DeliveredAt = deliveredAt;
            this.Price = price;
            this.Tip = tip;
            this.PaymentMethod = paymentMethod;
            this.PaymentMethodDisplayName = Instance().GetDisplayName(paymentMethod.GetType(), paymentMethod.ToString());
            this.Status = status;
            this.StatusDisplayName = Instance().GetDisplayName(status.GetType(), status.ToString());
            this.Deliverer = deliverer;
            this.Warehouse = warehouse;
            this.Customer = customer;
            this.Current = current;
            this.Feedback = feedback;
        }

        /// <summary>
        /// Public constructor without navigational properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivererId"></param>
        /// <param name="customerPhoneNumber"></param>
        /// <param name="dueDate"></param>
        /// <param name="vehicle"></param>
        /// <param name="startedAtId"></param>
        /// <param name="warehouseDistanceInKilometers">Distance measured from startedAt location</param>
        /// <param name="warehouseETA">Time measured from startedAt location</param>
        /// <param name="warehouseId"></param>
        /// <param name="warehousePickUpAt"></param>
        /// <param name="customerDistanceInKilometers">Distance measured from warehouse location</param>
        /// <param name="customerETA">Time measured from warehouse location</param>
        /// <param name="customerId"></param>
        /// <param name="currentId"></param>
        /// <param name="deliveredAt"></param>
        /// <param name="price"></param>
        /// <param name="tip"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="status"></param>
        public Delivery(Guid id, Guid? delivererId, string customerPhoneNumber, DateTime dueDate, Vehicles vehicle, Guid? startedAtId, double? warehouseDistanceInKilometers,
            DateTime? warehouseETA, Guid warehouseId, DateTime? warehousePickUpAt, double? customerDistanceInKilometers, DateTime? customerETA,
            Guid customerId, Guid? currentId, DateTime? deliveredAt, double price, double? tip, PaymentMethods paymentMethod, DeliveryStatus status)
        {
            this.Id = id;
            this.DelivererId = delivererId;
            this.CustomerPhoneNumber = customerPhoneNumber;
            this.DueDate = dueDate;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.StartedAtId = startedAtId;
            this.WarehouseDistanceInKilometers = warehouseDistanceInKilometers;
            this.WarehouseETA = warehouseETA;
            this.WarehouseId = warehouseId;
            this.WarehousePickUpAt = warehousePickUpAt;
            this.CustomerDistanceInKilometers = customerDistanceInKilometers;
            this.CustomerETA = customerETA;
            this.CustomerId = customerId;
            this.CurrentId = currentId;
            this.DeliveredAt = deliveredAt;
            this.Price = price;
            this.Tip = tip;
            this.PaymentMethod = paymentMethod;
            this.PaymentMethodDisplayName = Instance().GetDisplayName(paymentMethod.GetType(), paymentMethod.ToString());
            this.Status = status;
            this.StatusDisplayName = Instance().GetDisplayName(status.GetType(), status.ToString());
        }

        /// <summary>
        /// Public constructor without navigational properties and Id
        /// </summary>
        /// <param name="delivererId"></param>
        /// <param name="customerPhoneNumber"></param>
        /// <param name="dueDate"></param>
        /// <param name="vehicle"></param>
        /// <param name="startedAtId"></param>
        /// <param name="warehouseDistanceInKilometers"></param>
        /// <param name="warehouseETA"></param>
        /// <param name="warehouseId"></param>
        /// <param name="warehousePickUpAt"></param>
        /// <param name="customerDistanceInKilometers"></param>
        /// <param name="customerETA"></param>
        /// <param name="customerId"></param>
        /// <param name="currentId"></param>
        /// <param name="deliveredAt"></param>
        /// <param name="price"></param>
        /// <param name="tip"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="status"></param>
        public Delivery(Guid id, string customerPhoneNumber, DateTime dueDate, Vehicles vehicle, string vehicleDisplayName ,double price, PaymentMethods paymentMethod, DeliveryStatus status)
        {
            this.CustomerPhoneNumber = customerPhoneNumber;
            this.DueDate = dueDate;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = vehicleDisplayName;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.Price = price;
            this.PaymentMethod = paymentMethod;
            this.PaymentMethodDisplayName = Instance().GetDisplayName(paymentMethod.GetType(), paymentMethod.ToString());
            this.Status = status;
            this.StatusDisplayName = Instance().GetDisplayName(status.GetType(), status.ToString());
        }

        public Delivery(Guid? delivererId, string customerPhoneNumber, DateTime dueDate, Vehicles vehicle, Guid? startedAtId, double? warehouseDistanceInKilometers,
    DateTime? warehouseETA, Guid warehouseId, DateTime? warehousePickUpAt, double? customerDistanceInKilometers, DateTime? customerETA,
    Guid customerId, Guid? currentId, DateTime? deliveredAt, double price, double? tip, PaymentMethods paymentMethod, DeliveryStatus status)
        {
            this.DelivererId = delivererId;
            this.CustomerPhoneNumber = customerPhoneNumber;
            this.DueDate = dueDate;
            this.Vehicle = vehicle;
            this.VehicleDisplayName = Instance().GetDisplayName(vehicle.GetType(), vehicle.ToString());
            this.StartedAtId = startedAtId;
            this.WarehouseDistanceInKilometers = warehouseDistanceInKilometers;
            this.WarehouseETA = warehouseETA;
            this.WarehouseId = warehouseId;
            this.WarehousePickUpAt = warehousePickUpAt;
            this.CustomerDistanceInKilometers = customerDistanceInKilometers;
            this.CustomerETA = customerETA;
            this.CustomerId = customerId;
            this.CurrentId = currentId;
            this.DeliveredAt = deliveredAt;
            this.Price = price;
            this.Tip = tip;
            this.PaymentMethod = paymentMethod;
            this.PaymentMethodDisplayName = Instance().GetDisplayName(paymentMethod.GetType(), paymentMethod.ToString());
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
                this.WarehouseId != null && this.WarehouseId != Guid.Empty && !string.IsNullOrEmpty(this.StatusDisplayName) &&
                this.CustomerId != null && this.CustomerId != Guid.Empty &&
                this.DueDate != null && Status >= 0 && this.Vehicle >= 0 && 
                this.Price > 0 && !string.IsNullOrEmpty(this.CustomerPhoneNumber))
                return true;
            return false;
        }
    }
}