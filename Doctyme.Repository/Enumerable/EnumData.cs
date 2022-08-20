namespace Doctyme.Repository.Enumerable
{

    public class UserRoles
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Facility = "Facility";
        public const string Patient = "Patient";
        public const string Pharmacy = "Pharmacy";
        public const string SeniorCare = "SeniorCare";
    }

    public class UserTypes
    {
        public const int User = 1;
        public const int Doctor = 2;
        public const int Pharmacy = 3;
        public const int Facility = 4;
        public const int SeniorCare = 5;
        public const int Admin = 6;
        public const int Staff = 9;
    }

    public class OrganisationTypes
    {
        public const int Pharmacy = 1005;
        public const int Facility = 1006;
        public const int SeniorCare = 1007;
        public const int Doctor = 1008;
    }

    public class UserClaims
    {
        public const string UserId = "UserId";
        public const string UserRole = "UserRole";
        public const string FirstName = "FirstName";
        public const string HeaderName = "HeaderName";
        public const string LoginHeader = "LoginHeader";
        public const string ProfilePicture = "ProfilePicture";
        public const string ResetPasswordDays = "ResetPasswordDays";
        public const string UserTypeWishId = "UserTypeWishId";
        public const string IsRemember = "IsRemember";
    }

    public class GenderTypes
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
    }

    public class EmailTemplate
    {
        public const string ConfirmEmail = "ConfirmEmail.html";
        public const string PrescriptionEmail = "PrescriptionEmail.html";
        public const string ConfirmSlot = "SlotConfirm.html";
        public const string ClaimVerify = "ClaimVerify.html";
    }

    public class FilePathList
    {
        public const string FeaturedDoctor = "/Uploads/FeaturedDoctor/";
        public const string FeaturedFacility = "/Uploads/FacilitySiteImages/";
        public const string Doctor = "/Uploads/Doctor/";
        public const string SeniorCare = "/Uploads/SeniorCareProfile/";
        public const string ProfilePic = "/Uploads/ProfilePic/";
        public const string FeaturedSpeciality = "/Uploads/FeaturedSpeciality/";
        public const string SiteImage = "/Uploads/SiteImage/";
        public const string ClaimDocument = "/Uploads/Attachment/Doctor/";
        public const string Advertisement = "/Uploads/Advertisement/";
    }

    public class StaticFilePath
    {
        public const string ProfilePicture = "~/Uploads/ProfilePic/doctor.jpg";
        public const string WebSettings = "~/Uploads/WebSettings/HomePageUserCount.json";
        public const string AdminProfilePicture = "/Content/admin/img/user.png";
        public const string recordslogFile = "/recordslog.txt";
    }

    public class StoredProcedureList
    {
        public const string GetIPZIPMapping = @"prc_IPCityZip_Get";
        public const string GetDocAddList = @"GetDocAdvertisements";
        public const string GetLoginBanner = @"GetLoginBanner";
        public const string GetAddList = @"GetAdvertisements";
        public const string GetDrugTabList = @"GetDrugTabs";
        public const string GetDoctorFilterSearch = @"sp_GetDoctorsFilterList";
        public const string GetDoctorFilterSearch_v3 = @"GetDoctorFilterList_v3";
        public const string AgeGroupsSeen = @"EXEC AgeGroupsSeen;";
        public const string AgeGroupsSeen_v1 = @"AgeGroupsSeen_v1;";
        public const string GetSlotList = @"GetSlotList @DoctorId, @iDisplayStart, @iDisplayLength, @SortColumn, @SortDir, @Search, @SearchRecords OUTPUT";
        public const string SearchDoctorList = @"SearchDoctorList @Search, @Distance, @DistanceSearch, @IsAllowNewPatient, @IsNtPcp, @IsPrimaryCare,  @Specialties,@Language, @Affiliations, @Insurance, @AGS, @AGSFull, @Sorting";
        public const string GetDoctorDetails = @"GetDoctorDetails";
        public const string GetDoctorDetails_v1 = @"GetDoctorDetails_v1";
        public const string GETDoctorListPagination = @"GETDoctorListPagination";
        public const string GETDoctorListPagination_v3 = @"GETDoctorListPagination_v3";
        public const string GetFeaturedDoctorIdS = @"GetFeaturedDoctorIdS";
        public const string GetFeaturedDoctorIdS_v1 = @"GetFeaturedDoctorIdS_v1";
        public const string GetFeaturedFacilityIds = @"GetFeaturedFacilityIds";
        public const string GetFeaturedFacilityIds_v1 = @"GetFeaturedFacilityIds_v1";
        public const string SearchPharmacyList = @"SearchPharmacyList @Search, @Distance, @DistanceSearch, @PharmacyId, @Sorting";
        public const string SearchSeniorCare = @"SearchSeniorCare @Search, @Distance, @DistanceSearch, @Id, @Sorting";
        public const string SearchFacility = @"SearchFacility @Search, @Distance, @DistanceSearch, @Id, @Sorting";
        public const string SearchDrug = @"SearchDrug";
        public const string spGetCityStateZipByZipcode = @"spGetCityStateZipByZipcodeCity";
        public const string spGetSpecialization = @"GetSpecialization";
        public const string spGetDrug = @"GetDrug";
        public const string spGetDoctorListAutoCompleteByDoctorName = @"spGetDoctorListAutoCompleteByDoctorName";

        public const string GetMonthlyAppointmentCount = @"GetMonthlyAppointmentCount @Id,@FromDate, @ToDate, @UserFlag";
        public const string GetMonthlyPatientCount = @"GetMonthlyPatientCount @Id, @FromDate, @ToDate, @UserFlag";
        public const string GetAppointmentsByUser = @"sprGetAppointmentsByUser @ReferenceId, @UserTypeId, @FromDate, @ToDate";
        public const string GetPatientsByUser = @"sprGetPatientsByUser @ReferenceId, @UserTypeId, @FromDate, @ToDate";
        public const string GetNewPatientList = @"GetNewPatientList @Id, @UserFlag";
        public const string GetRecentlyAddedNewPatientList = @"GetRecentlyAddedNewPatientList @Id, @UserFlag";
        public const string GetRecentlyCompletedAppointmentList = @"GetRecentlyCompletedAppointmentList @Id, @UserFlag";
        public const string GetTodaysAppointmentList = @"GetTodaysAppointmentList @Id, @UserFlag";
        public const string GetDoctorProfileData = @"EXEC dtym_GetDoctorProfileData";

        public const string SearchDoctorListPagination = @"SearchDoctorListPagination @Search, @Distance, @DistanceSearch, @Latitude, @Longitude, @IsAllowNewPatient, @IsNtPcp, @IsPrimaryCare,  @Specialties,@Language, @Affiliations, @Insurance, @AGS, @AGSFull, @Sorting";
        public const string GetHomePageFeaturedDoctorList = @"GetHomePageFeaturedDoctorList";
        public const string GetHomePageFeaturedFacilityList = @"GetHomePageFeaturedFacilityList";
        public const string GetOrganisationListByTypeId = @"GetOrganisationListByTypeId";
        public const string GetOrganisationListByTypeId_HomePageSearch = @"GetOrganisationListByTypeId_HomePageSearch";
        public const string GetOrganisationListByTypeId_HomePageSearch_v1 = @"GetOrganisationListByTypeId_HomePageSearch_v1";
        public const string GetListOrganisationByType_v1 = @"GetListOrganisationByType_v1";
        public const string GetFacilitiByTypeId = @"GetFacilitiByTypeId";
        public const string spGetOrgNameByTypeId = @"spGetOrgNameByTypeId";
        public const string spGetExistingPrescription = @"spGetExistingRefillPrescription";
        public const string spGetBasicPatientProfile = @"EXEC spBasicPatientProfile_Get ";

        public const string GetOrganisationByOrgId = @"EXEC prc_getOrganizationByID";
        public const string GetDrpInsuranceList = @"GetDrpInsuranceList";
        public const string GetTimeSlots = @"prc_Get_TimeSlots_By_Doctor_ID";
        public const string GetUerDetailsByUserID = @"prc_GetUserDetails_ById";
        public const string GetInsurancePlanById = @"prc_GetInsuranceType_By_Id";
        public const string GetInsurancePlanByTypeId = @"prc_GetInsurancePlan_By_TypeId";
        public const string InsertSlot = @"EXEC prc_insertSlots";
        public const string GetStatesByCountry = @"EXEC GetStatesByCountry";
        public const string GetReview = @"GetReviewsFromOrgId";
        public const string GetOpeningHour = @"spOpeningHour_Get";
        public const string GetStateLicense = @"getStateLicensesByOrgId";
        public const string GetInsurancePlan = @"GetInsurancePlanFromOrgId";
        public const string GetSummary = @"spGetSummary";
        public const string UpdateReview = @"spReview_Update";
        public const string GetSpecialities = @"spFacilitySpeciality_Get";

        public const string GetCitiesByStates = @"EXEC GetCityesByState ";
        public const string GetZipCodeaByCities = @"EXEC GetZipCodesByCity ";

        public const string GetSeniorCareListByType = @"GetSeniorCareListByType";
        public const string GetSeniorCareProfileUpdate = @"spSeniorCareProfile_Update";
        public const string GetSeniorCareOrganizationAmenityOption = @"spOrganizationAmenityOption_Get";
        public const string GetSeniorCareSiteImage = @"spSeniorCareSiteImage_Get";

        public const string GetSeiorCareProfileDetails = @"EXEC prc_SeniorCareProfile_Get ";
        public const string GetFacilityProfileDetails = @"EXEC prc_Facility_Profile_Get ";
        public const string GetPharmacyProfileDetails = @"EXEC prc_Pharmacy_Profile_Get ";
        public const string GetTimeSlotsByOrgID = @"EXEC prc_Get_TimeSlots_By_Org_ID ";
        public const string GetOrganizationAddressByOrgId = @"prc_GetOrganizationAddress_Get ";
        public const string InsertBusinessClaim = @"prc_BusinessClaim_Insert ";
        public const string UpdateBusinessClaim = @"prc_BusinessClaim_Update ";
        public const string GetCategorySubCategory = @"prc_Category_SubCategory_Get";
        public const string InsertBlogItem = @"prc_BlogItem_Insert";
        public const string GetAllBlog = @"prcBlogIndex_Get";
        public const string GetAllBlogWithInActive = @"prcBlogIndex_Get_v1";
        public const string GetAllBlogSiteMap = @"prc_Get_BlogName";
        public const string GetDrugNameSiteMap = @"prc_Get_DrugName";
        public const string GetBlogById = @"prc_getBlog_ById";
        public const string GetIsMainSiteCount = @"sp_GetIsMainSiteCount";
        public const string GetBlogDetails = @"prc_Get_BlogDetails";
        public const string GetHomePageBlogDetails = @"prc_GetHomePage_BlogDetails";
        public const string GetDrugDetailsPageData = @"GetdataForDrugDetailsPage";
        public const string GetMostlySearchedMedicines = @"GetMostlySearchedMedicines";


        //Testimonials
        public const string GetAllTestimonials = @"prc_TestimonialsIndex_Get";
        public const string GetTestimonialById = @"prc_getTestimonial_ById";
        public const string InsertTestimonialItem = @"prc_TestimonialItem_Insert";
        public const string TestimonialItemUpdateStatus = @"prc_TestimonialItem_Update";
        public const string TestimonialItemsForHome = @"prc_getTestimonialsForHome";
        public const string sp_GetUserAddressCityStateZip = @"sp_GetUserAddressCityStateZip";

    }

    public class SqlQuery
    {
        public const string GeLanguageDropDownList = @"SELECT * FROM Language WHERE IsActive = 1 AND IsDeleted = 0";
        public const string GeFacilityTypeDropDownList = @"SELECT OrganizationTypeID, Org_Type_Name AS OrganizationTypeName FROM OrganisationType WHERE IsActive = 1 AND IsDeleted = 0";
    }

    public class UserFlag
    {
        public const string Doctor = "Doctor";
    }

    public class UserTabsLookup
    {
        public const int Overview = 1;
        public const int SideEffects = 2;
        public const int DosageformsandStrengths = 3;
        public const int DrugInteractions = 4;
        public const int Symptoms = 5;
        public const int IndicationandUsage = 9;
        public const int DosageandAdministration = 10;
        public const int Contradictions = 12;
    }
}
