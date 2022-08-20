using Doctyme.Model.ViewModels;
using Doctyme.Repository.Enumerable;
using Doctyme.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Doctyme.Repository.Services
{
    public class TestimonialRepository : ITestimonialsService
    {
        #region Get Testimonial
        public TestimonialItem GetTestimonial(int id)
        {
            TestimonialItem result = null;
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(StoredProcedureList.GetTestimonialById, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TestimonialID", id);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            result = new TestimonialItem {
                                TestimonialID = Convert.ToInt32(reader["TestimonialID"]),
                                Name = reader["Name"].ToString(),
                                Title = reader["Title"].ToString(),
                                Organization = reader["Organization"].ToString(),
                                ImagePath = reader["ImagePath"].ToString(),
                                Content = reader["Content"].ToString(),
                                FaceBookLink = reader["FaceBookLink"].ToString(),
                                Plink = reader["Plink"].ToString(),
                                GooglePlusLink = reader["GooglePlusLink"].ToString(),
                                LinkedLink = reader["LinkedLink"].ToString(),
                                TwitterLink = reader["TwitterLink"].ToString(),
                                InstagramLink = reader["InstagramLink"].ToString(),
                                Keywords = reader["Keywords"].ToString(),
                                MainSite = Convert.ToBoolean(reader["MainSite"].ToString())
                            };
                        }
                    }

                }

                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Save Testimonial
        public int SaveTestimonial(TestimonialItem item)
        {
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnection))
                {
                    using (SqlCommand command = new SqlCommand(StoredProcedureList.InsertTestimonialItem, conn))
                    {
                        item.ImagePath = item.ImagePath ?? "";

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TestimonialID", item.TestimonialID);
                        command.Parameters.AddWithValue("@Name", item.Name);
                        command.Parameters.AddWithValue("@Title", item.Title);
                        command.Parameters.AddWithValue("@Organization", item.Organization);
                        command.Parameters.AddWithValue("@ImagePath", item.ImagePath);
                        command.Parameters.AddWithValue("@Content", item.Content);
                        command.Parameters.AddWithValue("@FaceBookLink", item.FaceBookLink);
                        command.Parameters.AddWithValue("@Plink", item.Plink);
                        command.Parameters.AddWithValue("@GooglePlusLink", item.GooglePlusLink);
                        command.Parameters.AddWithValue("@LinkedLink", item.LinkedLink);
                        command.Parameters.AddWithValue("@TwitterLink", item.TwitterLink);
                        command.Parameters.AddWithValue("@InstagramLink", item.InstagramLink);
                        command.Parameters.AddWithValue("@Keywords", item.Keywords);
                        command.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        command.Parameters.AddWithValue("@MainSite", item.MainSite);
                        conn.Open();
                        int result =Convert.ToInt32(command.ExecuteScalar());
                        return result;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Testimonials for index page
        public TestimonialSearchResults GetTestimonialsForIndex(string SearchString, int PageIndex, int PageSize, string Sort)
        {
            TestimonialSearchResults response = new TestimonialSearchResults();
            List<TestimonialItem> resultlist = new List<TestimonialItem>();
            TestimonialItem result = null;
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(StoredProcedureList.GetAllTestimonials, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Search", SearchString);
                        cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", PageSize);
                        cmd.Parameters.AddWithValue("@Sort", Sort);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            result = new TestimonialItem
                            {
                                TestimonialID = Convert.ToInt32(reader["TestimonialID"]),
                                Name = reader["Name"].ToString(),
                                Title = reader["Title"].ToString(),
                                Organization = reader["Organization"].ToString(),
                                ImagePath = string.IsNullOrEmpty(reader["ImagePath"].ToString()) ?"--": reader["ImagePath"].ToString(),
                                Content = reader["Content"].ToString(),
                                FaceBookLink = reader["FaceBookLink"].ToString(),
                                Plink = reader["Plink"].ToString(),
                                GooglePlusLink = reader["GooglePlusLink"].ToString(),
                                LinkedLink = reader["LinkedLink"].ToString(),
                                TwitterLink = reader["TwitterLink"].ToString(),
                                InstagramLink = reader["InstagramLink"].ToString(),
                                IsActive =Convert.ToBoolean(reader["IsActive"]),
                                MainSite= Convert.ToBoolean(reader["MainSite"])
                            };
                            response.TotalRecordCount = Convert.ToInt32(reader["TotalRecordCount"]);
                            resultlist.Add(result);
                        }
                    }

                }
                response.TestimonialItems = resultlist;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Testimonial active and delete status Update 
        public int TestimonialStatusUpdate(StatusUpdateModel item)
        {
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnection))
                {
                    using (SqlCommand command = new SqlCommand(StoredProcedureList.TestimonialItemUpdateStatus, conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TestimonialID", item.Id);
                        command.Parameters.AddWithValue("@Flag", item.Flag);
                        command.Parameters.AddWithValue("@IsDelete", item.IsDeleted);

                        conn.Open();
                        int result = Convert.ToInt32(command.ExecuteScalar());
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Get Testimonials for home
        public List<TestimonialsForHome> GetTestiMonialsForHome()
        {
            
            List<TestimonialsForHome> resultlist = new List<TestimonialsForHome>();
            TestimonialsForHome result = null;
            string strConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Doctyme"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(StoredProcedureList.TestimonialItemsForHome, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            result = new TestimonialsForHome
                            {
                                TestimonialId = Convert.ToInt32(reader["TestimonialID"]),
                                ClientName = reader["Name"].ToString(),
                                Title = reader["Title"].ToString(),
                                ImagePath = reader["ImagePath"].ToString(),
                                Content = reader["Content"].ToString(),
                            };
                            resultlist.Add(result);
                        }
                    }

                }

                return resultlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }


}
