using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
   public class CustomerImpt
   {
      [Key]
      public int ID { get; set; }

      public string No { get; set; }
      
      public string PrefixTh { get; set; }
      public int? PrefixID { get; set; }
      public string NameTh { get; set; }
      public string SurNameTh { get; set; }
      public string Passport { get; set; }

      public string Email { get; set; }
      public string IDCard { get; set; }
      public string MoblieNo { get; set; }

      public DateTime? Birthday { get; set; }
      public string Gender { get; set; }

      public string PrefixEn { get; set; }
      public int? PrefixIDEn { get; set; }
      public string NameEn { get; set; }
      public string SurNameEn { get; set; }

      public string Address { get; set; }
      public string Building { get; set; }
      public string HouseNo { get; set; }
      public string VillageNo { get; set; }
      public string VillageName { get; set; }
      public string Moo { get; set; }
      public string Soi { get; set; }

      public string Lane { get; set; }
      public string Road { get; set; }
      public string Province { get; set; }
      public string Tumbon { get; set; }
      public string Aumper { get; set; }
      public string ZipCode { get; set; }

      public string AddressEn { get; set; }
      public string BuildingEn { get; set; }
      public string HouseNoEn { get; set; }
      public string VillageNoEn { get; set; }
      public string VillageNameEn { get; set; }
      public string MooEn { get; set; }
      public string SoiEn { get; set; }
      public string LaneEn { get; set; }
      public string RoadEn { get; set; }
      public string ProvinceEn { get; set; }
      public string TumbonEn { get; set; }
      public string AumperEn { get; set; }
      public string ZipCodeEn { get; set; }

      public string Msg { get; set; }
      public bool Imported { get; set; }


   }
}
