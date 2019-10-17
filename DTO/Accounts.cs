using Dhipaya.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.DTO.Accounts
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class Forgot
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

      public Customer Customer { get; set; }

      public string Url { get; set; }

   }
   public class CheckLoginDTO
   {
      public string responseDesc { get; set; }
      public string responseCode { get; set; }
      public CheckLoginCustomerProfileDTO customerProfile { get; set; }
   }
   public class CheckLoginCustomerProfileDTO
   {
      public string username { get; set; }

      public string password { get; set; }

      public string firstName { get; set; }

      public string lastName { get; set; }

      public string prefix { get; set; }
      public string prefixEn { get; set; }

      public string firstNameEn { get; set; }

      public string lastNameEn { get; set; }

      public string email { get; set; }

      public string moblieNo { get; set; }

      public string lineId { get; set; }

      public string citizenId { get; set; }

      public string birthdate { get; set; }
      public string friendCode { get; set; }
      public string customerType { get; set; }
      public string villageNo { get; set; }
      public string villageName { get; set; }
      public string villageNameEn { get; set; }
      public string lane { get; set; }
      public string laneEn { get; set; }
      public string road { get; set; }
      public string roadEn { get; set; }
      public string facebookFlag { get; set; }
      public string facebookID { get; set; }

      public string houseNo { get; set; }
      public string postalCode { get; set; }

      private int? _ProvinceID;
      public int? provinceId
      {
         get { if (_ProvinceID == 0) return null; return _ProvinceID; }
         set { if (value == 0) value = null; _ProvinceID = value; }
      }

      private int? _SubDistrictID;
      public int? subDistrictId
      {
         get { if (_SubDistrictID == 0) return null; return _SubDistrictID; }
         set { if (value == 0) value = null; _SubDistrictID = value; }
      }

      private int? _DistrictID;
      public int? districtId
      {
         get { if (_DistrictID == 0) return null; return _DistrictID; }
         set { if (value == 0) value = null; _DistrictID = value; }
      }
   }

}
