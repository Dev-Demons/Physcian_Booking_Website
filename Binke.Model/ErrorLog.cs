using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Binke.Model
{
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(50)]
        public string Source { get; set; }
        [Column(TypeName = "nvarchar"), MaxLength(50)]
        public string TargetSite { get; set; }
        [Column(TypeName = "nvarchar"), MaxLength(50)]
        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string InnerStackTrace { get; set; }
        public DateTime LogDate { get; set; }
        public string AppType { get; set; }
    }

}
