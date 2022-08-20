namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NewsletterSubscriber")]
    public partial class NewsletterSubscriber
    {
        public int NewsletterSubscriberId { get; set; }
        [StringLength(100)]
        public string EmailID { get; set; }
        public DateTime SubscribeDate { get; set; }
        public DateTime? UnsubscribedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
