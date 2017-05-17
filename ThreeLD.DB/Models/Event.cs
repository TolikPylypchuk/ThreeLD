using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ThreeLD.DB.Properties;

namespace ThreeLD.DB.Models
{
    [Table(nameof(AppDbContext.Events))]
    public class Event : EntityBase
    {
		[Display(
			Name = "EventNameDisplayName",
			ResourceType = typeof(Resources))]
        [Required(
			ErrorMessageResourceName = "EventNameRequired",
			ErrorMessageResourceType = typeof(Resources))]
		public string Name { get; set; }

		[Display(
			Name = "EventDateTimeDisplayName",
			ResourceType = typeof(Resources))]
		[DisplayFormat(
			DataFormatString = "{0:yyyy-MM-ddTHH\\:mm\\:ss}",
			ApplyFormatInEditMode = true)]
		[Required(
			ErrorMessageResourceName = "EventDateTimeRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public DateTime DateTime { get; set; }

		[Display(
			Name = "EventDurationDisplayName",
			ResourceType = typeof(Resources))]
		[DisplayFormat(
			DataFormatString = "{0:hh\\:mm}",
			ApplyFormatInEditMode = true)]
		[Required(
			ErrorMessageResourceName = "EventDurationRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public TimeSpan Duration { get; set; }

		[Display(
			Name = "EventAddressDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "EventAddressRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public string Address { get; set; }

        [Url]
		[Display(
			Name = "EventUrlDisplayName",
			ResourceType = typeof(Resources))]
		public string Url { get; set; }

		[Display(
			Name = "EventDescriptionDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "EventDescriptionRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public string Description { get; set; }

		[Display(
			Name = "EventCategoryDisplayName",
			ResourceType = typeof(Resources))]
		[Required(
			ErrorMessageResourceName = "EventCategoryRequired",
			ErrorMessageResourceType = typeof(Resources))]
        public string Category { get; set; }

        [DefaultValue(true)]
        public bool IsApproved { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string ProposedBy { get; set; }

		public virtual ICollection<User> BookmarkedBy { get; set; } =
			new HashSet<User>();
    }
}
