using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class Customer
   {
      public Customer()
      {
         this.User = new User();
         this.CustomerPoints = new List<CustomerPoint>();
      }

      [Key]
      public int ID { get; set; }

      public int? CustomerClassID { get; set; }

      public string Gender { get; set; }

      public bool? Success { get; set; }
      public bool RegGeneratedPoint { get; set; }

      public string MoblieNo { get; set; }
      public string TelNo { get; set; }
      public string WorkTelNo { get; set; }

      public UserStatusType Status { get; set; }

      public string LineID { get; set; }

      public string IDCard { get; set; }

      public int? PrefixTh { get; set; }

      [Required]
      public string NameTh { get; set; }

      [Required]
      public string SurNameTh { get; set; }

      public int? PrefixEn { get; set; }

      public string NameEn { get; set; }

      public string SurNameEn { get; set; }

      public DateTime? DOB { get; set; }

      public int? LastgivePointDOB { get; set; }

      [Required]
      [DataType(DataType.EmailAddress)]
      public string Email { get; set; }

      public bool IsDhiMember { get; set; }

      public string CUR_Address { get; set; }
      public string CUR_Building { get; set; }
      public string CUR_HouseNo { get; set; }
      public string CUR_Moo { get; set; }
      public string CUR_Soi { get; set; }
      public string CUR_VillageNo { get; set; }
      public string CUR_VillageName { get; set; }
      public string CUR_Lane { get; set; }
      public string CUR_Road { get; set; }
      public int? CUR_Province { get; set; }
      public int? CUR_Tumbon { get; set; }
      public int? CUR_Aumper { get; set; }
      public string CUR_ZipCode { get; set; }

      public string CUR_AddressEn { get; set; }
      public string CUR_BuildingEn { get; set; }
      public string CUR_HouseNoEn { get; set; }
      public string CUR_MooEn { get; set; }
      public string CUR_SoiEn { get; set; }
      public string CUR_VillageNoEn { get; set; }
      public string CUR_VillageNameEn { get; set; }
      public string CUR_LaneEn { get; set; }
      public string CUR_RoadEn { get; set; }
      public int? CUR_ProvinceEn { get; set; }
      public int? CUR_TumbonEn { get; set; }
      public int? CUR_AumperEn { get; set; }
      public string CUR_ZipCodeEn { get; set; }

      public string RefCode { get; set; }
      public string CustomerNo { get; set; }
      public string PromotionCode { get; set; }
      public string Passport { get; set; }

      public UserLevelType UserLevel { get; set; }

      public int? UserID { get; set; }
      public virtual User User { get; set; }

      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Update_By { get; set; }
      public DateTime? Update_On { get; set; }

      
      public CustomerChanal Channel { get; set; }
      public CustomerChanal ChannelUpdate { get; set; }
      public string FacebookFlag { get; set; }
      public string FacebookID { get; set; }

      public string FriendCode { get; set; }
      public string BCryptPwd { get; set; }
      public bool Syned { get; set; }

      [DefaultValue(true)]
      public bool DoSendReisterEmail { get; set; }
      public bool SentReisterEmail { get; set; }
      public bool IIASyned { get; set; }
      public bool IIAIgnoreSyned { get; set; }
      public bool UpdatedAllRequired { get; set; }


      public string SentReisterMsg { get; set; }

      [NotMapped]
      public int RedeemPoint { get; set; }
      [NotMapped]
      public int Point { get; set; }

      [NotMapped]
      public int Redeemed { get; set; }

      [NotMapped]
      public int Friends { get; set; }

      public bool ResetedPwd { get; set; }


      public virtual ICollection<CustomerPoint> CustomerPoints { get; set; }
      public virtual CustomerClass CustomerClass { get; set; }

      public bool FirstLogedIn { get; set; }
   }
}
