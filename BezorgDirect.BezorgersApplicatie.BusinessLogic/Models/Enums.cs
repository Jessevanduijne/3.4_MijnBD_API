using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup and written by Lennart de Waart (563079) */
    [NotMapped] // Do not map this model to the database
    public sealed class Enums
    {
        // Singleton pattern
        private static Enums uniqueInstance;

        /// <summary>
        /// Private constructor, unavailable outside this class
        /// </summary>
        private Enums() { }

        /// <summary>
        /// This static method creates a unique instance of the Enums class if it did not already exist.
        /// Finally it returns this instance to the function caller.
        /// </summary>
        /// <returns>unique instance of Enums class</returns>
        public static Enums Instance()
        {
            if (uniqueInstance == null)
                uniqueInstance = new Enums();
            return uniqueInstance;
        }

        /// <summary>
        /// Public method that expects an Enum type and a value and retrieves the displayname of that particular item.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns>The displayname of the given enum value</returns>
        public string GetDisplayName(Type enumType, string value)
        {
            var memberInfo = enumType.GetMember(value);
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            return ((DisplayAttribute)attributes[0]).Name;
        }

        public enum Vehicles
        {
            [Display(Name = "Fiets")]
            Fiets = 1,

            [Display(Name = "Scooter")]
            Scooter = 2,

            [Display(Name = "Motor")]
            Motor = 3,

            [Display(Name = "Auto")]
            Auto = 4
        }

        public enum DeliveryStatus
        {
            [Display(Name = "Afgemeld")]
            Afgemeld = 0,

            [Display(Name = "Aangekondigd")]
            Aangekondigd = 1,

            [Display(Name = "Bevestigd")]
            Bevestigd = 2,

            [Display(Name = "Onderweg")]
            Onderweg = 3,

            [Display(Name = "Afgeleverd")]
            Afgeleverd = 4
        }

        public enum NotificationStatus
        {
            [Display(Name = "Verstuurd")]
            Verstuurd = 0,

            [Display(Name = "Geaccepteerd")]
            Geaccepteerd = 1,

            [Display(Name = "Geweigerd")]
            Geweigerd = 2,

            [Display(Name = "Verlopen")]
            Verlopen = 3
        }

        public enum FeedbackCategories
        {
            [Display(Name = "Bezorger")]
            Bezorger = 0,

            [Display(Name = "Productkwaliteit")]
            Productkwaliteit = 1,

            [Display(Name = "Bezorgtijd")]
            Bezorgtijd = 2
        }

        public enum PaymentMethods
        {
            [Display(Name = "(nog) niet betaald")]
            None = 0,

            [Display(Name = "PIN")]
            PIN = 1
        }
    }
}