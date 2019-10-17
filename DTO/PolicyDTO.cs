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
   public class IIAProvince
   {
      public int PVN_OICCode { get; set; }
      public int ProvinceBlockCode { get; set; }
      public int ProvinceId { get; set; }
      public string ProvinceName { get; set; }
   }
   public class IIAAmphur
   {
      public int AmphurId { get; set; }
      public int AmphurBlockCode { get; set; }
      public int ProvinceId { get; set; }
      public string AmphurName { get; set; }
   }
   public class IIATumbon
   {
      public int DistrictId { get; set; }
      public int DistrictBlockCode { get; set; }
      public int AmphurId { get; set; }
      public string DistrictName { get; set; }
      public string DistrictZipCode { get; set; }
   }
   public class IIAMemberResult
   {
      public string resultCode { get; set; }
      public string resultDescription { get; set; }
      public IIAMemberData[] data { get; set; }

   }
   public class IIAMemberResult2
   {
      public IIAMemberResult result { get; set; }
   }
   public class IIASMSResult
   {
      public string ResultCode { get; set; }
      public string ResultDescription { get; set; }
      public IIAMemberData[] Data { get; set; }

   }
   public class IIAMemberData
   {
      public string PolicyNo { get; set; }
      public string EffectiveDate { get; set; }
      public string ExpiryDate { get; set; }
      public string ProjectCode { get; set; }
      public string ProjectName { get; set; }
      public string SubClass { get; set; }
      public string PolicyPremium { get; set; }
      public string PreviousPolicyNo { get; set; }
      public string OutletCode { get; set; }
      public string InsuranceClass { get; set; }
   }

   public class IIACoverage
   {
      public string T_Coverage { get; set; }
      public string E_Coverage { get; set; }
      public string Suminsured { get; set; }
      public string AgreementNo { get; set; }
      public string AgreementBrcode { get; set; }
   }

}
