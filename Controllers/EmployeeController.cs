using EMPLOYEE_MANAGEMENT.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EMPLOYEE_MANAGEMENT.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        ServiceProvider service = new ServiceProvider();
        #region functions


        public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        public List<SelectListItem> BindDropdown(string mastercode)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_FROM_TBL_MASTER_VALUE_ACTIVE_MASTER_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>()
            {
                new GeneralIOModel{Name="MASTER_CODE",Value=mastercode }
            });

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new SelectListItem()
                        {

                            Text = dr["RECORD_NAME"].ToString(),
                            Value = dr["RECORD_ID"].ToString()
                        });
                    }
                }
            }

            if (mastercode == "M3")
            {

                return list.OrderBy(v => PadNumbers(v.Text)).ToList();

            }
            else
            {
                return list;
            }


        }
        public List<SelectListItem> BindUserGroups()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Choose Groups", Value = "" });

            DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_WORKFLOW_GROUPS", "STORE_PROCEDURE", new List<GeneralIOModel>());
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new SelectListItem()
                        {
                            Text = dr["GROUP_DESC"].ToString(),
                            Value = dr["GROUP_CODE"].ToString()
                        });
                    }
                }
            }


            return list;
        }


        public List<EmployeeDetails> GetEmployeeList(DataTable dt)
        {
            List<EmployeeDetails> lst = new List<EmployeeDetails>();

            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new EmployeeDetails()
                {
                    EmployeeName = dr["EMPLOYEE_NAME"].ToString(),
                    EmpCode = dr["BPSCL_CODE"].ToString(),
                    DOB = Convert.ToDateTime(dr["DOB"]).ToString("dd/MM/yyyy"),
                    Department = dr["DEPARTMENT_NAME"].ToString(),
                    AdditionalDepartment = dr["ADDITIONAL_DEPARTMENT_VAL"].ToString(),
                    Designation = dr["DESIGNATION_NAME"].ToString(),
                    ContactNo = dr["CONTACT_NO"].ToString(),
                    EmailAddress = dr["EMAIL_ADDRESS"].ToString(),
                    MembersOf = dr["MEMBER_OF_VAL"].ToString(),
                    RealEmpCode = new CustomeDataProvider().Encryption(dr["EMP_CODE"].ToString())
                });
            }

            return lst;
        }

        #endregion



        #region employee profile
        public ActionResult EmployeeProfile(EmployeeDetails model)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                    new GeneralIOModel { Name = "EMP_CODE", Value = Session["EMP_CODE"].ToString() }
                });

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        model.EmployeeName = dt.Rows[0]["EMPLOYEE_NAME"].ToString();
                        model.EmpCode = dt.Rows[0]["BPSCL_CODE"].ToString();
                        model.MembersOf = dt.Rows[0]["MEMBER_OF_VAL"].ToString();
                        model.EmailAddress = dt.Rows[0]["EMAIL_ADDRESS"].ToString();
                        model.AdditionalDepartment = dt.Rows[0]["ADDITIONAL_DEPARTMENT_VAL"].ToString();
                        model.ContactNo = dt.Rows[0]["CONTACT_NO"].ToString();
                        model.Department = dt.Rows[0]["DEPARTMENT_NAME"].ToString();
                        model.Designation = dt.Rows[0]["DESIGNATION_NAME"].ToString();
                        model.Grade = dt.Rows[0]["EMP_GRADE"].ToString();
                        model.LastLoginTime = Convert.ToDateTime(dt.Rows[0]["LAST_LOGIN_TIME"]).ToString("hh:mm tt");
                        model.DOB = Convert.ToDateTime(dt.Rows[0]["DOB"]).ToString("dd/MM/yyyy");
                        model.ProfileImageName = Domain + @"\Content\resources\dist\img\" + dt.Rows[0]["PROFILE_IMAGE"].ToString();
                        model.SignatureName = Domain + @"\Content\resources\dist\img\" + dt.Rows[0]["SIGNATURE"].ToString();
                    }
                }


                return View(model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public ActionResult FileSave()
        {
            JObject jo = new JObject();
            //upload work
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {

                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    string Document = files.AllKeys[0].ToString();
                    HttpPostedFileBase file = files[0];



                    var fileName = Path.GetFileName(file.FileName);

                    if (!System.IO.Directory.Exists(Server.MapPath(@"\Content\resources\dist\img\")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath(@"\Content\resources\dist\img\"));
                    }

                    var path = Path.Combine(Server.MapPath(@"\Content\resources\dist\img\"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (Document == "ProfileImage")
                    {
                        var imageDimention = System.Drawing.Image.FromStream(file.InputStream);

                        if (imageDimention.Height == imageDimention.Width)
                        {
                            if ((bool)service.GeneralOperation("STATUS", "UPDATE_INTO_TBL_EMPLOYEE_PROFILE_IMAGE_UPLOAD", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="EMP_CODE",Value=Session["EMP_CODE"].ToString()} ,
                                new GeneralIOModel{Name="IMAGE",Value=fileName}
                            }))
                            {
                                file.SaveAs(path);
                                jo["status"] = "1";
                                jo["msg"] = Document + " Successfully Updated";
                            }
                            else
                            {
                                jo["status"] = "0";
                                jo["msg"] = Document + " not updated something went wrong";
                            }
                        }
                        else
                        {
                            jo["status"] = "0";
                            jo["msg"] = Document + " ratio should be 1:1";
                        }



                    }
                    else if (Document == "Signature")
                    {
                        // if (service.UPDATE_INTO_TBL_EMPLOYEE_SIGNATURE_UPLOAD(Session["EMP_CODE"].ToString(), fileName))
                        if ((bool)service.GeneralOperation("STATUS", "UPDATE_INTO_TBL_EMPLOYEE_SIGNATURE_UPLOAD", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="EMP_CODE",Value=Session["EMP_CODE"].ToString()} ,
                                new GeneralIOModel{Name="IMAGE",Value=fileName}
                            }))
                        {
                            file.SaveAs(path);
                            jo["status"] = "1";
                            jo["msg"] = Document + " Successfully Updated";
                        }
                        else
                        {
                            jo["status"] = "0";
                            jo["msg"] = Document + " not updated something went wrong";
                        }
                    }


                }
                catch (Exception ex)
                {
                    jo["status"] = "0";
                    jo["msg"] = ex.Message.ToString();
                }


            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }




            return Json(jo.ToString(Newtonsoft.Json.Formatting.None));
        }

        public JsonResult ChangeOldPassword(string NewPassword, string OldPassword)
        {

            JObject jobj = new JObject();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {


                if (!string.IsNullOrEmpty(NewPassword))
                {
                    if (!string.IsNullOrEmpty(OldPassword))
                    {


                        OldPassword = new CustomeDataProvider().Encryption(OldPassword);
                        NewPassword = new CustomeDataProvider().Encryption(NewPassword);
                        //DataTable dt = service.CHANGE_PASSWORD_USING_OLD_PASSWORD(Session["EMP_CODE"].ToString(), OldPassword, NewPassword);
                        DataTable dt = (DataTable)service.GeneralOperation("DATA", "CHANGE_PASSWORD_USING_OLD_PASSWORD", "STORE_PROCEDURE", new List<GeneralIOModel>() {

                            new GeneralIOModel{Name="EMP_CODE",Value=Session["EMP_CODE"].ToString()},
                            new GeneralIOModel{Name="CurrentPassword",Value=OldPassword},
                            new GeneralIOModel{Name="NewPassword",Value=NewPassword}

                        });
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dt.Rows[0]["LOG_STATUS"].ToString()))
                                {
                                    if (dt.Rows[0]["LOG_STATUS"].ToString().ToUpper() == "SUCCESS")//////
                                    {
                                        jobj["stat"] = "1";
                                        jobj["msg"] = "Your Password is successfully updated. Please use this new password for Login";
                                    }
                                    else
                                    {
                                        jobj["stat"] = "0";
                                        //jobj["msg"] = "Oops ! Failed to change password. Some Error Occurred 🙁";
                                        jobj["msg"] = dt.Rows[0]["LOG_STATUS"].ToString();
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        jobj["stat"] = "0";
                        jobj["msg"] = "old Password is missing";
                    }
                }
                else
                {
                    jobj["stat"] = "0";
                    jobj["msg"] = "new Password is missing";
                }

            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Logged out please login";
            }

            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));


        }
        #endregion

        #region employee master
        public ActionResult EmployeeMaster(EmployeeDetails model, string Submit)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                ViewBag.Departments = BindDropdown("M1");
                ViewBag.Designations = BindDropdown("M2");
                ViewBag.Grades = BindDropdown("M3");
                ViewBag.UserGroups = BindUserGroups();


                if (!string.IsNullOrEmpty(Submit))
                {
                    if (Submit == "Save")
                    {
                        //int flag = 0;
                        DateTime d;
                        DateTime dob = DateTime.MinValue;
                        if (DateTime.TryParse(model.DOB, out d))
                        {
                            dob = d;
                        }

                        model.MembersOf = (string.IsNullOrEmpty(model.MembersOf)) ? "" : model.MembersOf;
                        model.AdditionalDepartment = (string.IsNullOrEmpty(model.HdnAdditionalDepartment)) ? "" : model.HdnAdditionalDepartment;
                        model.Designation = (string.IsNullOrEmpty(model.Designation)) ? "" : model.Designation;
                        string password = new CustomeDataProvider().Encryption("Jup1ter1");

                        if (!string.IsNullOrEmpty(model.RealEmpCode))
                        {

                            DataTable existingdata = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                new GeneralIOModel { Name = "EMP_CODE", Value = model.RealEmpCode }
                            });
                            if (existingdata != null)
                            {
                                if (existingdata.Rows.Count > 0)
                                {
                                    model.SignatureName = existingdata.Rows[0]["SIGNATURE"].ToString();
                                    model.ProfileImageName = existingdata.Rows[0]["PROFILE_IMAGE"].ToString();
                                }
                            }


                            //for updating existing employee
                            // if (service.INSERT_INTO_TBL_EMPLOYEE(model.RealEmpCode, model.EmpCode, model.EmployeeName,
                            // dob, model.Grade, model.Department, model.AdditionalDepartment, model.Designation,
                            // model.ProfileImageName, model.ContactNo, model.EmailAddress, Convert.ToString(Session["EMP_CODE"]),
                            // model.Gender, model.SignatureName, model.MembersOf, password))
                            if ((bool)service.GeneralOperation("STATUS", "INSERT_INTO_TBL_EMPLOYEE", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="EMP_CODE",Value=model.RealEmpCode },
                                new GeneralIOModel{Name="BPSCL_CODE",Value=model.EmpCode },
                                new GeneralIOModel{Name="EMPLOYEE_NAME",Value=model.EmployeeName },
                                new GeneralIOModel{Name="DOB",Value=dob },
                                new GeneralIOModel{Name="EMP_GRADE",Value=model.Grade },
                                new GeneralIOModel{Name="DEPARTMENT",Value=model.Department},
                                new GeneralIOModel{Name="ADDITIONAL_DEPARTMENT",Value=model.AdditionalDepartment },
                                new GeneralIOModel{Name="DESIGNATION",Value=model.Designation },
                                new GeneralIOModel{Name="PROFILE_IMAGE",Value=model.ProfileImageName },
                                new GeneralIOModel{Name="CONTACT_NO",Value=model.ContactNo },
                                new GeneralIOModel{Name="EMAIL_ADDRESS",Value=model.EmailAddress },
                                new GeneralIOModel{Name="ENTRY_USER",Value= Convert.ToString(Session["EMP_CODE"])},
                                new GeneralIOModel{Name="GENDER",Value=model.Gender },
                                new GeneralIOModel{Name="SIGNATURE",Value=model.SignatureName },
                                new GeneralIOModel{Name="MEMBER_OF",Value=model.MembersOf },
                                new GeneralIOModel{Name="PASSWORD",Value=password },
                            }))
                            {
                                ModelState.Clear();
                                model = new EmployeeDetails();
                                TempData["successmsg"] = "Data Updated Successfully";
                                return RedirectToAction("EmployeeView");
                            }
                            else
                            {
                                TempData["errormsg"] = "Data not Updated something went wrong!!";
                                return View(model);
                            }
                        }
                        else
                        {
                            //for inserting new employee
                            //  if (service.INSERT_INTO_TBL_EMPLOYEE("", model.EmpCode, model.EmployeeName, dob, model.Grade,
                            //  model.Department, model.AdditionalDepartment, model.Designation, "no-image.jpeg", model.ContactNo,
                            //  model.EmailAddress, Convert.ToString(Session["EMP_CODE"]), model.Gender, "", model.MembersOf, password))
                            if ((bool)service.GeneralOperation("STATUS", "INSERT_INTO_TBL_EMPLOYEE", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="EMP_CODE",Value="" },
                                new GeneralIOModel{Name="BPSCL_CODE",Value=model.EmpCode },
                                new GeneralIOModel{Name="EMPLOYEE_NAME",Value=model.EmployeeName },
                                new GeneralIOModel{Name="DOB",Value=dob },
                                new GeneralIOModel{Name="EMP_GRADE",Value=model.Grade },
                                new GeneralIOModel{Name="DEPARTMENT",Value=model.Department},
                                new GeneralIOModel{Name="ADDITIONAL_DEPARTMENT",Value=model.AdditionalDepartment },
                                new GeneralIOModel{Name="DESIGNATION",Value=model.Designation },
                                new GeneralIOModel{Name="PROFILE_IMAGE",Value="no-image.jpeg" },
                                new GeneralIOModel{Name="CONTACT_NO",Value=model.ContactNo },
                                new GeneralIOModel{Name="EMAIL_ADDRESS",Value=model.EmailAddress },
                                new GeneralIOModel{Name="ENTRY_USER",Value= Convert.ToString(Session["EMP_CODE"])},
                                new GeneralIOModel{Name="GENDER",Value=model.Gender },
                                new GeneralIOModel{Name="SIGNATURE",Value="" },
                                new GeneralIOModel{Name="MEMBER_OF",Value=model.MembersOf },
                                new GeneralIOModel{Name="PASSWORD",Value=password },
                            }))
                            {
                                ModelState.Clear();
                                model = new EmployeeDetails();
                                TempData["successmsg"] = "Data Saved Successfully";
                            }
                            else
                            {
                                TempData["errormsg"] = "Data not Saved something went wrong!!";
                                return View(model);
                            }
                        }


                    }
                }

                // model = new BPSCL_EmployeeDetails();
                return View(model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }

        }

        public JsonResult ValidateFieldValue(string FieldName, string Value, string RealEmpCode)
        {

            JObject jobj = new JObject();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {


                if (!string.IsNullOrEmpty(FieldName))
                {
                    if (!string.IsNullOrEmpty(Value))
                    {



                        DataTable dt = new DataTable();

                        if (!string.IsNullOrEmpty(Value))
                        {
                            if (!string.IsNullOrEmpty(RealEmpCode))
                            {
                                DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                    new GeneralIOModel { Name = "EMP_CODE", Value = RealEmpCode }
                                });
                                string bpsclcode = "";
                                string contactno = "";
                                string emailid = "";
                                if (dt1 != null)
                                {
                                    if (dt1.Rows.Count > 0)
                                    {

                                        bpsclcode = dt1.Rows[0]["BPSCL_CODE"].ToString();
                                        contactno = dt1.Rows[0]["CONTACT_NO"].ToString();
                                        emailid = dt1.Rows[0]["EMAIL_ADDRESS"].ToString();
                                    }

                                }


                                if (FieldName == "EmpCode")
                                {
                                    if (bpsclcode != Value.Replace(" ", ""))
                                    {
                                        //dt = service.SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE(Value);

                                        dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "BPSCL_CODE",Value= Value }
                                                    });
                                    }
                                }
                                else if (FieldName == "ContactNo")
                                {
                                    if (contactno != Value.Replace(" ", ""))
                                    {

                                        //dt = service.SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_CONTACT_NO(Value);

                                        dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_CONTACT_NO", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                            new GeneralIOModel { Name = "CONTACT_NO", Value = Value }
                                        });
                                    }
                                }
                                else if (FieldName == "EmailId")
                                {
                                    if (emailid != Value.Replace(" ", ""))
                                    {
                                        // dt = service.SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMAIL_ID(Value);
                                        dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMAIL_ID", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                            new GeneralIOModel { Name = "EMAIL_ID", Value = Value }
                                        });
                                    }
                                }

                            }
                            else
                            {
                                if (FieldName == "EmpCode")
                                {

                                    dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_BPSCL_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                                        new GeneralIOModel { Name= "BPSCL_CODE",Value= Value }
                                                    });
                                }
                                else if (FieldName == "ContactNo")
                                {
                                    dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_CONTACT_NO", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                            new GeneralIOModel { Name = "CONTACT_NO", Value = Value }
                                        });


                                }
                                else if (FieldName == "EmailId")
                                {

                                    dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMAIL_ID", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                            new GeneralIOModel { Name = "EMAIL_ID", Value = Value }
                                        });
                                }


                            }

                            if (dt != null)
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    jobj["stat"] = "1";
                                    //Already exists

                                }
                                else
                                {
                                    //not exists
                                    jobj["stat"] = "0";
                                }
                            }
                        }


                    }
                    else
                    {
                        jobj["stat"] = "2";
                        jobj["msg"] = FieldName + " is missing";
                    }
                }
                else
                {
                    jobj["stat"] = "2";

                }

            }
            else
            {
                jobj["stat"] = "2";
                jobj["msg"] = "Logged out please login";
            }

            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));


        }

        public JsonResult BindAdditionalDepartment(string DeptName, string DeptCode)
        {
            JObject jobj = new JObject();
            List<SelectListItem> additionaldepartmentlist = BindDropdown("M1");
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {

                if (!string.IsNullOrEmpty(DeptCode))
                {

                    if (additionaldepartmentlist.Count > 0)
                    {
                        SelectListItem itm = additionaldepartmentlist.Find(v => v.Value == DeptCode && v.Text == DeptName);

                        additionaldepartmentlist.Remove(itm);
                    }

                }
                else
                {
                    jobj["stat"] = "0";
                    jobj["msg"] = "Department is missing";
                }


            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Logged out please login";
            }

            return Json(additionaldepartmentlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DepartmentHodBind(string DeptCode)
        {
            JObject jobj = new JObject();
            if (!string.IsNullOrEmpty(Session["EMP_CODE"].ToString()))
            {


                if (!string.IsNullOrEmpty(DeptCode))
                {
                    //  DataTable dt = service.SELECT_BPSCL_CODE_EMP_CODE_PROFILE_IMAGE_TBL_EMPLOYEE_BY_DEPARTMENT(DeptCode);
                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_BPSCL_CODE_EMP_CODE_PROFILE_IMAGE_TBL_EMPLOYEE_BY_DEPARTMENT", "STORE_PROCEDURE", new List<GeneralIOModel>()
                    {
                        new GeneralIOModel{Name="DEPARTMENT",Value=DeptCode}
                    });
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            jobj["EMPLOYEE_NAME"] = "HOD - " + dt.Rows[0]["EMPLOYEE_NAME"].ToString();
                            jobj["stat"] = "1";
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
                    jobj["stat"] = "0";
                    jobj["msg"] = "Department is missing";
                }
            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Logged out please login";
            }

            return Json(jobj.ToString(Newtonsoft.Json.Formatting.None));


        }


        /*if any department has no hod>then only show(hod intender dept,hod planning)
        [if in user group(table ) choose intender or planning ic then>(remove hod intender and hod planning from dropdown)same opposit respect to indenter an planning ic]
        only after selection of hod indenting department the hod planning option will show */
        public JsonResult UserGroupBind(string HodFlag, string[] SelectedValues)
        {
            JObject jobj = new JObject();
            List<SelectListItem> userGrouplist = BindUserGroups();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                if (SelectedValues != null)
                {
                    if (SelectedValues.Length > 0)
                    {
                        foreach (var item in SelectedValues)
                        {
                            SelectListItem itm = userGrouplist.Find(v => v.Value == item);
                            userGrouplist.Remove(itm);

                        }
                    }
                }


                if (!string.IsNullOrEmpty(HodFlag))
                {
                    bool hodinplrmv = false;
                    bool intplicrmv = false;
                    bool hidehodplanning = true;

                    if (HodFlag == "1")
                    {
                        SelectListItem itm1 = userGrouplist.Find(v => v.Value == "G2");//hod intend
                        userGrouplist.Remove(itm1);
                        SelectListItem itm2 = userGrouplist.Find(v => v.Value == "G3");//hod planing
                        userGrouplist.Remove(itm2);



                    }
                    else if (HodFlag == "0")
                    {
                        if (SelectedValues != null)
                        {
                            if (SelectedValues.Length > 0)
                            {
                                foreach (var item in SelectedValues)
                                {
                                    if (item == "G1" || item == "G4")
                                    {
                                        hodinplrmv = true;
                                    }


                                    if (item == "G2")
                                    {
                                        intplicrmv = true;
                                        hidehodplanning = false;
                                    }
                                    if (item == "G3")
                                    {
                                        intplicrmv = true;
                                    }

                                }
                            }
                            else
                            {
                                hidehodplanning = true;
                            }


                        }
                        else
                        {
                            hidehodplanning = true;
                        }




                    }

                    if (hodinplrmv)
                    {
                        SelectListItem itm1 = userGrouplist.Find(v => v.Value == "G2");
                        userGrouplist.Remove(itm1);
                        SelectListItem itm2 = userGrouplist.Find(v => v.Value == "G3");
                        userGrouplist.Remove(itm2);
                    }
                    else if (intplicrmv)
                    {
                        SelectListItem itm1 = userGrouplist.Find(v => v.Value == "G1");//indenter
                        userGrouplist.Remove(itm1);
                        SelectListItem itm2 = userGrouplist.Find(v => v.Value == "G4");//planning ic
                        userGrouplist.Remove(itm2);


                    }
                    if (hidehodplanning)
                    {
                        SelectListItem itm3 = userGrouplist.Find(v => v.Value == "G3");
                        userGrouplist.Remove(itm3);
                    }

                }
                else
                {
                    jobj["stat"] = "0";
                    jobj["msg"] = "Hod Flag is missing";
                }
            }
            else
            {
                jobj["stat"] = "0";
                jobj["msg"] = "Logged out please login";
            }

            return Json(userGrouplist, JsonRequestBehavior.AllowGet);


        }
        #endregion

        #region employee view
        public ActionResult EmployeeView()
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                EmployeeDetails model = new EmployeeDetails();

                // DataTable dt = service.SELECT_ALL_TBL_EMPLOYEE_DETAILS_ACTIVE();
                DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_ACTIVE", "STORE_PROCEDURE", new List<GeneralIOModel>());
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        model.list1 = GetEmployeeList(dt);
                    }
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public ActionResult EmployeeEdit(string id)
        {
            EmployeeDetails model = new EmployeeDetails();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                ViewBag.Departments = BindDropdown("M1");
                ViewBag.Designations = BindDropdown("M2");
                ViewBag.Grades = BindDropdown("M3");
                ViewBag.UserGroups = BindUserGroups();

                if (!string.IsNullOrEmpty(id))
                {
                    id = new CustomeDataProvider().Decryption(id);
                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                        new GeneralIOModel { Name = "EMP_CODE", Value =id }
                    });
                    //id = new CustomeDataProvider().Decryption(id);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            model.EmployeeName = dt.Rows[0]["EMPLOYEE_NAME"].ToString();
                            model.EmpCode = dt.Rows[0]["BPSCL_CODE"].ToString();
                            model.DOB = Convert.ToDateTime(dt.Rows[0]["DOB"]).ToString("dd/MM/yyyy");
                            model.Department = dt.Rows[0]["DEPARTMENT_CODE"].ToString();
                            model.AdditionalDepartment = dt.Rows[0]["ADDITIONAL_DEPARTMENT"].ToString();
                            model.AdditionalDepartmentTexts = dt.Rows[0]["ADDITIONAL_DEPARTMENT_VAL"].ToString();
                            model.Designation = dt.Rows[0]["DESIGNATION_CODE"].ToString();
                            model.ContactNo = dt.Rows[0]["CONTACT_NO"].ToString();
                            model.EmailAddress = dt.Rows[0]["EMAIL_ADDRESS"].ToString();
                            model.MembersOf = dt.Rows[0]["MEMBER_OF"].ToString();
                            model.MembersOfText = dt.Rows[0]["MEMBER_OF_VAL"].ToString();
                            model.RealEmpCode = dt.Rows[0]["EMP_CODE"].ToString();
                            model.Gender = dt.Rows[0]["GENDER"].ToString();
                            model.Grade = dt.Rows[0]["EMP_GRADE_CODE"].ToString();
                            model.ProfileImageName = dt.Rows[0]["PROFILE_IMAGE"].ToString();
                            model.SignatureName = dt.Rows[0]["SIGNATURE"].ToString();

                        }
                    }
                }
                return View("EmployeeMaster", model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public ActionResult BlockEmployee(EmployeeDetails model)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                if (!string.IsNullOrEmpty(model.EmpCode) && !string.IsNullOrEmpty(model.Reason))
                {
                    model.EmpCode = new CustomeDataProvider().Decryption(model.EmpCode);
                    // if (service.UPDATE_TBL_LOGIN_BLOCK_STATUS(model.EmpCode, model.Reason))
                    if ((bool)service.GeneralOperation("STATUS", "UPDATE_TBL_LOGIN_BLOCK_STATUS", "STORE_PROCEDURE", new List<GeneralIOModel>()
                    {
                        new GeneralIOModel{Name="EMP_CODE",Value=model.EmpCode},
                        new GeneralIOModel{Name="REASON",Value=model.Reason}
                    }))
                    {
                        TempData["successmsg"] = "Employee Blocked";
                    }
                    else
                    {
                        TempData["errormsg"] = "Employee not Blocked something went wrong";
                    }

                }
                return RedirectToAction("EmployeeView");
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public ActionResult ChangeEmployeePassword(EmployeeDetails model)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                if (!string.IsNullOrEmpty(model.EmpCode))
                {
                    if (!string.IsNullOrEmpty(model.OwnPassword))
                    {
                        model.OwnPassword = new CustomeDataProvider().Encryption(model.OwnPassword);
                        DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALL_TBL_EMPLOYEE_DETAILS_BY_EMP_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                            new GeneralIOModel { Name = "EMP_CODE", Value = Convert.ToString(Session["EMP_CODE"]) }
                        });
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["PASSWORD"].ToString() == model.OwnPassword)
                                {
                                    if (!string.IsNullOrEmpty(model.EmployeePassword))
                                    {
                                        model.EmpCode = new CustomeDataProvider().Decryption(model.EmpCode);
                                        model.EmployeePassword = new CustomeDataProvider().Encryption(model.EmployeePassword);

                                        // DataTable dt1 = service.UPDATE_TBL_LOGIN_RESET_PASSWORD(model.EmpCode, model.EmployeePassword, Session["EMP_CODE"].ToString(), model.OwnPassword);
                                        DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "UPDATE_TBL_LOGIN_RESET_PASSWORD", "STORE_PROCEDURE", new List<GeneralIOModel>()
                                        {
                                            new GeneralIOModel{Name="EMP_CODE",Value=model.EmpCode},
                                            new GeneralIOModel{Name="NEW_PASSWORD",Value=model.EmployeePassword},
                                            new GeneralIOModel{Name="LOGIN_USER",Value=Convert.ToString(Session["EMP_CODE"])},
                                            new GeneralIOModel{Name="LOGIN_USER_PASS",Value=model.OwnPassword}
                                        });
                                        if (dt1 != null)
                                        {
                                            if (dt1.Rows.Count > 0)
                                            {
                                                if (dt1.Rows[0]["STATUS"].ToString() == "SUCCESS")
                                                {
                                                    TempData["successmsg"] = dt1.Rows[0]["MSG"].ToString();
                                                }
                                                else
                                                {
                                                    TempData["errormsg"] = dt1.Rows[0]["MSG"].ToString();
                                                }
                                            }
                                        }


                                    }
                                    else
                                    {
                                        TempData["errormsg"] = "Employee password is missing";
                                    }
                                }
                                else
                                {
                                    TempData["errormsg"] = "Your password is wrong";
                                }
                            }
                        }



                    }
                    else
                    {
                        TempData["errormsg"] = "your own password is missing";
                    }


                }
                return RedirectToAction("EmployeeView");
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }


        #endregion
    }
}