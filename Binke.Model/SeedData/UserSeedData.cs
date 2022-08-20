using Binke.Model.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Binke.Model.SeedData
{
    public class UserSeedData
    {
        private readonly BinkeDbContext _context;

        public UserSeedData(BinkeDbContext context)
        {
            _context = context;
        }

        /**Create User with Super Admin role**/
        public void SeedAdminUser()
        {
            try
            {
                #region User Roles

                var manager = new RoleManager<Role, int>(new RoleStore(_context));
                var userStore = new UserStore<ApplicationUser, Role, int, UserLogin, UserRole, UserClaim>(_context);
                List<string> roles = new List<string>() { "Admin", "Doctor", "Facility", "Patient", "Pharmacy", "SeniorCare" };
                var existrolesList = _context.Roles.Select(x => x.Name).ToList();
                if (existrolesList.Any())
                {
                    var notExirst = roles.Except(existrolesList);
                    foreach (var notRole in notExirst)
                    {
                        manager.Create(new Role { Name = notRole });
                    }
                }
                else
                {
                    foreach (var objRole in roles)
                    {
                        manager.Create(new Role { Name = objRole });
                    }
                }
                #endregion

                var user = new ApplicationUser
                {
                    UserName = "cgankit@gmail.com",
                    Email = "cgankit@gmail.com",
                    Prefix = "Dr.",
                    Suffix = "MD",
                    FirstName = "Ankit",
                    MiddleName = "G.",
                    LastName = "Chodavadiya",
                    Gender = "Male",
                    CreatedDate = DateTime.UtcNow,
                    LastResetPassword = DateTime.Now,
                    EmailConfirmed = true,
                    IsActive = true,
                    IsDeleted = false,
                };


                if (_context.Users.Any(u => u.UserName == user.UserName)) return;

                var usermanager = new UserManager<ApplicationUser, int>(userStore);
                usermanager.Create(user, "P@ssword123");
                usermanager.AddToRole(user.Id, "Admin");
            }
            catch (Exception ex)
            {
                var log = new ErrorLog
                {
                    AppType = "SeedAdminUser",
                    Source = ex.Source,
                    TargetSite = ex.TargetSite.Name,
                    Type = ex.GetType().Name,
                    Message = ex.Message,
                    Stack = ex.StackTrace,
                    InnerExceptionMessage = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.Message + "Inner Exception:" + ex.InnerException.InnerException.Message : ex.InnerException.Message : string.Empty,
                    InnerStackTrace = ex.InnerException?.StackTrace,
                    LogDate = DateTime.UtcNow
                };

                WriteFile(log);
            }
        }

        private static void WriteFile(ErrorLog logs)
        {
            var fileName = "Error_Log_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";

            var path = @"~/Uploads/ErrorLog/" + fileName;
            var logFileInfo = new FileInfo(path);
            var fileStream = !logFileInfo.Exists ? logFileInfo.Create() : new FileStream(path, FileMode.Append);
            var log = new StreamWriter(fileStream);
            var errorString = $@"AppType :-
                                 =======================================================
                                 {logs.AppType}
                                 =======================================================
                                 Source :-
                                 =======================================================
                                 {logs.Source}
                                 =======================================================
                                 TargetSite :-
                                 =======================================================
                                 {logs.TargetSite}
                                 =======================================================
                                 Type :-
                                 =======================================================
                                 {logs.Type}
                                 =======================================================
                                 Message :-
                                 =======================================================
                                 {logs.Message}
                                 =======================================================
                                 Stack :-
                                 {logs.Stack}
                                 =======================================================
                                 InnerExceptionMessage :-
                                 { logs.InnerExceptionMessage}
                                 =======================================================
                                 InnerStackTrace :-
                                 { logs.InnerStackTrace}
";
            log.WriteLine(errorString);
            log.Close();
        }
    }

}
