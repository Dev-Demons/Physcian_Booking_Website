using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Binke.CommonUtility;
using Binke.Models;
using Binke.Models.SiteMapModels;
using Binke.Utility;
using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;

namespace Binke.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly IAddressService _address;
        private readonly IDoctorService _doctor;
        private readonly IPharmacyService _pharmacy;
        private readonly IFacilityService _facility;
        private readonly ISeniorCareService _seniorCare;
        private ApplicationUserManager _userManager;
        private readonly IStateService _state;
        private readonly IUserService _appUser;

        private static Timer _sitemapTimer;
        public class FileInfo //Added by Reena
        {
            public int FileId
            {
                get;
                set;
            }
            public string FileName
            {
                get;
                set;
            }
            public string FilePath
            {
                get;
                set;
            }
        }

        [System.ComponentModel.TypeConverter(typeof(System.Configuration.TimeSpanSecondsConverter))]
        [System.Configuration.ConfigurationProperty("shutdownTimeout", DefaultValue = "00:01:30")]
        public TimeSpan ShutdownTimeout { get; set; }

        public SubmissionController(IAddressService address, IDoctorService doctor, IPharmacyService pharmacy, IFacilityService facility, ISeniorCareService seniorCare, IStateService state, IUserService appUser, ApplicationUserManager applicationUserManager)
        {
            _address = address;
            _doctor = doctor;
            _pharmacy = pharmacy;
            _facility = facility;
            _seniorCare = seniorCare;
            _state = state;
            _appUser = appUser;
            _userManager = applicationUserManager;
        }

        #region SiteMaps
        // SiteMaps
        public ActionResult SiteMaps()
        {
            ViewBag.InProgress = false;
            ViewBag.Message = string.Empty;
            if (SiteMapStatusModel.InProgress)
            {
                ViewBag.InProgress = true;
                ViewBag.Message = "Sitemap Generate inprogress and the process may take time to complete, please wait...";
            }
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/SiteMaps/"));
            if (!dir.Exists)
            {
                Directory.CreateDirectory(Server.MapPath("~/SiteMaps"));
            }

            ////var Files = dir.GetFiles("*.xml");
            var Files = dir.GetFiles();//Added by Reena

            ViewBag.Count = Files.Length;

            ViewBag.Files = Files;
            return View();
        }

        public PartialViewResult GetSiteMapList()
        {
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/SiteMaps/"));
            if (!dir.Exists)
            {
                Directory.CreateDirectory(Server.MapPath("~/SiteMaps"));
            }

            ////var Files = dir.GetFiles("*.xml");
            var Files = dir.GetFiles();//Added by Reena
            return PartialView(@"Partial/_SitemapFiles", Files);
        }


        public ActionResult GenerateSiteMaps()
        {
            //ShutdownTimeout = TimeSpan.FromMinutes(30);
            //System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => ProcessGenerateSiteMaps());
            _sitemapTimer = new Timer(ProcessGenerateSiteMaps,
                                   null, 0, 600000);
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSitemapStatus()
        {
            if (!SiteMapStatusModel.InProgress && _sitemapTimer != null)
            {
                _sitemapTimer.Dispose();
            }
            return Json(SiteMapStatusModel.InProgress, JsonRequestBehavior.AllowGet);
        }

        private void ProcessGenerateSiteMaps(object state)
        {
             var logFile = GetFileName("ProcessGenerate.txt" , Server.MapPath("~/Uploads/"));
            try
            {
                if (!SiteMapStatusModel.InProgress)
                {
                    SiteMapStatusModel.InProgress = true;
                    DeleteOlderSitemapFiles();

                    List<string> mapNames = new List<string>();
                    try
                    {
                        WriteTextFile(logFile, "1 Doctor");
                        string doctor = GenerateDoctorSiteMap(); 
                        WriteTextFile(logFile, "1 End ");//-- Doctors sitemaps
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "1 End-problem " + ex.Message);//-- Doctors sitemaps
                    }//-- Doctors sitemaps
                                                             //if(doctor != "")
                                                             //{
                                                             //    mapNames.Add(doctor);
                    try
                    {
                        WriteTextFile(logFile, "Pharmacy  start ");                                         //}
                        string pharmacy = GeneratePharmacySiteMap(); //-- Pharmacy sitemaps
                        WriteTextFile(logFile, "Pharmacy  end ");
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "Pharmacy  End-problem " + ex.Message);
                    }
                                                                 //if (pharmacy != "")
                                                                 //{
                                                                 //    mapNames.Add(pharmacy);
                                                                 //}
                    try
                    {
                        WriteTextFile(logFile, "2 Facility");
                        string facility = GenerateFacilitySiteMap(); //-- Facility sitemaps
                        WriteTextFile(logFile, "2 End ");
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "2 Facilty  End-problem " + ex.Message);
                    }
                                                                 //if (facility != "")
                                                                 //{
                                                                 //    mapNames.Add(facility);
                                                                 //}
                    try
                    {
                        WriteTextFile(logFile, "3 SeniorCare");
                        string seniorCare = GenerateSeniorCareSiteMap(); //-- Senior Care sitemaps
                        WriteTextFile(logFile, " 3 End ");
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "SeniorCare  End-problem " + ex.Message);

                    }
                                                                     //if (seniorCare != "")
                                                                     //{
                                                                     //    mapNames.Add(seniorCare);
                                                                     //}
                    try
                    {
                        WriteTextFile(logFile, "4 Article");
                        string article = GenerateArticleNameSiteMap(); //-- article sitemaps
                        WriteTextFile(logFile, "4 End ");
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "4 article End-problem " + ex.Message);
                    }

                    try
                    {
                        WriteTextFile(logFile, "5 Drug ");
                        string drugname = GenerateDrugNameSiteMap(); //-- drug sitemaps
                        WriteTextFile(logFile, "5 End ");
                    }
                    catch (Exception ex)
                    {
                        WriteTextFile(logFile, "4 drug End-problem " + ex.Message);
                    }

                    GenerateSiteMapIndex();
                    GenerateRobotTextfile(); //Added by Reena
                    SiteMapStatusModel.InProgress = false;
                }
            }
            catch (Exception ex)
            {
                WriteTextFile(logFile, "Error of siteMap"+ex.Message);
                LogError.WriteErrorLog(ex.Message.ToString(), "Submission Controller", "Generate Site Map", Server.MapPath("~/ErrorLog/"));
            }
        }

        // --- Replace Organization name to correct url format
        public string StringReplace(string str)
        {
            string result = "";
            str = str.Trim();
            //result = str.Replace(" & ", "-");
            //result = str.Replace(" ", "-");
            //result = str.Replace(" ", "-");
            //result = str.Replace(".", "");
            result = str.Replace(" & ", "-");
            result = result.Replace(". ", "");
            result = result.Replace(".", "");
            result = result.Replace(".-", "");
            result = result.Replace("; ", "");
            result = result.Replace("  ", "-");
            result = result.Replace(" ", "-");
            result = result.Replace("---", "-");
            result = result.Replace("--", "-");

            return result;
        }

        //-- Generate doctors sitemaps 
        public string GenerateDoctorSiteMap()
        {
            var logFile = GetFileName("ProcessGenerate.txt", Server.MapPath("~/Uploads/"));
            WriteTextFile(logFile, "Generate Doctor SIteMap");
            var allDoctors = _doctor.GetAll();

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
            foreach (var item in allDoctors)
            {
               try
                {
                    if (!string.IsNullOrWhiteSpace(item.NPI))
                    {
                        string npi = item.NPI;
                        string urlDoctorName = "";
                        item.Credential = item.Credential == null ? "" : item.Credential;
                        item.LastName = item.LastName == null ? "" : item.LastName;
                        item.FirstName = item.FirstName == null ? "" : item.FirstName;
                        item.MiddleName = item.MiddleName == null ? "" : item.MiddleName;
                        if (!string.IsNullOrEmpty(item.FirstName) && !string.IsNullOrEmpty(item.LastName))
                        {
                            //if (!string.IsNullOrEmpty(item.MiddleName))
                            //    urlDoctorName = Convert.ToString(item.LastName).Replace(".", "").Replace(" ", "-") + "-" + Convert.ToString(item.MiddleName).Replace(".", "").Replace(" ", "-") + "-" + Convert.ToString(item.FirstName).Replace(".", "").Replace(" ", "-") + "-" + Convert.ToString(item.Credential).Replace(".", "").Replace(" ", "-");
                            //else
                            //    urlDoctorName = Convert.ToString(item.LastName).Replace(".", "").Replace(" ", "-") + "-" + Convert.ToString(item.FirstName).Replace(".", "").Replace(" ", "-") + "-" + Convert.ToString(item.Credential).Replace(".", "").Replace(" ", "-");
                            if (!string.IsNullOrEmpty(item.Credential))
                            {
                                if (!string.IsNullOrEmpty(item.MiddleName))
                                    urlDoctorName = Convert.ToString(item.LastName.Trim()) + "-" + Convert.ToString(item.MiddleName.Trim()) + "-" + Convert.ToString(item.FirstName.Trim()) + "-" + Convert.ToString(item.Credential.Trim());
                                else if (!string.IsNullOrEmpty(item.Credential))
                                    urlDoctorName = Convert.ToString(item.LastName.Trim()) + "-" + Convert.ToString(item.FirstName.Trim()) + "-" + Convert.ToString(item.Credential.Trim());
                            }
                            else
                            {
                                urlDoctorName = Convert.ToString(item.LastName.Trim()) + "-" + Convert.ToString(item.FirstName.Trim());
                            }
                            string url = "https://www.doctyme.com/Profile/Doctor/" + StringReplace(urlDoctorName) + "-" + item.NPI.ToString();
                            sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                            count++;
                            
                            if (count == 50000)
                            {
                                if (mapIndex == 0)
                                    mapName = "doctor-sitemap.xml";
                                else
                                    mapName = "doctor-sitemap" + mapIndex.ToString() + ".xml";

                                SitemapGenerator sg = new SitemapGenerator();
                                var doc = sg.GenerateSiteMap(sitemapItems);
                                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                                System.Threading.Tasks.Task.Delay(1000);
                                sitemapItems.Clear();
                                count = 0;
                                mapIndex++;
                                WriteTextFile(logFile, "Doctor SIteMap"+ mapName);//sonu
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //sonu
                    WriteTextFile(logFile, "Error doctor SIteMap:" + ex.Message);
                    if (ex.InnerException != null)
                        WriteTextFile(logFile, "Error doctor SIteMap:" + ex.InnerException.Message);
                    
                    WriteTextFile(logFile, "Error Doctor SIteMap count:"+count);
                    WriteTextFile(logFile, "Error Doctor SIteMap MapIndex:"+mapIndex);

                    //count
                    //    mapIndex
                }
                
            };

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "doctor-sitemap.xml";
                else
                    mapName = "doctor-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
                System.Threading.Tasks.Task.Delay(1000);
            }
            WriteTextFile(logFile, "End Generate Doctor SIteMap"+mapName);
            return mapName;
        }

        //-- Generate pharmacy sitemaps 
        public string GeneratePharmacySiteMap()
        {
            var allPharmacy = _pharmacy.GetAll();
            allPharmacy = allPharmacy.Where(x => x.OrganizationTypeID == 1005);

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
            foreach (var item in allPharmacy)
            {
                if (!string.IsNullOrWhiteSpace(item.NPI))
                {
                    string npi = item.NPI;
                    string pharmacyName = StringReplace(item.OrganisationName.ToString());

                    string url = "https://www.doctyme.com/Profile/Pharmacy/" + Url.Encode(string.Format("{0}", pharmacyName + "-" + item.NPI.ToString()));
                    sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                    count++;

                    if (count == 50000)
                    {
                        if (mapIndex == 0)
                            mapName = "pharmacy-sitemap.xml";
                        else
                            mapName = "pharmacy-sitemap" + mapIndex.ToString() + ".xml";

                        SitemapGenerator sg = new SitemapGenerator();
                        var doc = sg.GenerateSiteMap(sitemapItems);
                        doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                        System.Threading.Tasks.Task.Delay(1000);
                        sitemapItems.Clear();
                        count = 0;
                        mapIndex++;
                    }
                }
            };

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "pharmacy-sitemap.xml";
                else
                    mapName = "pharmacy-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
                System.Threading.Tasks.Task.Delay(1000);
            }


            return mapName;
        }

        //-- Generate facility sitemaps 
        public string GenerateFacilitySiteMap()
        {
            var logFile = GetFileName("ProcessGenerate.txt", Server.MapPath("~/Uploads/"));
            WriteTextFile(logFile, "Generate Facility SIteMap");
            var allFacility = _facility.GetAll();
            allFacility = allFacility.Where(x => x.OrganizationTypeID == 1006);

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
           foreach (var item in allFacility)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.NPI))
                    {
                        string npi = item.NPI;
                        string facilityName = StringReplace(item.OrganisationName.ToString());

                        string url = "https://www.doctyme.com/Profile/Facility/" + facilityName + "-" + item.NPI.ToString();
                        sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                        count++;

                        if (count == 50000)
                        {
                            if (mapIndex == 0)
                                mapName = "facility-sitemap.xml";
                            else
                                mapName = "facility-sitemap" + mapIndex.ToString() + ".xml";

                            SitemapGenerator sg = new SitemapGenerator();
                            var doc = sg.GenerateSiteMap(sitemapItems);
                            doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                            System.Threading.Tasks.Task.Delay(1000);
                            sitemapItems.Clear();
                            count = 0;
                            mapIndex++;
                            WriteTextFile(logFile, "facilty SIteMap" + mapName);//sonu
                        }
                    }
                }


                catch (Exception ex)
                {
                    //sonu
                    WriteTextFile(logFile, "Error FACILITY SIteMap:" + ex.Message);
                    if (ex.InnerException != null)
                        WriteTextFile(logFile, "Error FACILITY SIteMap:" + ex.InnerException.Message);
                    WriteTextFile(logFile, "Error FACILITY SIteMap count:" + count);
                    WriteTextFile(logFile, "Error FACILITY SIteMap MapIndex:" + mapIndex);
                }
            };

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "facility-sitemap.xml";
                else
                    mapName = "facility-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
                System.Threading.Tasks.Task.Delay(1000);
            }
            WriteTextFile(logFile, "End Generate Facility SIteMap"+mapName);
            return mapName;
        }

        //-- Generate senior care sitemaps 
        public string GenerateSeniorCareSiteMap()
        {
            var logFile = GetFileName("ProcessGenerate.txt", Server.MapPath("~/Uploads/"));
            WriteTextFile(logFile, "Generate Seniorcare SIteMap");
            var allSeniorCare = _seniorCare.GetAll();
            allSeniorCare = allSeniorCare.Where(x => x.OrganizationTypeID == 1007);

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
            foreach (var item in allSeniorCare)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.NPI))
                    {
                        string npi = item.NPI;
                        string seniorCareName = StringReplace(item.OrganisationName.ToString());

                        string url = "https://www.doctyme.com/Profile/SeniorCare/" + seniorCareName + "-" + item.NPI.ToString();
                        sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                        count++;

                        if (count == 50000)
                        {
                            if (mapIndex == 0)
                                mapName = "senior-care-sitemap.xml";
                            else
                                mapName = "senior-care-sitemap" + mapIndex.ToString() + ".xml";

                            SitemapGenerator sg = new SitemapGenerator();
                            var doc = sg.GenerateSiteMap(sitemapItems);
                            doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                            sitemapItems.Clear();
                            count = 0;
                            mapIndex++;
                            WriteTextFile(logFile, "seniorCare SIteMap" + mapName);//sonu
                        }
                    }
                }
                catch(Exception ex)
                {
                    //sonu
                    WriteTextFile(logFile, "Error seniorcare SIteMap:" + ex.Message);
                    if (ex.InnerException != null)
                        WriteTextFile(logFile, "Error seniorcare SIteMap:" + ex.InnerException.Message);
                   
                    WriteTextFile(logFile, "Error seniorCare SIteMap count:" + count);
                    WriteTextFile(logFile, "Error seniorCare SIteMap MapIndex:" + mapIndex);
                }
            };
            

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "senior-care-sitemap.xml";
                else
                    mapName = "senior-care-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
            }
            WriteTextFile(logFile, "End Generate Seniorcare SIteMap"+mapName);
            return mapName;
        }

        //-- Generate senior care sitemaps 
        public string GenerateArticleNameSiteMap()
        {
            var logFile = GetFileName("ProcessGenerate.txt", Server.MapPath("~/Uploads/"));
            WriteTextFile(logFile, "Generate Article name SIteMap");
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetAllBlogSiteMap + " ");
            DataTable dt = ds.Tables[0];
            var result = Common.ConvertDataTable<BlogItemSiteMap>(dt);

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
            foreach (var item in result)
            {
                 try
                {
                    if (!string.IsNullOrWhiteSpace(item.ArticleNameReplace))
                    {

                        string articleName = StringReplace(item.ArticleNameReplace);

                        string url = "https://www.doctyme.com/Blog/BlogItem?ArticleName=" + articleName;
                        sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                        count++;

                        if (count == 50000)
                        {
                            if (mapIndex == 0)
                                mapName = "article-name-sitemap.xml";
                            else
                                mapName = "article-name-sitemap" + mapIndex.ToString() + ".xml";

                            SitemapGenerator sg = new SitemapGenerator();
                            var doc = sg.GenerateSiteMap(sitemapItems);
                            doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                            sitemapItems.Clear();
                            count = 0;
                            mapIndex++;
                            WriteTextFile(logFile, "article SIteMap" + mapName);//sonu
                        }
                    }
                }
                catch(Exception ex)
                {
                     WriteTextFile(logFile, "Error article SIteMap:" + ex.Message);
                    if (ex.InnerException != null)
                        WriteTextFile(logFile, "Error article SIteMap:" + ex.InnerException.Message);
                    WriteTextFile(logFile, "Error article SIteMap count:" + count);
                    WriteTextFile(logFile, "Error article SIteMap MapIndex:" + mapIndex);
                }
                
            };

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "article-name-sitemap.xml";
                else
                    mapName = "article-name-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
            }
            WriteTextFile(logFile, "End Generate Article name SIteMap"+mapName);
            return mapName;
        }

        //-- Generate senior care sitemaps 
        public string GenerateDrugNameSiteMap()
        {
            var logFile = GetFileName("ProcessGenerate.txt", Server.MapPath("~/Uploads/"));
            WriteTextFile(logFile, "Generate Drugname SIteMap");
            DataSet ds = _doctor.GetQueryResult(StoredProcedureList.GetDrugNameSiteMap + " ");
            DataTable dt = ds.Tables[0];
            var result = Common.ConvertDataTable<DrugNameSiteMap>(dt);

            var sitemapItems = new List<SitemapItem>();

            int count = 0;
            int mapIndex = 0;
            string mapName = "";
            foreach (var item in result)
            {
               try
                {
                    if (!string.IsNullOrWhiteSpace(item.drugname))
                    {

                        string drugName = StringReplace(item.drugname);
                        string url = "https://www.doctyme.com/SearchDrug/" + drugName;
                        sitemapItems.Add(new SitemapItem(url, changeFrequency: SitemapChangeFrequency.Always, priority: 1.0));
                        count++;

                        if (count == 50000)
                        {
                            if (mapIndex == 0)
                                mapName = "drug-name-sitemap.xml";
                            else
                                mapName = "drug-name-sitemap" + mapIndex.ToString() + ".xml";

                            SitemapGenerator sg = new SitemapGenerator();
                            var doc = sg.GenerateSiteMap(sitemapItems);
                            doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));

                            sitemapItems.Clear();
                            count = 0;
                            mapIndex++;
                            WriteTextFile(logFile, "drug SIteMap" + mapName);//sonu
                        }
                    }
                }
                catch(Exception ex)
                {
                    WriteTextFile(logFile, "Error drug SIteMap:" + ex.Message);
                    if (ex.InnerException != null)
                        WriteTextFile(logFile, "Error drug SIteMap:" + ex.InnerException.Message);
                    WriteTextFile(logFile, "Error drug SIteMap count:" + count);
                    WriteTextFile(logFile, "Error drug SIteMap MapIndex:" + mapIndex);
                }
            };

            if (count > 0)
            {
                if (mapIndex == 0)
                    mapName = "drug-name-sitemap.xml";
                else
                    mapName = "drug-name-sitemap" + mapIndex.ToString() + ".xml";

                SitemapGenerator sg = new SitemapGenerator();
                var doc = sg.GenerateSiteMap(sitemapItems);
                doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), mapName));
                count = 0;
                mapIndex = 0;
            }
            WriteTextFile(logFile, "End Generate Drugname SIteMap"+mapName);
            return mapName;
        }

        public ActionResult FileDownload(string filename)
        {
            String FilePath = Server.MapPath("~/SiteMaps/");

            FilePath = Path.Combine(FilePath, filename);

            return File(FilePath, "application/xml", filename);
        }

        //Added by Reena
        public ActionResult DownloadAllFiles()
        {
            var filesCol = GetFile().ToList();
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }
        }

        //Added by Reena
        public void GenerateSiteMapIndex()
        {
            var sitemapItems = new List<SitemapItem>();

            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SiteMaps");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            foreach (var item in dirInfo.GetFiles())
            {
                if (item.Name.Contains("index"))
                {
                    System.IO.File.Delete(dirInfo.FullName + @"\" + item.Name);
                }
            }

            var filesCol = GetFile().ToList();
            foreach (var item in filesCol)
            {
                sitemapItems.Add(new SitemapItem(item.FileName, lastModified: DateTime.Now));
            }

            string name = "sitemap-index.xml";
            SitemapGenerator sg = new SitemapGenerator();
            var doc = sg.GenerateSIteMapIndex(sitemapItems);
            doc.Save(Path.Combine(Server.MapPath("~/SiteMaps/"), name));
        }

        private void DeleteOlderSitemapFiles()
        {
            // Delete 
            string[] files = Directory.GetFiles(Path.Combine(Server.MapPath("~/SiteMaps/")));

            foreach (string file in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                fi.Delete();
            }
        }

        //Added by Reena
        private List<FileInfo> GetFile()
        {
            List<FileInfo> listFiles = new List<FileInfo>();
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SiteMaps");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfo()
                {
                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return listFiles;
        }

        public void GenerateRobotTextfile() //Added by Reena
        {
            int urlCount = 0;
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SiteMaps");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            if (System.IO.File.Exists(dirInfo.FullName + @"\robot.txt"))
            {
                System.IO.File.Delete(dirInfo.FullName + @"\robot.txt");
            }
            using (StreamWriter sw = System.IO.File.CreateText(dirInfo.FullName + @"\robot.txt"))
            {
                sw.WriteLine("User-agent: *");
                sw.WriteLine("Disallow:");
                sw.WriteLine();
                sw.WriteLine();
                foreach (var item in dirInfo.GetFiles())
                {
                    if (!item.Name.Contains("index") && !item.Name.Contains("robot"))
                    {
                        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                        doc.Load(dirInfo.FullName + @"\" + item.Name);
                        System.Xml.XmlElement root = doc.DocumentElement;
                        System.Xml.XmlNodeList elemList = root.GetElementsByTagName("loc");
                        urlCount = urlCount + elemList.Count;
                        sw.WriteLine(@"Sitemap: https://www.doctyme.com/Sitemaps/{0}", item.Name);
                    }
                }
                sw.WriteLine();
                sw.WriteLine("# Total URL Counts:{0} ", urlCount);
            }
        }
        #endregion
        private static string GetFileName(string filename, string path)
        {
            path += filename;
            //var i = 0;
            while (true)
            {
                if (System.IO.File.Exists(path))
                {
                    //var fpn = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + i + Path.GetExtension(path);
                    //var fpn = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path);

                    //if (!System.IO.File.Exists(fpn))
                    //{
                    //    path = fpn;
                    //    break;
                    //}
                    //i = i + 1;
                    break;
                }
                else
                    break;
            }
            return path;
        }
        private static void WriteTextFile(string fpath, string line)
        {
            try
            {
                if (!System.IO.File.Exists(fpath))
                {
                    using (System.IO.File.Create(fpath))
                    {
                    }
                }
                string[] str = { line };
                System.IO.File.AppendAllLines(fpath, str);
            }
            catch
            {
            }
        }

    }
}
