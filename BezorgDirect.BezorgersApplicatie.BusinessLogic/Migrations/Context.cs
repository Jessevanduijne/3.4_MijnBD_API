using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Deliverer> Deliverers { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set token to is unique
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Token)
                .IsUnique();

            // Seed database with dummy data
            Guid homeLocationId = Guid.NewGuid();
            Guid customerLocationId = Guid.NewGuid();
            Guid warehouseLocationId = Guid.NewGuid();
            Guid delivererId1 = Guid.NewGuid();
            Guid delivererId2 = Guid.NewGuid();
            Guid delivererId3 = Guid.NewGuid();
            Guid delivererId4 = Guid.NewGuid();
            Guid delivererId5 = Guid.NewGuid();
            Guid delivererId6 = Guid.NewGuid();
            Guid delivererId7 = Guid.NewGuid();
            Guid delivererId8 = Guid.NewGuid();
            Guid delivererId9 = Guid.NewGuid();
            Guid delivererId10 = Guid.NewGuid();
            Guid delivererId11 = Guid.NewGuid();
            Guid delivererId12 = Guid.NewGuid();
            Guid deliveryId1 = Guid.NewGuid();
            Guid deliveryId2 = Guid.NewGuid();
            Guid deliveryId3 = Guid.NewGuid();
            Guid deliveryId4 = Guid.NewGuid();
            Guid deliveryId5 = Guid.NewGuid();
            Guid deliveryId6 = Guid.NewGuid();
            Guid deliveryId7 = Guid.NewGuid();
            Guid deliveryId8 = Guid.NewGuid();
            Guid deliveryId9 = Guid.NewGuid();
            Guid deliveryId10 = Guid.NewGuid();
            Guid deliveryId11 = Guid.NewGuid();
            Guid deliveryId12 = Guid.NewGuid();
            Guid deliveryId13 = Guid.NewGuid();
            Guid deliveryId14 = Guid.NewGuid();
            Guid deliveryId15 = Guid.NewGuid();
            Guid deliveryId16 = Guid.NewGuid();
            Guid deliveryId17 = Guid.NewGuid();
            Guid deliveryId18 = Guid.NewGuid();
            Guid reviewId = Guid.NewGuid();

            modelBuilder.Entity<Location>().HasData(
                new Location(homeLocationId, 52.322404, 4.596926, "Lage Duin 38-14", "2121CH", "Bennebroek", false),
                new Location(customerLocationId, 52.389454, 4.612877, "Bijdorplaan 15", "2015CE", "Haarlem", false),
                new Location(warehouseLocationId, 52.425825, 4.646711, "Vlietweg 16", "2071KW", "Santpoort-Noord", true)
            );

            modelBuilder.Entity<Deliverer>().HasData(
                // Out of range
                new Deliverer(delivererId1, "test1@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 5, Enums.Vehicles.Auto, 5.41),
                // Currently busy
                new Deliverer(delivererId2, "test2@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Not available
                new Deliverer(delivererId3, "test3@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Has no deliveries
                new Deliverer(delivererId4, "test4@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Has an outstanding notification
                new Deliverer(delivererId5, "test5@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Doesn't have the suited vehicle
                new Deliverer(delivererId6, "test6@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Scooter, 5.41),
                // Best delivery time
                new Deliverer(delivererId7, "test7@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Second best delivery time
                new Deliverer(delivererId8, "test8@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Best reaction time
                new Deliverer(delivererId9, "test9@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Second best reaction time
                new Deliverer(delivererId10, "test10@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Best acceptratio
                new Deliverer(delivererId11, "test11@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41),
                // Second best acceptratio
                new Deliverer(delivererId12, "test12@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, "0612345678", homeLocationId, new DateTime(1997, 12, 12), 40, Enums.Vehicles.Auto, 5.41)
            );

            modelBuilder.Entity<Availability>().HasData(
                new Availability(Guid.NewGuid(), delivererId1, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId2, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),                
                new Availability(Guid.NewGuid(), delivererId4, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId5, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId6, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId7, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId8, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId9, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId10, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId11, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay),
                new Availability(Guid.NewGuid(), delivererId12, DateTime.Now.Date, DateTime.Now.TimeOfDay, DateTime.Now.AddMinutes(155).TimeOfDay)
            );

            //modelBuilder.Entity<Delivery>().HasData(
            //    new Delivery(deliveryId1, delivererId2, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-21), warehouseLocationId, DateTime.Now, 5.7, DateTime.Now.AddMinutes(9), customerLocationId, warehouseLocationId, null, 14.26, null, Enums.PaymentMethods.None, Enums.DeliveryStatus.Onderweg),
            //    new Delivery(deliveryId2, null, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, null, null, null, warehouseLocationId, null, 5.7, DateTime.Now.AddMinutes(9), customerLocationId, null, null, 53.68, null, Enums.PaymentMethods.None, Enums.DeliveryStatus.Aangekondigd),
            //    new Delivery(deliveryId3, delivererId7, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(-1), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId4, delivererId7, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now, 14.26, 2.5, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId5, delivererId8, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(1), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId6, delivererId8, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(1), 14.26, 1, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId7, delivererId9, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(58), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId8, delivererId9, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(46), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId9, delivererId10, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(16), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId10, delivererId10, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(28), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId11, delivererId11, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(42), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId12, delivererId11, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(33), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId13, delivererId12, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(4), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId14, delivererId12, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(8), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    // add deliveries to out of scope deliverers                
            //    new Delivery(deliveryId15, delivererId6, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(8), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId16, delivererId5, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(8), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId17, delivererId3, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(8), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd),
            //    new Delivery(deliveryId18, delivererId1, "0612345678", DateTime.Now.AddHours(2), Enums.Vehicles.Auto, homeLocationId, 13.6, DateTime.Now.AddMinutes(-30), warehouseLocationId, DateTime.Now.AddMinutes(-9), 5.7, DateTime.Now, customerLocationId, customerLocationId, DateTime.Now.AddMinutes(8), 14.26, null, Enums.PaymentMethods.PIN, Enums.DeliveryStatus.Afgeleverd)            
            //);

            modelBuilder.Entity<Feedback>().HasData(
                new Feedback(Guid.NewGuid(), delivererId1, deliveryId18, Enums.FeedbackCategories.Bezorger, 5),
                new Feedback(Guid.NewGuid(), delivererId1, deliveryId18, Enums.FeedbackCategories.Bezorgtijd, 5),
                new Feedback(Guid.NewGuid(), delivererId1, deliveryId18, Enums.FeedbackCategories.Productkwaliteit, 3)
            );

            modelBuilder.Entity<Notification>().HasData(
                new Notification(Guid.NewGuid(), delivererId5, deliveryId2, DateTime.Now, null, null, DateTime.Now.AddDays(365), Enums.NotificationStatus.Verstuurd),
                new Notification(Guid.NewGuid(), delivererId7, deliveryId3, DateTime.Now, DateTime.Now.AddMinutes(8), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId7, deliveryId4, DateTime.Now, DateTime.Now.AddMinutes(7), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId8, deliveryId5, DateTime.Now, DateTime.Now.AddMinutes(8), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId8, deliveryId6, DateTime.Now, DateTime.Now.AddMinutes(7), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId9, deliveryId7, DateTime.Now, DateTime.Now.AddMinutes(1), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId9, deliveryId8, DateTime.Now, DateTime.Now.AddMinutes(2), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId9, deliveryId9, DateTime.Now, null, DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geweigerd),
                new Notification(Guid.NewGuid(), delivererId9, deliveryId10, DateTime.Now, null, DateTime.Now.AddMinutes(2), DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geweigerd),
                new Notification(Guid.NewGuid(), delivererId10, deliveryId10, DateTime.Now, DateTime.Now.AddMinutes(2), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId10, deliveryId11, DateTime.Now, DateTime.Now.AddMinutes(2), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId10, deliveryId12, DateTime.Now, null, DateTime.Now.AddMinutes(3), DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geweigerd),
                new Notification(Guid.NewGuid(), delivererId11, deliveryId11, DateTime.Now, DateTime.Now.AddMinutes(8), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId11, deliveryId12, DateTime.Now, DateTime.Now.AddMinutes(8), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId12, deliveryId13, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId12, deliveryId14, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),

                new Notification(Guid.NewGuid(), delivererId2, deliveryId1, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId1, deliveryId18, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId3, deliveryId17, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd),
                new Notification(Guid.NewGuid(), delivererId6, deliveryId15, DateTime.Now, DateTime.Now.AddMinutes(9), null, DateTime.Now.AddMinutes(10), Enums.NotificationStatus.Geaccepteerd)
            );

            // Build database
            base.OnModelCreating(modelBuilder);
        }
    }
}