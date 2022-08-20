using System;
using System.Data.Entity.Migrations;
using System.Web.Mvc;
using AutoMapper;
using Doctyme.Model;
using Binke.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using Unity;
using System.Configuration;

namespace Binke
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            UnityConfig.Container.RegisterInstance(app.GetDataProtectionProvider());

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationSignInManager>());
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: GrabUserId)
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            //---------------------------------------------------------------------------------------
            //// Vishal & Youssef commented this blok till add migration configuration
            //AutoMapperConfigure();
            //var configuration = new Configuration { ContextType = typeof(BinkeDbContext) };
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
        }

        public static int GrabUserId(System.Security.Claims.ClaimsIdentity claim)
        {
            int.TryParse(claim.GetUserId(), out var id);
            return id;
        }

        public void AutoMapperConfigure()
        {
            //// Vishal & Youssef commented this blok till confirm with Kazeem what should we do
            Mapper.Initialize(autoConfig =>
            {
                autoConfig.CreateMap<AddressViewModel, Address>()
                    .ForMember(des => des.AddressId, (IMemberConfigurationExpression<AddressViewModel, Address, int> opt) => opt.MapFrom(src => src.Id))
                    //.ForMember(des => des.DoctorId, (IMemberConfigurationExpression<AddressViewModel, Address, int?> opt) => opt.MapFrom(src => src.DoctorId))
                    //.ForMember(des => des.FacilityId, (IMemberConfigurationExpression<AddressViewModel, Address, int?> opt) => opt.MapFrom(src => src.FacilityId))
                    //.ForMember(des => des.PatientId, (IMemberConfigurationExpression<AddressViewModel, Address, int?> opt) => opt.MapFrom(src => src.PatientId))
                    .ReverseMap();
               
                autoConfig.CreateMap<DoctorBasicInformation, Doctor>()
                .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<DoctorBasicInformation, Doctor, int> opt) => opt.MapFrom(src => src.Id))
                .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<DoctorBasicInformation, Doctor, int> opt) => opt.MapFrom(src => src.DoctorId))
                .ReverseMap();
                //    autoConfig.CreateMap<DoctorAffiliationModel, DoctorFacilityAffiliation>()
                //        .ForMember(des => des.AffiliationId, (IMemberConfigurationExpression<DoctorAffiliationModel, DoctorFacilityAffiliation, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<DoctorAffiliationModel, DoctorFacilityAffiliation, int> opt) => opt.MapFrom(src => src.DoctorId))
                //        .ReverseMap();



                //    autoConfig.CreateMap<DoctorImageViewModel, DoctorImage>()
                //       .ForMember(des => des.DoctorImageId, (IMemberConfigurationExpression<DoctorImageViewModel, DoctorImage, int> opt) => opt.MapFrom(src => src.Id))
                //       .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<DoctorImageViewModel, DoctorImage, int> opt) => opt.MapFrom(src => src.DoctorId))
                //       .ReverseMap();
                //    autoConfig.CreateMap<ExperienceViewModel, Experience>()
                //       .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<ExperienceViewModel, Experience, int> opt) => opt.MapFrom(src => src.Id))
                //       .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<ExperienceViewModel, Experience, int> opt) => opt.MapFrom(src => src.DoctorId))
                //       .ReverseMap();
                //    autoConfig.CreateMap<FacilityBasicInformation, Model.Facility>()
                //      .ForMember(des => des.FacilityId, (IMemberConfigurationExpression<FacilityBasicInformation, Model.Facility, int> opt) => opt.MapFrom(src => src.Id))
                //      .ForMember(des => des.FacilityId, (IMemberConfigurationExpression<FacilityBasicInformation, Model.Facility, int> opt) => opt.MapFrom(src => src.FacilityId))
                //      .ReverseMap();
                //    autoConfig.CreateMap<FeaturedDoctorViewModel, FeaturedDoctor>()
                //      .ForMember(des => des.FeaturedDoctorId, (IMemberConfigurationExpression<FeaturedDoctorViewModel, FeaturedDoctor, int> opt) => opt.MapFrom(src => src.Id))
                //      .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<FeaturedDoctorViewModel, FeaturedDoctor, int> opt) => opt.MapFrom(src => src.DoctorId))
                //      .ReverseMap();
                //    autoConfig.CreateMap<FeaturedSpecialityViewModel, FeaturedSpeciality>()
                //      .ForMember(des => des.FeaturedSpecialityId, (IMemberConfigurationExpression<FeaturedSpecialityViewModel, FeaturedSpeciality, int> opt) => opt.MapFrom(src => src.Id))
                //      .ForMember(des => des.SpecialityId, (IMemberConfigurationExpression<FeaturedSpecialityViewModel, FeaturedSpeciality, short> opt) => opt.MapFrom(src => src.SpecialityId))
                //      .ReverseMap();
                //    autoConfig.CreateMap<OpeningHoursViewModel, OpeningHour>()
                //        .ForMember(des => des.OpeningHourId, (IMemberConfigurationExpression<OpeningHoursViewModel, OpeningHour, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<OpeningHoursViewModel, OpeningHour, int?> opt) => opt.MapFrom(src => src.DoctorId))
                //        .ReverseMap();
                //    autoConfig.CreateMap<PatientBasicInformation, Patient>()
                //        .ForMember(des => des.PatientId, (IMemberConfigurationExpression<PatientBasicInformation, Patient, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.PatientId, (IMemberConfigurationExpression<PatientBasicInformation, Patient, int> opt) => opt.MapFrom(src => src.PatientId))
                //        .ReverseMap();
                //    autoConfig.CreateMap<PharmacyBasicInformation, Pharmacy>()
                //     .ForMember(des => des.PharmacyId, (IMemberConfigurationExpression<PharmacyBasicInformation, Pharmacy, int> opt) => opt.MapFrom(src => src.Id))
                //     .ForMember(des => des.PharmacyId, (IMemberConfigurationExpression<PharmacyBasicInformation, Pharmacy, int> opt) => opt.MapFrom(src => src.PharmacyId))
                //     .ReverseMap();
                //    autoConfig.CreateMap<SlotViewModel, Slot>()
                //        .ForMember(des => des.SlotId, (IMemberConfigurationExpression<SlotViewModel, Slot, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<SlotViewModel, Slot, int?> opt) => opt.MapFrom(src => src.DoctorId))
                //        .ForMember(des => des.FacilityId, (IMemberConfigurationExpression<SlotViewModel, Slot, int?> opt) => opt.MapFrom(src => src.FacilityId))
                //        .ForMember(des => des.SeniorCareId, (IMemberConfigurationExpression<SlotViewModel, Slot, int?> opt) => opt.MapFrom(src => src.SeniorCareId))
                //        .ReverseMap();
                //    autoConfig.CreateMap<SocialMediaViewModel, SocialMedia>()
                //        .ForMember(des => des.SocialMediaId, (IMemberConfigurationExpression<SocialMediaViewModel, SocialMedia, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<SocialMediaViewModel, SocialMedia, int?> opt) => opt.MapFrom(src => src.DoctorId))
                //        .ForMember(des => des.SeniorCareId, (IMemberConfigurationExpression<SocialMediaViewModel, SocialMedia, int?> opt) => opt.MapFrom(src => src.SeniorCareId))
                //        .ReverseMap();
                //    autoConfig.CreateMap<QualificationViewModel, Qualification>()
                //        .ForMember(des => des.QualificationId, (IMemberConfigurationExpression<QualificationViewModel, Qualification, int> opt) => opt.MapFrom(src => src.Id))
                //        .ForMember(des => des.DoctorId, (IMemberConfigurationExpression<QualificationViewModel, Qualification, int> opt) => opt.MapFrom(src => src.DoctorId))
                //        .ReverseMap();

            });
        }
    }
}
