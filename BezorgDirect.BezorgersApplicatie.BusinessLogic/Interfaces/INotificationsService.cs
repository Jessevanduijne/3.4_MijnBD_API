using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Lennart de Waart (563079) */
    public interface INotificationsService // Contract for NotificationsService, service may be changed
    {
        Task<bool> CreateNotification(Guid deliveryId, Vehicles v, DateTime dueDate, LatLng warehouse, LatLng customer);
        Task<bool> AddNotification(Guid notificationId, Guid delivererId, Guid deliveryId, DateTime createdAt, DateTime expiredAt, NotificationStatus status);
        Notification GetNewNotification(Guid deliveryId, Vehicles v, DateTime dueDate, LatLng warehouse, LatLng customer);
        Task<Notification> GetNotificationById(Guid id);
        Task<Notification> GetFirstOpenNotificationByDelivererId(Guid delivererId);
        Task<bool> UpdateNotificationStatusById(Guid id, bool accepted);
        Task<bool> UpdateExpiredNotifications();
    }
}
