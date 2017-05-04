using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreeLD.DB.Models
{
    [Table(nameof(AppDbContext.Notifications))]
    public class Notification : EntityBase
    {
        [Required(ErrorMessage = "The message of the notication is required.")]
        public string Message { get; set; }

        [DefaultValue(false)]
        public bool IsRead { get; set; }

        [Required(ErrorMessage = "Specify the editor.")]
        public string From { get; set; }

        [Required(ErrorMessage = "Specify the user.")]
        public string To { get; set; }

        [ForeignKey(nameof(From))]
        public virtual User Editor { get; set; }

        [ForeignKey(nameof(To))]
        public virtual User User { get; set; }
    }
}
