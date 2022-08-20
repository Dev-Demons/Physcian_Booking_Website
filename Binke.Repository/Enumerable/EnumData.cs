namespace Binke.Repository.Enumerable
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
    }

    public class FilePathList
    {
        public const string FeaturedDoctor = "/Uploads/FeaturedDoctor/";
        public const string Doctor = "/Uploads/Doctor/";
        public const string SeniorCare = "/Uploads/SeniorCare/";
        public const string ProfilePic = "/Uploads/ProfilePic/";
        public const string FeaturedSpeciality = "/Uploads/FeaturedSpeciality/";
        
    }

    public class StaticFilePath
    {
        public const string ProfilePicture = "~/Uploads/ProfilePic/doctor.jpg";
        public const string WebSettings = "~/Uploads/WebSettings/HomePageUserCount.json";
    }

    public class StoredProcedureList
    {
        public const string AgeGroupsSeen = @"EXEC AgeGroupsSeen;";
        public const string GetSlotList = @"GetSlotList @DoctorId, @iDisplayStart, @iDisplayLength, @SortColumn, @SortDir, @Search, @SearchRecords OUTPUT";
        public const string SearchDoctorList = @"SearchDoctorList @Search, @Distance, @DistanceSearch, @IsAllowNewPatient, @IsNtPcp, @IsPrimaryCare,  @Specialties,@Language, @Affiliations, @Insurance, @AGS, @AGSFull, @Sorting";
        public const string SearchPharmacyList = @"SearchPharmacyList @Search, @Distance, @DistanceSearch, @PharmacyId, @Sorting";
        public const string SearchSeniorCare = @"SearchSeniorCare @Search, @Distance, @DistanceSearch, @Id, @Sorting";
        public const string SearchFacility = @"SearchFacility @Search, @Distance, @DistanceSearch, @Id, @Sorting";
        public const string SearchDrug = @"SearchDrug";

        public const string GetMonthlyAppointmentCount = @"GetMonthlyAppointmentCount @Id,@FromDate, @ToDate, @UserFlag";
        public const string GetMonthlyPatientCount = @"GetMonthlyPatientCount @Id, @FromDate, @ToDate, @UserFlag";
        public const string GetNewPatientList = @"GetNewPatientList @Id, @UserFlag";
        public const string GetRecentlyAddedNewPatientList = @"GetRecentlyAddedNewPatientList @Id";
        public const string GetRecentlyCompletedAppointmentList = @"GetRecentlyCompletedAppointmentList @Id, @UserFlag";
        public const string GetTodaysAppointmentList = @"GetTodaysAppointmentList @Id, @UserFlag";

    }

    public class UserFlag
    {
        public const string Doctor = "Doctor";
    }
}
