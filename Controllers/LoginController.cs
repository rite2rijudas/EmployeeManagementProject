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
    public class LoginController : ApplicationController
    {
        // GET: Login
        ServiceProvider service = new ServiceProvider();
        List<GeneralIOModel> iolist = new List<GeneralIOModel>();

        public List<SelectListItem> GetFYSessions(DataTable dt)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["IS_CURRENT_FY"].ToString().ToUpper() == "YES")
                {

                    list.Add(new SelectListItem()
                    {
                        Selected = true,
                        Text = dr["SESSION_NAME"].ToString(),
                        Value = dr["SESSION_ID"].ToString()
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {

                        Text = dr["SESSION_NAME"].ToString(),
                        Value = dr["SESSION_ID"].ToString()
                    });

                }


            }

            return list;
        }

        public ActionResult LogOut()
        {

            Session["EMP_CODE"] = null;
            Session["SESSION_CODE"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Login(AccountDetails account, string Submit)
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
            }
            DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBl_FY_SESSION", "STORE_PROCEDURE", new List<GeneralIOModel>());
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    ViewBag.FYList = GetFYSessions(dt);
                }
            }

            if (!string.IsNullOrEmpty(Submit))
            {
                if (Submit == "Login")
                {
                    if (!string.IsNullOrEmpty(account.Login) && !string.IsNullOrEmpty(account.Password))
                    {
                        account.Password = new CustomeDataProvider().Encryption(account.Password);

                        DataTable empvalidate = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_LOGIN_LOGIN_ID_PASSWORD", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                            new GeneralIOModel { Name= "LOGIN_ID",Value=account.Login },
                            new GeneralIOModel { Name="PASSWORD",Value=account.Password}
                        });

                        account.Password = new CustomeDataProvider().Decryption(account.Password);

                        if (empvalidate != null)
                        {
                            if (empvalidate.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(empvalidate.Rows[0]["LOG_STATUS"].ToString()))
                                {

                                    if (empvalidate.Rows[0]["LOG_STATUS"].ToString() == "SUCCESS")
                                    {
                                        Session["EMP_CODE"] = empvalidate.Rows[0]["EMP_CODE"].ToString();
                                        Session["SESSION_CODE"] = account.FYSessionCode;

                                        if (!string.IsNullOrEmpty(empvalidate.Rows[0]["RESET_REQUIRED"].ToString()))
                                        {
                                            if (empvalidate.Rows[0]["RESET_REQUIRED"].ToString().ToUpper() == "YES")
                                            {
                                                if (!string.IsNullOrEmpty(Session["EMP_CODE"].ToString()))
                                                {
                                                    DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "EMP_CODE",Value= Session["EMP_CODE"].ToString() }
                                                    });

                                                    if (dt1 != null)
                                                    {
                                                        if (dt1.Rows.Count > 0)
                                                        {
                                                            ViewBag.UserImage = dt1.Rows[0]["PROFILE_IMAGE"].ToString();
                                                            ViewBag.UserName = dt1.Rows[0]["EMPLOYEE_NAME"].ToString();
                                                        }
                                                    }
                                                }

                                                account = new AccountDetails();
                                                return View("password_reset", account);
                                            }
                                        }

                                        return RedirectToAction("EmployeeProfile", "Employee");
                                    }
                                    else
                                    {
                                        TempData["ValidationMessage"] = empvalidate.Rows[0]["LOG_STATUS"].ToString();

                                        return View("Login", account);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["ValidationMessage"] = "The login fields are required";

                        return View("Login", account);
                    }
                }
            }





            return View(account);
        }



        public JsonResult EmpCodeValidateForForgetpassword(string EmpCode)
        {

            JObject jobj = new JObject();

            if (!string.IsNullOrEmpty(EmpCode))
            {

                DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_LOGIN_LOGIN_ID", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "LOGIN_ID",Value= EmpCode }
                                                    });
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["LOG_STATUS"].ToString()))
                        {
                            if (dt.Rows[0]["LOG_STATUS"].ToString() == "SUCCESS")
                            {
                                DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "SELECT_TBL_CONFIGURATION_SETTINGS_BY_RECORD_ID", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "TD_RECORD_ID",Value= "R1" }
                                                    });
                                if (dt1 != null)
                                {
                                    if (dt1.Rows.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(dt1.Rows[0]["DESC_1"].ToString()))
                                        {

                                            DataTable dt2 = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "BPSCL_CODE",Value= EmpCode }
                                                    });
                                            if (dt2 != null)
                                            {
                                                if (dt2.Rows.Count > 0)
                                                {
                                                    jobj["CONTACT_NO"] = dt2.Rows[0]["CONTACT_NO"].ToString();
                                                    jobj["EMAIL"] = dt2.Rows[0]["EMAIL_ADDRESS"].ToString();
                                                }
                                            }

                                            jobj["DEVELOPER MODE"] = dt1.Rows[0]["DESC_1"].ToString();
                                            jobj["stat"] = "1";
                                        }
                                        else
                                        {
                                            jobj["msg"] = "could not find developer mode";
                                            jobj["stat"] = "0";
                                        }
                                    }
                                    else
                                    {
                                        jobj["stat"] = "0";
                                    }
                                }
                                else
                                {
                                    jobj["stat"] = "0";
                                }


                            }
                            else
                            {
                                jobj["msg"] = dt.Rows[0]["LOG_STATUS"].ToString();
                                jobj["stat"] = "0";
                            }

                        }
                    }
                }
            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Emp Code is missing";
            }



            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));

        }

        public JsonResult SendOtpToUser(string BPSCL_CODE, string Otp)
        {

            JObject jobj = new JObject();

            if (!string.IsNullOrEmpty(BPSCL_CODE))
            {
                if (!string.IsNullOrEmpty(Otp))
                {

                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "BPSCL_CODE",Value= BPSCL_CODE }
                                                    });
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            string username = " " + dt.Rows[0]["EMPLOYEE_NAME"].ToString();
                            string email = dt.Rows[0]["EMAIL_ADDRESS"].ToString();

                            // string username = "Riju Das";
                            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                            {
                                if (new CustomeDataProvider().SendEmail("", email, "OTP FOR CHANGE PASSWORD", "YOUR OTP -" + Otp))
                                {
                                    jobj["stat"] = "1";
                                }
                                else
                                {
                                    jobj["stat"] = "0";
                                    jobj["msg"] = "Oops ! Failed to Send OTP, contact Administrator";
                                }
                            }

                        }
                        else
                        {
                            jobj["stat"] = "0";
                            jobj["msg"] = "user data not available";
                        }
                    }
                    else
                    {
                        jobj["stat"] = "0";
                        jobj["msg"] = "user data not available";
                    }



                }
                else
                {
                    jobj["stat"] = "0";
                    jobj["msg"] = "Otp is missing";
                }
            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Emp Code is missing";
            }



            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));


        }

        public JsonResult UpdatePassword(string BPSCL_CODE, string Password)
        {

            JObject jobj = new JObject();

            if (!string.IsNullOrEmpty(BPSCL_CODE))
            {
                if (!string.IsNullOrEmpty(Password))
                {
                    string EmpCode = "";
                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                        new GeneralIOModel { Name = "BPSCL_CODE", Value = BPSCL_CODE }
                    });
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            EmpCode = dt.Rows[0]["EMP_CODE"].ToString();
                            Password = new CustomeDataProvider().Encryption(Password);

                            DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "UPDATE_TBL_LOGIN_PASSWORD_BY_EMP_CODE_RESET_PASSWORD", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "EMP_CODE",Value= EmpCode } ,
                                                        new GeneralIOModel { Name= "PASSWORD",Value= Password }
                                                    });
                            if (dt1 != null)
                            {
                                if (dt1.Rows.Count > 0)
                                {
                                    if (dt1.Rows[0]["LOG_STATUS"].ToString() == "SUCCESS")
                                    {
                                        jobj["stat"] = "1";
                                        jobj["msg"] = "Your Password is successfully updated. Please use this new password for Login";
                                    }
                                    else
                                    {
                                        jobj["stat"] = "0";
                                        jobj["msg"] = dt1.Rows[0]["LOG_STATUS"].ToString();
                                    }
                                }
                            }

                        }
                    }


                }
                else
                {
                    jobj["stat"] = "0";
                    jobj["msg"] = "Password is missing";
                }
            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Emp Code is missing";
            }



            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));

        }


        public ActionResult ResetPasswordAtFirstLogin(AccountDetails model)
        {
            DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBl_FY_SESSION", "STORE_PROCEDURE", new List<GeneralIOModel>());
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    ViewBag.FYList = GetFYSessions(dt);
                }
            }
            if (!string.IsNullOrEmpty(Session["EMP_CODE"].ToString()))
            {
                DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "EMP_CODE",Value= Session["EMP_CODE"].ToString() }
                                                    });
                if (dt1 != null)
                {
                    if (dt1.Rows.Count > 0)
                    {
                        ViewBag.UserImage = dt1.Rows[0]["PROFILE_IMAGE"].ToString();
                        ViewBag.UserName = dt1.Rows[0]["EMPLOYEE_NAME"].ToString();
                    }
                }


                if (!string.IsNullOrEmpty(model.Newpassword) && !string.IsNullOrEmpty(model.ConfirmedPassword))
                {
                    if (model.Newpassword == model.ConfirmedPassword)
                    {
                        model.Newpassword = new CustomeDataProvider().Encryption(model.Newpassword);
                        DataTable dt2 = (DataTable)service.GeneralOperation("DATA", "UPDATE_TBL_LOGIN_PASSWORD_BY_EMP_CODE_RESET_PASSWORD", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "EMP_CODE",Value= Convert.ToString(Session["EMP_CODE"]) },
                                                        new GeneralIOModel { Name= "PASSWORD",Value= model.Newpassword }
                                                    });

                        if (dt2 != null)
                        {
                            if (dt2.Rows.Count > 0)
                            {
                                if (dt2.Rows[0]["LOG_STATUS"].ToString() == "SUCCESS")
                                {
                                    model.Newpassword = new CustomeDataProvider().Decryption(model.Newpassword);
                                    TempData["passwordValidationsuccessmessage"] = "Your Password is successfully updated. Please use this new password for Login";
                                    model = new AccountDetails();
                                    return View("Login", model);
                                }
                                else
                                {
                                    model.Newpassword = new CustomeDataProvider().Decryption(model.Newpassword);
                                    TempData["passwordValidationmessage"] = dt2.Rows[0]["LOG_STATUS"].ToString();
                                    return View("password_reset", model);
                                }
                            }
                        }

                        TempData["passwordValidationmessage"] = "Oops! Failed to change password. Some Error Occurred 🙁";
                        return View("password_reset", model);

                    }
                    else
                    {
                        TempData["passwordValidationmessage"] = "Passwords does not matched";
                        return View("password_reset", model);
                    }
                }
                else
                {
                    TempData["passwordValidationmessage"] = "Please fill new password and confirm password field properly";
                    return View("password_reset", model);
                }
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }

        }
    }
}