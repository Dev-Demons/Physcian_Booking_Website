using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Doctyme.Model;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Doctyme.Repository.Interface;

namespace Binke.Api
{
    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {

        
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
           
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore(context.Get<DoctymeDbContext>()));

            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            manager.PasswordValidator = new PasswordValidator()
            {
                //RequiredLength = 6,
                //RequireNonLetterOrDigit = false,
                //// RequireDigit = true,
                //RequireLowercase = false,
                //RequireUppercase = false,
            };

            var dataProtectionProvider = options.DataProtectionProvider;

            
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            

            return (manager);
        }
      
    }
}