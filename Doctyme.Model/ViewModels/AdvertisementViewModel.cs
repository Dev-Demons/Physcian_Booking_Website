using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctyme.Model.ViewModels
{
    class AdvertisementViewModel
    {
        public int AdvertisementId { get; set; }
        public int AdvertisementLocationId { get; set; }
        public int? UserTypeId { get; set; }
        public int? ReferenceId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? TotalImpressions { get; set; }
        public int? PaymentTypeId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class DrpAdvertisementLocationModel
    {
        public int AdvertisementLocationID { get; set; }
        public string AdvertisementLocationName { get; set; }
    }

    public class Advertisements //Added by Reena
    {
        public int AdvertisementId { get; set; }
        public int ReferenceId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentTypeDescription { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public double DistanceCount { get; set; }
    }
}
