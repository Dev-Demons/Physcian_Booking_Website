using System;
using System.Data.Entity;
using Unity;
using Unity.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Unity.Injection;
using Microsoft.Owin.Security;
using System.Web;
using Doctyme.Model;
using Doctyme.Repository.Interface;
using Binke.Repository.Services;
using Doctyme.Repository.Services;
using Doctyme.Repository;
using Doctyme.Repository.Interface;

namespace Binke
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static readonly Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        /// 
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            //container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            //DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            container
                .RegisterType<DbContext, Doctyme.Model.DoctymeDbContext>(new PerRequestLifetimeManager())
                .RegisterType<ApplicationSignInManager>()
                .RegisterType<ApplicationUserManager>(new PerRequestLifetimeManager())
                .RegisterType<ApplicationRoleManager>(new PerRequestLifetimeManager())
                .RegisterType<IAuthenticationManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication))
                .RegisterType<IUserStore<ApplicationUser, int>, UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>>()
                .RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole, string, IdentityUserRole>>()
                .RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>), new PerRequestLifetimeManager());

            #region _A_
            container.RegisterType<IAddressService, AddressRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAgeGroupService, AgeGroupRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAdvertisementService, AdvertisementRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAdvertisementLocationService, AdvertisementLocationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorAgeGroupService, DoctorAgeRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _B_
            container.RegisterType<IBoardCertificationService, BoardCertificationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBlogService, BlogRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _C_
            container.RegisterType<ICityService, CityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ICityStateZipService, CityStateZipRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IContactUsService, ContactUsRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _D_
            container.RegisterType<IDoctorService, DoctorRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorFacilityAffiliationService, DoctorFacilityAffiliationRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorInsuranceAcceptedService, DoctorInsuranceAcceptedRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorInsuranceService, DoctorInsuranceRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorImageService, DoctorImageRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorLanguageService, DoctorLanguageRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDoctorSpecialityService, DoctorSpecialityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDrugService, DrugRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDrugTypeService, DrugTypeRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDrugQuantityService, DrugQuantityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDrugManufacturerService, DrugManufacturerRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDrugDetailService, DrugDetailRepository>(new PerRequestLifetimeManager());
            //container.RegisterType<IDoctorInsuranceService, DoctorInsuranceRepository>(new PerRequestLifetimeManager());
            //container.RegisterType<IDrugPharmacyDetailService, DrugPharmacyDetailRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDashboardService, DashboardRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IDocOrgStateLicensesService, DocOrgStateLicensesRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _E_
            container.RegisterType<IErrorLogService, ErrorLogRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IExperienceService, ExperienceRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _F_
            container.RegisterType<IFacilityService, FacilityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IFacilityTypeService, FacilityTypeRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IFacilityOptionService, FacilityOptionRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IFeaturedDoctorService, FeaturedDoctorRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IFeaturedSpecialityService, FeaturedSpecialityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IFeaturedService, FeaturedRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _G_
            container.RegisterType<IGenderService, GenderRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _H_
            #endregion

            #region _I_
            container.RegisterType<IInsuranceTypeRepository, InsuranceTypeRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _J_

            #endregion

            #region _K_

            #endregion

            #region _L_
            container.RegisterType<ILanguageService, LanguageRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _M_
            #endregion

            #region _N_
            container.RegisterType<INewsletterSubscriberService, NewsletterSubscriberRepository>(new PerRequestLifetimeManager());            
            #endregion

            #region _O_
            container.RegisterType<IOpeningHourService, OpeningHourRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _P_
            container.RegisterType<IPatientService, PatientRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IPharmacyService, PharmacyRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _Q_
            container.RegisterType<IQualificationService, QualificationRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _R_
            //container.RegisterType<IRoleService, RoleRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IReviewService, ReviewRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IRepository, RepositoryService>(new PerRequestLifetimeManager());
            #endregion

            #region _S_
            container.RegisterType<ISeniorCareService, SeniorCareRepository>(new PerRequestLifetimeManager());
            //container.RegisterType<ISeniorCareImageService, SeniorCareImageRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ISlotService, SlotRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ISocialMediaService, SocialMediaRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ISpecialityService, SpecialityRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IStateService, StateRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _T_
            container.RegisterType<ITempSlotBookingService, TempSlotBookingRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ITestimonialsService, TestimonialRepository>(new PerRequestLifetimeManager());

            // As per Kazeem, DRUG RELATED TABLES ARE STILL UNDER CONSTRUCTIONS.
            //container.RegisterType<ITabletService, TabletRepository>(new PerRequestLifetimeManager());

            #endregion

            #region _U_
            container.RegisterType<IUserService, UserRepository>(new PerRequestLifetimeManager());
            //container.RegisterType<IUserRoleService, UserRoleRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IUserTypeService, UserTypeRepository>(new PerRequestLifetimeManager());
            #endregion

            #region _V_

            #endregion

            #region _W_

            #endregion

            #region _X_

            #endregion

            #region _Y_

            #endregion

            #region _Z_

            #endregion

            #region _Unity SignalR Dependency Register_
            #endregion

            #region Unity Error Log Factory Register
            //UnityFactory.SetFactory(() => container.Resolve<IErrorLogService>());
            #endregion
        }

        /*
        public static void RegisterComponents()
        {
            var containers = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            containers.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>),
                new HierarchicalLifetimeManager());
            containers.RegisterType<IContactUs, CustomerRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(containers));
        }
        */
    }
}
