using BezorgDirect.BezorgersApplicatie.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static BezorgDirect.BezorgersApplicatie.BusinessLogic.Models.Enums;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces
{
    /* This interface was setup and written by Lennart de Waart (563079) */
    public interface INotificationsRepository // Contract for NotificationsRepository, repository may be changed
    {
        bool UpdateExpiredNotifications(List<Notification> expiredNotifications);
        Task<List<Notification>> GetAllExpiredNotifications();
        Task<bool> AddNotification(Guid id, Guid delivererId, Guid deliveryId, DateTime createdAt, DateTime expiredAt, NotificationStatus status);
        Task<Notification> GetFirstOpenNotificationByDelivererId(Guid delivererId);
        Task<Notification> UpdateNotificationStatusById(Guid id, bool accepted);
        Task<Notification> GetNotificationById(Guid id);
    }
}
