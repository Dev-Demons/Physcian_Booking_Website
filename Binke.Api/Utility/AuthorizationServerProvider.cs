using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using Doctyme.Repository.Services;
using Microsoft.AspNet.Identity.Owin;
using Doctyme.Model;
using System.Text;
using System.IO;

namespace Binke.Api.Utility
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.Parameters.Any())
            {
                 string uid = context.Parameters.Where(f => f.Key == "Login_Method").Select(f => f.Value).SingleOrDefault()[0];
                context.OwinContext.Set<string>("Login_Method", uid);
                //var client = new ApplicationClient
                //{
                //    Id = "MyApp",
                //    AllowedGrant = OAuthGrant.ResourceOwner,
                //    ClientSecretHash = new PasswordHasher().HashPassword("MySecret"),
                //    Name = "My App",
                //    CreatedOn = DateTimeOffset.UtcNow
                //};

                // context.OwinContext.Set<string>("oauth:ClientId", uid);

                context.Validated(); 
            }
          
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var claim=context.Identity.Claims;
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            //context.AdditionalResponseParameters.Add("Status", claim.Select(x => x.Properties.Keys.Contains("Status"));
            return Task.FromResult<object>(null);
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var form =await context.Request.ReadFormAsync();
            ApplicationUserManager _userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            UserRepository repo = new UserRepository(context.OwinContext.Get<DoctymeDbContext>());
            var manager=context.OwinContext.GetUserManager<ApplicationUser>();
          
            var LoginMethod = Convert.ToInt32(form["Login_Method"]);
            var email = false;
            if (LoginMethod == 1)
            {


                if (!string.IsNullOrEmpty(context.UserName))
                {
                   var  emailuser= await _userManager.FindByEmailAsync(context.UserName);
                    if (emailuser is null)
                    {
                        var jsonString = "{\"Status\":404,\"Message\":\"Invalid UserName\"}";
                        byte[] data = Encoding.UTF8.GetBytes(jsonString);
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "application/json";



                      //  await context.Response.Body.WriteAsync(data, 0, data.Length);
                        context.SetError("Status", "404");
                        context.SetError("Message", "Invalid UserName!");

                        return;
                    }
                   var  users=  await _userManager.FindAsync(context.UserName,context.Password);//_appUser.GetAll().Where(x => x.Email == model.UserName).Count() > 0;
                    if(users == null)
                    {
                       
                        var jsonString = "{\"Status\":404,\"Message\":\"Invalid Password\"}";
                        
                        context.SetError("Status", "404");
                        context.SetError("Message", "Invalid Password!");
                        // byte[] data = Encoding.UTF8.GetBytes(jsonString);

                        //context.Response.StatusCode = 200;
                        //context.Response.ContentType = "application/json";

                        //  context.SetError(new string(' ', jsonString.Length - 24));

                        // context.Response.StatusCode = 400;
                        //  context.Response.Write(jsonString);

                        //  context.Response.Write(data, 0, data.Length);
                        return;
                    }
                    else
                    {
                        if (users.EmailConfirmed)
                        {
                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                            identity.AddClaim(new Claim(ClaimTypes.Name, users.UserName));
                            identity.AddClaim(new Claim("UserId", users.Id.ToString()));
                            identity.AddClaim(new Claim("Status", "200"));
                            identity.AddClaim(new Claim("Message", "Login Successful !"));
                            var claimsPrincipal = new ClaimsPrincipal(identity);
                            // Set current principal
                            Thread.CurrentPrincipal = claimsPrincipal;

                            context.Validated(identity);
                        }
                        else
                        {
                            context.SetError("Status", "404");
                            context.SetError("Message", "Please Confirm your Email!");

                        }
                    }
                    //if (email)
                    //{
                    //    var user = _appUser.GetAll().Where(x => x.Email == model.UserName).ToList();
                    //    var password = user.Where(x => JsonConvert.DeserializeObject<ApiRegisterViewModel>(x.RegisterViewModel).Password == model.Password).Count() > 0;
                    //    if (password)
                    //    {
                    //        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, Message = "Login Successful !" });
                    //    }
                    //    else
                    //    {
                    //        return Request.CreateResponse(HttpStatusCode.OK, new { Status = 0, Message = "Incorrect Password !" });
                    //    }
                    //}
                    //else
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = 0, Message = "Incorrect Email !" });
                    //}
                }
                else
                {
                   
                    var jsonString = "{\"Status\":404,\"IsNewUser\":true,\"Message\":\"Invalid UserName\"}";
                    byte[] data = Encoding.UTF8.GetBytes(jsonString);
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";


                    context.SetError("Status", "404");
                    context.SetError("Message", "Please Enter UserName!");
                    // await context.Response.Body.WriteAsync(data, 0, data.Length);
                    return ;
                }
            }
            if (LoginMethod == 2)
            {
                var phn = false;
                if (!string.IsNullOrEmpty(context.UserName))
                {
                    phn = repo.GetAll().Where(x => x.PhoneNumber == context.UserName).Count() > 0;
                }
                if (phn)
                {
                    var users = repo.GetAll().Where(x => x.PhoneNumber == context.UserName).ToList();
                    // var password = user.Where(x => JsonConvert.DeserializeObject<ApiRegisterViewModel>(x.RegisterViewModel).Password == model.Password).Count() > 0;
                    if (phn)
                    {

                        if (users.FirstOrDefault().EmailConfirmed)
                        {


                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                            identity.AddClaim(new Claim(ClaimTypes.Name, users.FirstOrDefault().UserName));
                            identity.AddClaim(new Claim("UserId", users.FirstOrDefault().Id.ToString()));

                            var claimsPrincipal = new ClaimsPrincipal(identity);
                            // Set current principal
                            Thread.CurrentPrincipal = claimsPrincipal;

                            context.Validated(identity);
                        }
                        else
                        {
                            context.SetError("Status", "404");
                            context.SetError("Message", "Please Confirm your Email !");
                        }
                    }
                    else
                    {
                        var jsonString = "{\"Status\":404,\"IsNewUser\":true,\"Message\":\"Invalid UserName\"}";

                        byte[] data = Encoding.UTF8.GetBytes(jsonString);
                        context.Response.ContentType = "application/json";

                        await context.Response.Body.WriteAsync(data, 0, data.Length);
                        context.SetError("Status", "404");
                        context.SetError("Message", "Phone Number Doesn't Exist !");
                        return;
                    }
                }
                else
                {


                    var jsonString = "{\"Status\":404,\"IsNewUser\":true,\"Message\":\"Invalid UserName\"}";

                    byte[] data = Encoding.UTF8.GetBytes(jsonString);
                 
                    context.Response.ContentType = "application/json";

                    await context.Response.Body.WriteAsync(data, 0, data.Length);
                    context.SetError("Status", "404");
                    context.SetError("Message", "Phone Number Doesn't Exist !");
                    return;
                }

            }

            //if (string.IsNullOrEmpty(model.UserName))
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Status = HttpStatusCode.BadRequest, Message = "Please Enter UserName !", IsNewUser = true });
            //}
            //else
            //{
            //    context.SetError("invalid_grant", "Please Enter Correct Login Method.");
            //    context.SetError("Status", HttpStatusCode.BadRequest.ToString());
                
            //}
            //var user = await _userManager.FindAsync(context.UserName, context.Password);

            //if (user is null)
            //{
            //    context.SetError("invalid_grant", "Invalid username and/or password.");
            //    return;
            //}

            //if (user.EmailConfirmed)
            //{
            //    if (user.IsActive)
            //    {
            //        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //        identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            //        identity.AddClaim(new Claim("UserId", user.Id.ToString()));

            //        var claimsPrincipal = new ClaimsPrincipal(identity);
            //        // Set current principal
            //        Thread.CurrentPrincipal = claimsPrincipal;

            //        context.Validated(identity);
            //    }
            //    else
            //    {
            //        context.SetError("invalid_grant", "Your Account Is Not Active.");
            //    }
            //}
            //else
            //{
            //    context.SetError("invalid_grant", "Please check your email account to confirm your email address.");
            //}
        }
    }
}