using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class Location
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        [Required]
        public double Latitude { get; set; } // NOT NULL

        [Required]
        public double Longitude { get; set; } // NOT NULL

        public string Address { get; set; } // IS NULL

        [StringLength(6, ErrorMessage = "Postcode moet 6 tekens lang zijn en mag geen spaties bevatten")]
        public string PostalCode { get; set; } // IS NULL

        public string Place { get; set; } // IS NULL

        [Required]
        public bool IsWarehouse { get; set; } // NOT NULL

        /// <summary>
        /// Public constructor for the entire Location model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="address"></param>
        /// <param name="postalCode"></param>
        /// <param name="place"></param>
        /// <param name="isWarehouse"></param>
        public Location(Guid id, double latitude, double longitude, string address, string postalCode, string place, bool isWarehouse)
        {
            this.Id = id;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Address = address;
            this.PostalCode = postalCode;
            this.Place = place;
            this.IsWarehouse = isWarehouse;
        }


        /// <summary>
        /// Construct a Location from AddressData and LatLng point
        /// </summary>
        /// <param name="address"></param>
        /// <param name="point"></param>
        /// <param name="isWarehouse"></param>
        public Location(AddressData address, LatLng point, bool isWarehouse = false)
        {
            this.Id = Guid.NewGuid();
            this.Latitude = point.Latitude;
            this.Longitude = point.Longitude;
            this.Address = address.Address;
            this.PostalCode = address.PostalCode;
            this.Place = address.Place;
            this.IsWarehouse = isWarehouse;
        }

        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (this.Id != null && this.Id != Guid.Empty &&
                this.Latitude > 0 && this.Longitude > 0 &&
                (this.IsWarehouse == true || this.IsWarehouse == false))
                return true;
            return false;
        }
    }

    [NotMapped]
    public class LatLng
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// Public constructor for the entire LatLng class
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public LatLng(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (this.Latitude == 0 || this.Longitude == 0) return false;
            else return true;
        }
    }

    [NotMapped]
    public class AddressData
    {
        public string HouseNumber { get; set; }
        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string Place { get; set; } 

        /// <summary>
        /// Public constructor for the entire AddressData class
        /// </summary>
        /// <param name="address"></param>
        /// <param name="postalCode"></param>
        /// <param name="place"></param>
        public AddressData(string address, string postalCode, string place)
        {
            this.Address = address;
            this.PostalCode = postalCode;
            this.Place = place;
        }

        /// <summary>
        /// Public empty constructor
        /// </summary>
        public AddressData()
        { }

        /// <summary>
        /// Gets an address as a string
        /// </summary>
        /// <returns>Address string</returns>
        public string AsString()
        {
            return $"{Address}, {PostalCode} {Place}";
        }

        /// <summary>
        /// Public method that returns a boolean value which states whether the model is valid/fully filled.
        /// </summary>
        /// <returns>Returns whether the model is valid/fully filled</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Address) || string.IsNullOrEmpty(this.PostalCode) || string.IsNullOrEmpty(this.Place)) return false;
            else return true;
        }
    }
}