using Binke.Api.Controllers;
using Doctyme.Model;
using Doctyme.Repository;
using Doctyme.Repository.Interface;
using Doctyme.Repository.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.WebApi;

namespace Binke.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            container

                .RegisterType<DbContext, Doctyme.Model.DoctymeDbContext>()
                .RegisterType<IUserStore<ApplicationUser, int>, UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>>()
                .RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole, string, IdentityUserRole>>();
            container.RegisterType<IDashboardService, DashboardRepository>();
            container.RegisterType<IDoctorService, DoctorRepository>();
            container.RegisterType<IRepository, RepositoryService>();
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType(typeof(IUserStore<>), typeof(UserStore<>));
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            // Added by Nagaraj 13-jan-2020 
            container.RegisterType<IFeaturedSpecialityService, FeaturedSpecialityRepository>();
            container.RegisterType<IFeaturedDoctorService, FeaturedDoctorRepository>();
            container.RegisterType<IDoctorFacilityAffiliationService, DoctorFacilityAffiliationRepository>();
            container.RegisterType<IDoctorService, DoctorRepository>();
            container.RegisterType<IFacilityService, FacilityRepository>();
            container.RegisterType<IPharmacyService, PharmacyRepository>();
            container.RegisterType<ISeniorCareService, SeniorCareRepository>();
            container.RegisterType<ISpecialityService, SpecialityRepository>();
            // End

            container.RegisterType<IFeaturedService, FeaturedRepository>();
            container.RegisterType<IDoctorInsuranceAcceptedService, DoctorInsuranceAcceptedRepository>();
            container.RegisterType<IDoctorInsuranceService, DoctorInsuranceRepository>();
            container.RegisterType<IAgeGroupService, AgeGroupRepository>();
            container.RegisterType<IDoctorImageService, DoctorImageRepository>();
            container.RegisterType<IDoctorFacilityAffiliationService, DoctorFacilityAffiliationRepository>();
            container.RegisterType<IPatientService, PatientRepository>();
            container.RegisterType<IUserService, UserRepository>();
            container.RegisterType<ISeniorCareService, SeniorCareRepository>();
            container.RegisterType<ApplicationUserManager>();
             

            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            
        }
    }
}