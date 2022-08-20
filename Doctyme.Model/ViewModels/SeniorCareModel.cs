using Doctyme.Model.Utility;
using System;
using System.Collections.Generic;


namespace Doctyme.Model.ViewModels
{
    public class SeniorCareData
    {
        public int OrganisationId { get; set; }
        public int? OrganizationTypeID { get; set; }
        public int? UserTypeID { get; set; }
        public string Location
        {
            get
            {
                return Latitude + Longitude;
            }
        }

        public string Speciality { get; set; }
        public string Email { get; set; }
        public string OrganisationName { get; set; }
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool? IsLatLong { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool? EnabledBooking { get; set; }
        public string NPI { get; set; }        
        public int totRows { get; set; }
        //public int OrganisationId { get; set; }
        //public int OrganizationTypeID { get; set; }
        //public string OrganisationName { get; set; }        
        //public string AuthorizedOfficialFirstName { get; set; }         
        //public string AuthorizedOfficialLastName { get; set; }       
        //public string AuthorizedOfficialTelephoneNumber { get; set; }
        //public string AuthorizedOfficialTitleOrPosition { get; set; }
        //public string NPI { get; set; }
        //public bool IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string FullAddress { get; set; }        

        //public string FullName { get; set; }        
        //public string Email { get; set; }
        //public string Gender { get; set; }
    }

    public class SeniorCareDataListModel
    {
        public List<SeniorCareData> SeniorCareDataList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }

}
