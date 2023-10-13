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
    public class MasterEntryController : ApplicationController
    {
        // GET: MasterEntry
        ServiceProvider service = new ServiceProvider();
        #region functions
        public List<SelectListItem> GetMasterTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_ALLTBL_MASTER_TYPE_USER_ENTRY", "STORE_PROCEDURE", new List<GeneralIOModel>());
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new SelectListItem()
                        {

                            Text = dr["MASTER_DESCRIPTION"].ToString(),
                            Value = dr["MASTER_CODE"].ToString()
                        });
                    }
                }
            }


            return list;
        }

        public List<MasterEntryDetails> GetMasterItemList(DataTable dt)
        {
            List<MasterEntryDetails> lst = new List<MasterEntryDetails>();

            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new MasterEntryDetails()
                {
                    RecordName = dr["RECORD_NAME"].ToString(),
                    RecordStatus = dr["RECORD_STATUS"].ToString(),
                    MasterCode = new CustomeDataProvider().Encryption(dr["MASTER_CODE"].ToString()),

                    RecordId = new CustomeDataProvider().Encryption(dr["RECORD_ID"].ToString())
                });
            }

            return lst;
        }


        #endregion



        public ActionResult MasterEntry(MasterEntryDetails model, string Submit)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                ViewBag.MasterItemTypes = GetMasterTypes();

                if (!string.IsNullOrEmpty(Submit))
                {
                    if (Submit == "Save")
                    {
                        if (string.IsNullOrEmpty(model.RecordId))
                        {
                            //for inserting

                            
                            if ((bool)service.GeneralOperation("STATUS", "INSERT_INTO_TBL_MASTER_VALUE", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="RECORD_ID",Value=""},
                                new GeneralIOModel{Name="MASTER_CODE",Value=model.MasterCode},
                                new GeneralIOModel{Name="PARENT_CODE",Value=""},
                                new GeneralIOModel{Name="RECORD_NAME",Value=model.RecordName},
                                new GeneralIOModel{Name="DESC_1",Value=""},
                                new GeneralIOModel{Name="DESC_2",Value=""},
                                new GeneralIOModel{Name="DESC_3",Value=""},
                                new GeneralIOModel{Name="DESC_4",Value=""},
                                new GeneralIOModel{Name="DESC_5",Value=""},
                                new GeneralIOModel{Name="DESC_6",Value=""},
                                new GeneralIOModel{Name="ENTRY_BY",Value=Convert.ToString(Session["EMP_CODE"])}
                            }))
                            {
                                ModelState.Clear();
                                model = new MasterEntryDetails();
                                TempData["successmsg"] = "Data Inserted Successfully";

                            }
                            else
                            {
                                TempData["errormsg"] = "data not saved something went wrong!!";
                                return View(model);
                            }
                        }
                        else
                        {

                            //for updating

                           
                            if ((bool)service.GeneralOperation("STATUS", "UPDATE_INSERT_TBL_MASTER_VALUE_RECORD_ID", "STORE_PROCEDURE", new List<GeneralIOModel>()
                            {
                                new GeneralIOModel{Name="RECORD_ID",Value=model.RecordId},
                                new GeneralIOModel{Name="MASTER_CODE",Value=model.MasterCode},
                                new GeneralIOModel{Name="PARENT_CODE",Value=""},
                                new GeneralIOModel{Name="RECORD_NAME",Value=model.RecordName},
                                new GeneralIOModel{Name="DESC_1",Value=""},
                                new GeneralIOModel{Name="DESC_2",Value=""},
                                new GeneralIOModel{Name="DESC_3",Value=""},
                                new GeneralIOModel{Name="DESC_4",Value=""},
                                new GeneralIOModel{Name="DESC_5",Value=""},
                                new GeneralIOModel{Name="DESC_6",Value=""},
                                new GeneralIOModel{Name="ENTRY_BY",Value=Convert.ToString(Session["EMP_CODE"])}
                            }))
                            {

                                TempData["successmsg"] = "Data Updated Successfully";
                                return RedirectToAction("MasterEntryView", model);
                            }
                            else
                            {
                                TempData["errormsg"] = "data not updated something went wrong!!";
                                return View(model);
                            }
                        }

                    }
                }




                return View(model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public ActionResult MasterEntryView(MasterEntryDetails model)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {
                ViewBag.MasterItemTypes = GetMasterTypes();

                if (!string.IsNullOrEmpty(model.MasterCode))
                {
                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_FROM_TBL_MASTER_VALUE_ACTIVE_MASTER_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>()
                    {
                        new GeneralIOModel{Name="MASTER_CODE",Value=model.MasterCode }
                    });
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            model.list1 = GetMasterItemList(dt);
                        }
                    }
                }


                return View(model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }



        public ActionResult MasterItemEdit(string id)
        {
            MasterEntryDetails model = new MasterEntryDetails();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {

                ViewBag.MasterItemTypes = GetMasterTypes();
                if (!string.IsNullOrEmpty(id))
                {
                    id = new CustomeDataProvider().Decryption(id);
                    DataTable dt = (DataTable)service.GeneralOperation("DATA", "SELECT_FROM_TBL_MASTER_VALUE_ACTIVE_RECORD_ID", "STORE_PROCEDURE", new List<GeneralIOModel>()
                    {
                        new GeneralIOModel{Name="RECORD_ID",Value=id }
                    });
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            model.MasterCode = dt.Rows[0]["MASTER_CODE"].ToString();
                            model.RecordId = dt.Rows[0]["RECORD_ID"].ToString();
                            model.RecordName = dt.Rows[0]["RECORD_NAME"].ToString();

                        }
                    }
                }
                return View("MasterEntry", model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }

        public JsonResult DuplicateCheckMasterType(string FieldName, string Value, string MasterCode, string RecordId)
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

                            if (!string.IsNullOrEmpty(MasterCode))
                            {
                                int presentflag = 0;
                                if (FieldName == "MasterValue")
                                {

                                    DataTable dt1 = (DataTable)service.GeneralOperation("DATA", "SELECT_FROM_TBL_MASTER_VALUE_ACTIVE_MASTER_CODE", "STORE_PROCEDURE", new List<GeneralIOModel>() {
                                       new GeneralIOModel { Name = "MASTER_CODE", Value = MasterCode }
                                   });



                                    if (!string.IsNullOrEmpty(RecordId))
                                    {
                                        string recordname = "";
                                        DataTable dt2 = (DataTable)service.GeneralOperation("DATA", "SELECT_FROM_TBL_MASTER_VALUE_ACTIVE_RECORD_ID", "STORE_PROCEDURE", new List<GeneralIOModel>()
                                        {
                                            new GeneralIOModel{Name="RECORD_ID",Value=RecordId }
                                        });
                                        if (dt2 != null)
                                        {
                                            if (dt2.Rows.Count > 0)
                                            {
                                                recordname = dt2.Rows[0]["RECORD_NAME"].ToString();
                                            }
                                        }

                                        if (recordname.Replace(" ", "").ToUpper() != Value.Replace(" ", "").ToUpper())
                                        {
                                            if (dt1 != null)
                                            {
                                                if (dt1.Rows.Count > 0)
                                                {
                                                    foreach (DataRow dr in dt1.Rows)
                                                    {
                                                        if (dr["RECORD_NAME"].ToString().Replace(" ", "").ToUpper() == Value.Replace(" ", "").ToUpper())
                                                        {
                                                            presentflag = presentflag + 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dt1 != null)
                                        {
                                            if (dt1.Rows.Count > 0)
                                            {
                                                foreach (DataRow dr in dt1.Rows)
                                                {
                                                    if (dr["RECORD_NAME"].ToString().Replace(" ", "").ToUpper() == Value.Replace(" ", "").ToUpper())
                                                    {
                                                        presentflag = presentflag + 1;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                }






                                if (presentflag > 0)
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

        public ActionResult HoldOrReleaseMasterItem(string id, string Status, string MasterCode)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["EMP_CODE"])))
            {

                MasterEntryDetails model = new MasterEntryDetails();
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(Status))
                {
                    id = new CustomeDataProvider().Decryption(id);
                    MasterCode = new CustomeDataProvider().Decryption(MasterCode);
                    model.RecordId = id;
                    model.MasterCode = MasterCode;

                    if ((bool)service.GeneralOperation("STATUS", "UPDATE_STATUS_TBL_MASTER_VALUE", "STORE_PROCEDURE", new List<GeneralIOModel>()
                    {
                        new GeneralIOModel{Name="RECORD_ID",Value=id}
                    }))
                    {
                        if (Status == "ACTIVE")
                        {

                            TempData["successmsg"] = "Master Item Successfully Released ";
                        }
                        else if (Status == "INACTIVE")
                        {
                            TempData["successmsg"] = "Master Item Successfully Holded ";

                        }
                    }
                    else
                    {
                        if (Status == "ACTIVE")
                        {

                            TempData["errormsg"] = "Master Item not released something went wrong";
                        }
                        else if (Status == "INACTIVE")
                        {
                            TempData["errormsg"] = "Master Item not holded something went wrong";

                        }
                    }

                }
                return RedirectToAction("MasterEntryView", model);
            }
            else
            {
                return RedirectToAction("LogOut", "Login");
            }
        }


      
    }
}