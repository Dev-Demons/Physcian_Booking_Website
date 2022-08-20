namespace Doctyme.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ErrorLog
    {
        [Key]
        public long LogId { get; set; }

        [StringLength(50)]
        public string Source { get; set; }

        [StringLength(50)]
        public string TargetSite { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public string Message { get; set; }

        public string Stack { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string InnerStackTrace { get; set; }

        public DateTime LogDate { get; set; }

        public string AppType { get; set; }
    }
}
