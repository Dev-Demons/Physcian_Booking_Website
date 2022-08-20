using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model
{
    [Table("CityStateZip")]
    public class CityStateZip
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityStateZipCodeID { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string ZipCode { get; set; }

        [Column(TypeName = "varchar"), MaxLength(100)]
        public string City { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string State { get; set; }
        [Column(TypeName = "varchar"), MaxLength(50)]
        public string LocationType { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public decimal? Xaxis { get; set; }
        public decimal? Yaxis { get; set; }
        public decimal? Zaxis { get; set; }
        [Column(TypeName = "varchar"), MaxLength(100)]
        public string WorldRegion { get; set; }
        [Column(TypeName = "varchar"), MaxLength(10)]
        public string Country { get; set; }
        [Column(TypeName = "varchar"), MaxLength(200)]
        public string LocationText { get; set; }
        [Column(TypeName = "varchar"), MaxLength(100)]
        public string Location { get; set; }
        public bool Decommissioned { get; set; }
        
    }

    public class CityStateInfoByZipCodeModel
    {
        public int CityStateZipCodeID { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }

    }

}
