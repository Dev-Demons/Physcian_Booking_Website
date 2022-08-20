using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.Owin.Security;

namespace Binke.App_Helpers
{
    public static class ClaimsExtensions
    {
        public static void AddUpdateClaim(this IIdentity identity, string key, string value)
        {
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = ((ClaimsIdentity)identity).FindFirst(key);
            if (existingClaim != null)
                ((ClaimsIdentity)identity).RemoveClaim(existingClaim);

            // add new claim
            ((ClaimsIdentity)identity).AddClaim(new Claim(key, value));
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsIdentity(identity), new AuthenticationProperties() { IsPersistent = true });

        }

        public static string GetClaimValue(this IIdentity identity, string key)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(key);
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
