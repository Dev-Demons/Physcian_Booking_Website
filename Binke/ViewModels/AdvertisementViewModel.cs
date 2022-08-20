using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Doctyme.Model;

namespace Binke.ViewModels
{
    public class AdvertisementViewModel
    {
        public int AdvertisementId { get; set; }
        public int? UserTypeId { get; set; }
        public string AdvertiserType { get; set; }
        public int? ReferenceId { get; set; }
        public string AdvertiserName { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveString { get; set; }
        public bool IsDeleted { get; set; }
        public int? TotalImpressions { get; set; }
        public int? PaymentTypeId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? AdvertisementTypeId { get; set; }
        public int? CityStateZipCodeId { get; set; }
    }

    public class AdvertisementListViewModel
    {
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string FullCityStateZipCode
        {
            get
            {
                if (CityStateZipCodeId.HasValue) return ZipCode + "(" + City + ", " + State + ")";
                else return "";
            }
        }
        public int AdvertisementId { get; set; }
        public int? AdvertisementTypeId { get; set; }
        public int? CityStateZipCodeId { get; set; }
        public string AdvertisementTypeName { get; set; }
        public int? UserTypeId { get; set; }
        public string AdvertiserType { get; set; }
        public int? ReferenceId { get; set; }
        public string AdvertiserName { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? TotalImpressions { get; set; }
        public int? PaymentTypeId { get; set; }
        public string UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public string PaymentTypeName { get; set; }
        public int? TotalRecordCount { get; set; }
        public decimal? Lat { get; set; }

        public decimal? Long { get; set; }
        public double? DistanceCount { get; set; }

    }


    public class DrpAdvertisementLocationViewModel
    {
        public int AdvertisementLocationID { get; set; }
        public string AdvertisementLocationName { get; set; }
    }

    public class AdvertisementReferenceViewModel
    {
        public int ID { get; set; }
        public string ReferenceName { get; set; }
    }
}
