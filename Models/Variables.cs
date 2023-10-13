using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EMPLOYEE_MANAGEMENT.Models
{
    public class GeneralIOModel
    {
        public object Name { get; set; }
        public object Value { get; set; }
    }

    public class AccountDetails
    {
        [Display(Name = "Login")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string Newpassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmedPassword { get; set; }


        public string FYSessionCode { get; set; }

    }


    public class EmployeeDetails
    {
        public string RealEmpCode { get; set; }
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string DOB { get; set; }
        public string Grade { get; set; }
        public string ContactNo { get; set; }
        public string AdditionalDepartment { get; set; }
        public string AdditionalDepartmentTexts { get; set; }
        public string HdnAdditionalDepartment { get; set; }
        public string EmailAddress { get; set; }
        public string LastLoginTime { get; set; }
        public string MembersOf { get; set; }
        public string MembersOfText { get; set; }
        public string ProfileImageName { get; set; }
        public string SignatureName { get; set; }
        public string Gender { get; set; }
        public string UserGroups { get; set; }
        public string Reason { get; set; }
        public string OwnPassword { get; set; }
        public string EmployeePassword { get; set; }




        public List<EmployeeDetails> list1 { get; set; }
    }



    public class MasterEntryDetails
    {

        public string MasterCode { get; set; }
        public string RecordId { get; set; }
        public string RecordName { get; set; }
        public string RecordStatus { get; set; }
        public string MasterType { get; set; }
        public string MasterValue { get; set; }
        public List<MasterEntryDetails> list1 { get; set; }
    }

}