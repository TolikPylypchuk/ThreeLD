using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ThreeLD.DB.Properties;

namespace ThreeLD.DB.Models
{
    [Table(nameof(AppDbContext.Notifications))]
    public class Notification : EntityBase
    {
		[Display(
			Name = "NotificationMessageDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "NotificationMessageRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public string Message { get; set; }

        [DefaultValue(false)]
        public bool IsRead { get; set; }
		
        public string From { get; set; }
        public string To { get; set; }

        [ForeignKey(nameof(From))]
        public virtual User Editor { get; set; }

        [ForeignKey(nameof(To))]
        public virtual User User { get; set; }
    }
}
