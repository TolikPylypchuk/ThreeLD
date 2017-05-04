using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class NotificationsViewModel
    {
        public List<Notification> UnreadNotifications { get; set; }
        public List<Notification> ReadNotifications { get; set; }
    }
}
