using Dhipaya.Extensions;
using Dhipaya.Models;
using Dhipaya.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.DTO
{
   
   public class IIAExpireDTO 
   {
      //typereport|transdate|insuranceclass|subclass|effectivedate|expirydate|
      //newpolicyno|oldpolicyno|outletcode|invoiceno|newnetgprem|newduty|tax|totalamount|paid|insuredname|t_title|t_firstname|t_lastname|idcardno|
      //insuredaddress|contactaddress|projcode|projectdesc|customergroupid|custgroupname|packageid|
      //packagename|dfbookno|dfcdno|departmentid|depname|licenseno|email

      public string typereport { get; set; }
      public string transdate { get; set; }
      public string insuranceclass { get; set; }
      public string subclass { get; set; }
      public string effectivedate { get; set; }
      public string expirydate { get; set; }
      public string newpolicyno { get; set; }
      public string oldpolicyno { get; set; }
      public string outletcode { get; set; }
      public string invoiceno { get; set; }
      public string newnetgprem { get; set; }
      public string newduty { get; set; }
      public string tax { get; set; }
      public string totalamount { get; set; }
      public string paid { get; set; }
      public string insuredname { get; set; }
      public string t_title { get; set; }
      public string t_firstname { get; set; }
      public string t_lastname { get; set; }
      public string idcardno { get; set; }
      public string insuredaddress { get; set; }
      public string contactaddress { get; set; }
      public string projcode { get; set; }
      public string projectdesc { get; set; }
      public string customergroupid { get; set; }
      public string custgroupname { get; set; }
      public string packageid { get; set; }
      public string packagename { get; set; }
      public string dfbookno { get; set; }
      public string dfcdno { get; set; }
      public string departmentid { get; set; }
      public string depname { get; set; }
      public string licenseno { get; set; }
      public string email { get; set; }


   }
}
