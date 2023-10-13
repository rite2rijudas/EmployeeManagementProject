using EMPLOYEE_MANAGEMENT.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPLOYEE_MANAGEMENT.Controllers
{
    public class ApplicationController : Controller
    {
       
       
            // GET: Application
            ServiceProvider service = new ServiceProvider();

            public static string userID = null;
            //public static string ClientID = null;
            public static string domain = "";
            public ApplicationController()
            {
                //domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                JObject jfinal = new JObject();
                JArray jrFinal = new JArray();
                JArray jrComp = new JArray();
                if (System.Web.HttpContext.Current.Session["EMP_CODE"] != null)
                {
                    userID = System.Web.HttpContext.Current.Session["EMP_CODE"].ToString();
                }
                if (userID != null)
                {
                    DataTable dtUser = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "EMP_CODE",Value= userID }
                                                    });
                    if (dtUser != null)
                    {
                        if (dtUser.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtUser.Rows[0]["EMPLOYEE_NAME"].ToString()))
                            {

                                jfinal["EmployeeName"] = dtUser.Rows[0]["EMPLOYEE_NAME"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dtUser.Rows[0]["DESIGNATION_NAME"].ToString()))
                            {

                                jfinal["DesgName"] = dtUser.Rows[0]["DESIGNATION_NAME"].ToString();
                            }

                            if (!string.IsNullOrEmpty(dtUser.Rows[0]["PROFILE_IMAGE"].ToString()))
                            {
                                jfinal["EmployeeImage"] = @"\Content\resources\dist\img\" + dtUser.Rows[0]["PROFILE_IMAGE"].ToString();
                            }
                            else
                            {
                                jfinal["EmployeeImage"] = @"\Content\resources\dist\img\" + "no-image.jpeg";

                            }
                            jfinal["stat"] = "1";
                        }
                        else
                        {
                            jfinal["EmployeeName"] = "";
                            jfinal["DesgName"] = "";
                            jfinal["EmployeeImage"] = @"\Content\resources\dist\img\" + "no-image.jpeg";
                            jfinal["stat"] = "1";
                        }
                    }
                    else
                    {
                        jfinal["EmployeeName"] = "";
                        jfinal["DesgName"] = "";
                        jfinal["EmployeeImage"] = @"\Content\resources\dist\img\" + "no-image.jpeg";
                        jfinal["stat"] = "0";
                    }
                }
                else
                {
                    jfinal["EmployeeName"] = "";
                    jfinal["DesgName"] = "";
                    jfinal["EmployeeImage"] = @"\Content\resources\dist\img\" + "no-image.jpeg";
                    jfinal["stat"] = "0";
                }

                ViewData["menus"] = jfinal;

            }
    }
}
