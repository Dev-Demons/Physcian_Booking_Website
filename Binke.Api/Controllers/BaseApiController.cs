using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Security.Claims;

namespace Binke.Api
{
    public class BaseApiController : ApiController
    {
        protected int UserId { get; set; }

        public BaseApiController()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var id = identity.Claims.Where(c => c.Type == "UserId").Select(x => x.Value).SingleOrDefault();

            if (!string.IsNullOrEmpty(id))
            {
                UserId = Convert.ToInt32(id);
            }
        }
    }
}
