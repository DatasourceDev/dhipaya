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
   public class CategoryDTO : ModelBaseDTO
   {
      //public int? LastPrivilegeID { get; set; }
      public int AllCnt { get; set; }

      public IEnumerable<MerchantCategory> MerchantCategories { get; set; }

   }

   public class MerchantDTO : ModelBaseDTO
   {
      //public CustomerType? CustomerType { get; set; }
      //public string Outlets { get; set; }
      //public int? ProvinceID { get; set; }
      public int? CategoryID { get; set; }
      //public int? LastPrivilegeID { get; set; }
      public int AllCnt { get; set; }

      public IEnumerable<Province> Provinces { get; set; }
      public IEnumerable<Merchant> Merchants { get; set; }
      public IEnumerable<MerchantCategory> MerchantCategorys { get; set; }

   }
   public class MerchantInfoDTO
   {
      public int? PrivilegeID { get; set; }
      public Privilege Privilege { get; set; }
      public Customer Customer { get; set; }
   }
   public class PrivilegeDTO : ModelBaseDTO
   {
      public int? CustomerClassID { get; set; }
      public string Outlets { get; set; }
      public int? ProvinceID { get; set; }
      public int? CategoryID { get; set; }
      public int? LastPrivilegeID { get; set; }
      public int? LastIndex { get; set; }
      public int AllPrivilegeCnt { get; set; }
      public int? poppromo { get; set; }

      public IEnumerable<Privilege> Privileges { get; set; }
      public IEnumerable<Province> Provinces { get; set; }
      public IEnumerable<MerchantCategory> MerchantCategorys { get; set; }

   }
   public class PrivilegenfoDTO
   {
      public int? PrivilegeID { get; set; }
      public Privilege Privilege { get; set; }
      public Customer Customer { get; set; }
   }
}
