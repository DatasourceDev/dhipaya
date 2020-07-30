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
   public class ModelBaseDTO
   {
      public int result_code { get; set; }
      public string result_msg { get; set; }
      public string search_text { get; set; }
   }
   public class ModelReportBaseDTO
   {
      public string search_text { get; set; }
      public string search_equal_text { get; set; }
      public int? search_birthday { get; set; }
      public int? search_birthmonth { get; set; }
      public int? search_birthyear { get; set; }
      public string search_sdate { get; set; }
      public string search_edate { get; set; }
      public int? search_privilege { get; set; }
      public int? search_product_id { get; set; }
      public int? search_category_id { get; set; }
      public int? search_trantype { get; set; }
      public int? customerClassID { get; set; }
      public int? customerClassID2 { get; set; }
      public CustomerChanal? customer_chanal { get; set; }
      public UserLevelType? search_user_type { get; set; }
      public RedeemType? search_redeemtype { get; set; }

      public string search_code { get; set; }

      public int? pno { get; set; }
      public int? totalrow { get; set; }
      public int? pmax { get; set; }

      public string orderby { get; set; }
      public int? dup { get; set; }

   }
   public class ResetPwdDTO
   {
      public int ID { get; set; } //CustomerID
      public int? UserID { get; set; }

      public string oldpassword { get; set; }

      public string username { get; set; }

      [DataType(DataType.Password)]
      //[RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)[A-Za-z\\d]{8,12}$", ErrorMessage = "รหัสผ่านต้องประกอบด้วยตัวเลข ตัวอักษรใหญ่ และตัวอักษรเล็ก")]
      [StringLength(12, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
      public string password { get; set; }

      [DataType(DataType.Password)]
      [Compare("password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
      public string confirmPassword { get; set; }
   }

   public class MobileResetPwdDTO
   {
      [Required(ErrorMessage = "กรุณาระบุรหัสผ่าน")]
      public string Password { get; set; }

      [Required(ErrorMessage = "กรุณาระบุรหัสผู้ใช้")]
      public string UserName { get; set; }
      public string channel { get; set; }

   }

   public class MobileRandomPwdDTO
   {
      [Required(ErrorMessage = "กรุณาระบุรหัสผู้ใช้")]
      public string UserName { get; set; }

      public string channel { get; set; }

   }

   public class MobilePointDTO : ModelBaseDTO
   {
      [Required(ErrorMessage = "กรุณาระบุรหัสผู้ใช้")]
      public string UserName { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Product")]
      public string Product { get; set; }
      public int? ProductID { get; set; }

      public string Package { get; set; }
      public string PolicyNo { get; set; }
      public string OrderNo { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Channel")]
      public string Channel { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ Source")]
      public string Source { get; set; }

      public decimal? Amount { get; set; }

      public string IDCard { get; set; }

      public string Passport { get; set; }

   }
   public class CustomerDataAutoFillDTO : ModelBaseDTO
   {
      [Required(ErrorMessage = "กรุณาระบุ username")]
      public string username { get; set; }

      [Required(ErrorMessage = "กรุณาระบุ password")]
      public string password { get; set; }

      public string firstName { get; set; }

      public string lastName { get; set; }

      public int? cardType { get; set; }

      public string cardNo { get; set; }

      public string lang { get; set; }

   }
   public class CustomerStatusDTO : ModelBaseDTO
   {
      [Required(ErrorMessage = "กรุณาระบุรหัสผู้ใช้")]
      public string UserName { get; set; }
      public string Password { get; set; }
      public string flag { get; set; }
      public string facebookFlag { get; set; }
      public string ggFlag { get; set; }
   }
   public class SSODTO
   {
      public string UserName { get; set; }
      public string Password { get; set; }
      public string u { get; set; }
      public string p { get; set; }
      public string f { get; set; }
      public string flag { get; set; }
      public string facebookFlag { get; set; }
      public string ggFlag { get; set; }
   }
   public class SendDeleteDTO
   {
      public string idcard { get; set; }
   }
   public class TempDTO
   {
      public string str { get; set; }
   }
   public class CustomersImportDTO
   {
      public bool valid { get; set; }
      public bool dosave { get; set; }
      public bool? result { get; set; }
      public IList<CustomerImport> Imports { get; set; }
      public IList<CustomerImport> ImportFails { get; set; }

   }
   public class CustomerImport
   {
      public int row { get; set; }
      public string No { get; set; }
      public string PrefixTh { get; set; }
      public int? PrefixID { get; set; }
      public string NameTh { get; set; }
      public string SurNameTh { get; set; }
      public string Email { get; set; }
      public string Passport { get; set; }
      public string IDCard { get; set; }
      public string MoblieNo { get; set; }
      public string Create_By { get; set; }
      public DateTime? Create_On { get; set; }
      public string Msg { get; set; }
      public bool succeed { get; set; }
   }

   public class CustomersDTO : ModelReportBaseDTO
   {
      public IEnumerable<Customer> Customers { get; set; }
   }
   public class MailFriendGetFriendDTO
   {
      public Customer friend { get; set; }
      public Customer customer { get; set; }
      public int point { get; set; }
   }
   public class MailRedeemSendAddrestoAdminDTO
   {
      public Customer customer { get; set; }
      public Redeem redeem { get; set; }
      public Merchant merchant { get; set; }
      public Privilege privilege { get; set; }
   }
   public class MailSendDeleteAccountDTO
   {
      public IQueryable<Customer> customers { get; set; }
      public List<string> codes { get; set; }
   }
   public class CustomerDTO : ModelBaseDTO
   {
      public bool valid { get; set; } //CustomerID
      public bool ShowIdcardDupPopup { get; set; } //CustomerID
      public List<string> dupEmail { get; set; }
      public List<string> dupFBID { get; set; }
      public string dupIdcard { get; set; } //CustomerID

      public int ID { get; set; } //CustomerID
      public int? userID { get; set; }

      public string username { get; set; }
      public string passport { get; set; }
      public int? customerClassID { get; set; }
      public string customerClassName { get; set; }

      [DataType(DataType.Password)]
      //[RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)[A-Za-z\\d]{8,12}$", ErrorMessage = "รหัสผ่านต้องประกอบด้วยตัวเลข ตัวอักษรใหญ่ และตัวอักษรเล็ก")]
      [StringLength(12, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
      public string password { get; set; }
      public string pEncyprt { get; set; }

      [DataType(DataType.Password)]
      [Compare("password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
      public string confirmPassword { get; set; }

      [Required(ErrorMessage = "กรุณาระบุชื่อ")]
      public string firstName { get; set; }

      [Required(ErrorMessage = "กรุณาระบุนามสกุล")]
      public string lastName { get; set; }

      public string gender { get; set; }
      public string telNo { get; set; }
      public string prefix { get; set; }
      public string prefixEn { get; set; }

      public int? prefixInt { get; set; }
      public int? prefixEnInt { get; set; }

      public string firstNameEn { get; set; }

      public string lastNameEn { get; set; }

      [Required]
      public bool userConfirm { get; set; }

      [Required(ErrorMessage = "กรุณาระบุอีเมล (รหัสผู้ใช้งาน)")]
      [EmailAddress]
      [DataType(DataType.EmailAddress)]
      public string email { get; set; }


      //[Required(ErrorMessage = "หมายเลขโทรศัพท์มือถือ")]
      [Phone]
      [DataType(DataType.PhoneNumber)]
      public string moblieNo { get; set; }


      public string lineId { get; set; }

      public string citizenId { get; set; }
      public string citizenIdInit { get; set; }

      public bool isDhiMember { get; set; }

      public int birthdateDay { get; set; }
      public int birthdateMonth { get; set; }
      public int birthdateYear { get; set; }


      public string birthdate { get; set; }
      public string customerNo { get; set; }
      public string refCode { get; set; }
      public string friendCode { get; set; }
      public string promotionCode { get; set; }
      public string refTo { get; set; }

      public UserLevelType userLevel { get; set; }
      public string status { get; set; }

      public CustomerChanal channelInt { get; set; }
      public string channel { get; set; }
      public string facebookFlag { get; set; }
      public string facebookID { get; set; }

      public string address { get; set; }
      public string houseNo { get; set; }
      public string moo { get; set; }
      public string soi { get; set; }
      public string postalCode { get; set; }

      public string villageNo { get; set; }
      public string villageName { get; set; }
      public string lane { get; set; }
      public string road { get; set; }

      public string provinceName { get; set; }
      public string districtName { get; set; }
      public string subdistrictName { get; set; }

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

      public string addressEn { get; set; }
      public string houseNoEn { get; set; }
      public string mooEn { get; set; }
      public string soiEn { get; set; }
      public string postalCodeEn { get; set; }
      public string villageNoEn { get; set; }
      public string villageNameEn { get; set; }
      public string laneEn { get; set; }
      public string roadEn { get; set; }

      public string provinceNameEn { get; set; }
      public string districtNameEn { get; set; }
      public string subdistrictNameEn { get; set; }

      private int? _ProvinceIDEn;
      public int? provinceIdEn
      {
         get { if (_ProvinceIDEn == 0) return null; return _ProvinceIDEn; }
         set { if (value == 0) value = null; _ProvinceIDEn = value; }
      }

      private int? _SubDistrictIDEn;
      public int? subDistrictIdEn
      {
         get { if (_SubDistrictIDEn == 0) return null; return _SubDistrictIDEn; }
         set { if (value == 0) value = null; _SubDistrictIDEn = value; }
      }

      private int? _DistrictIDEn;
      public int? districtIdEn
      {
         get { if (_DistrictIDEn == 0) return null; return _DistrictIDEn; }
         set { if (value == 0) value = null; _DistrictIDEn = value; }
      }

      public DateTime? createon { get; set; }
      public DateTime? updateon { get; set; }
      public bool IIAIgnoreSyned { get; set; }

      public bool FirstLogedIn { get; set; }

   }

   public class CustomerPointHistoryDTO
   {
      public DateTime TransactionDate { get; set; }
      public decimal Point { get; set; }
      public Redeem Redeem { get; set; }
      public CustomerPoint CustomerPoint { get; set; }
   }
   public class CustomerBinding
   {
      public static string GetCustomerAddress(Customer model, DAL.ChFrontContext context)
      {
         var address = "";
         if (model != null)
         {
            if (!string.IsNullOrEmpty(model.CUR_HouseNo))
               address = model.CUR_HouseNo + " ";
            if (model.CUR_Tumbon.HasValue)
            {
               var tumbon = context.Tumbons.Where(w => w.TumbonID == model.CUR_Tumbon).FirstOrDefault();
               if (tumbon != null)
                  address += tumbon.TumbonName + " ";
            }
            if (model.CUR_Aumper.HasValue)
            {
               var aum = context.Aumphurs.Where(w => w.AumphurID == model.CUR_Aumper).FirstOrDefault();
               if (aum != null)
                  address += aum.AumphurName + " ";
            }
            if (model.CUR_Province.HasValue)
            {
               var province = context.Provinces.Where(w => w.ProvinceID == model.CUR_Province).FirstOrDefault();
               if (province != null)
                  address += province.ProvinceName + " ";
            }
            if (!string.IsNullOrEmpty(model.CUR_ZipCode))
               address += model.CUR_ZipCode + " ";
         }
         return address;
      }

      public static Customer Binding(Customer customer, CustomerDTO model)
      {
         if (customer == null)
         {
            customer = new Customer();
            customer.Create_On = DateUtil.Now();
            customer.ChannelUpdate = CustomerChanal.TIP;
         }

         if (!string.IsNullOrEmpty(model.email))
            customer.Email = model.email;

         if (model.prefixInt != null)
            customer.PrefixTh = model.prefixInt;

         if (!string.IsNullOrEmpty(model.firstName))
            customer.NameTh = model.firstName;

         if (!string.IsNullOrEmpty(model.lastName))
            customer.SurNameTh = model.lastName;

         if (model.prefixEnInt != null)
            customer.PrefixEn = model.prefixEnInt;

         if (!string.IsNullOrEmpty(model.firstNameEn))
            customer.NameEn = model.firstNameEn;

         if (!string.IsNullOrEmpty(model.lastNameEn))
            customer.SurNameEn = model.lastNameEn;

         if (!string.IsNullOrEmpty(model.moblieNo))
            customer.MoblieNo = model.moblieNo;

         if (!string.IsNullOrEmpty(model.telNo))
            customer.TelNo = model.telNo;

         if (!string.IsNullOrEmpty(model.lineId))
            customer.LineID = model.lineId;

         if (!string.IsNullOrEmpty(model.citizenId))
            customer.IDCard = model.citizenId;

         if (!string.IsNullOrEmpty(model.gender))
            customer.Gender = model.gender;

         if (model.prefixEnInt != null)
            customer.UserID = model.userID;

         if (!string.IsNullOrEmpty(model.passport))
            customer.Passport = model.passport;

         /*address*/

         if (customer.ChannelUpdate == CustomerChanal.TipInsure)
         {
            customer.CUR_HouseNo = model.houseNo;
            customer.CUR_Soi = model.lane;
            customer.CUR_Lane = model.lane;
            customer.CUR_Road = model.road;
            customer.CUR_Moo = model.villageNo;
            customer.CUR_VillageNo = model.villageNo;
            customer.CUR_VillageName = model.villageName;

            customer.CUR_HouseNoEn = model.houseNoEn;
            customer.CUR_SoiEn = model.laneEn;
            customer.CUR_LaneEn = model.laneEn;
            customer.CUR_RoadEn = model.roadEn;
            customer.CUR_MooEn = model.villageNoEn;
            customer.CUR_VillageNoEn = model.villageNoEn;
            customer.CUR_VillageNameEn = model.villageNameEn;
         }
         else if (customer.ChannelUpdate == CustomerChanal.Mobile)
         {
            customer.CUR_HouseNo = model.houseNo;
            customer.CUR_Moo = model.villageNo;
            customer.CUR_VillageNo = model.villageNo;
            customer.CUR_VillageName = model.villageName;
            customer.CUR_Lane = model.lane;
            customer.CUR_Soi = model.lane;
            customer.CUR_Road = model.road;

            customer.CUR_HouseNoEn = model.houseNo;
            customer.CUR_MooEn = model.villageNo;
            customer.CUR_VillageNoEn = model.villageNoEn;
            customer.CUR_VillageNameEn = model.villageNameEn;
            customer.CUR_LaneEn = model.laneEn;
            customer.CUR_SoiEn = model.laneEn;
            customer.CUR_RoadEn = model.roadEn;
         }
         else
         {
            customer.CUR_HouseNo = model.houseNo;
            customer.CUR_Road = model.road;
            customer.CUR_Soi = model.soi;
            customer.CUR_Lane = model.soi;
            customer.CUR_Moo = model.moo;
            customer.CUR_VillageName = model.villageName;
            customer.CUR_VillageNo = model.moo;

            customer.CUR_HouseNoEn = model.houseNoEn;
            customer.CUR_RoadEn = model.roadEn;
            customer.CUR_SoiEn = model.soiEn;
            customer.CUR_LaneEn = model.soiEn;
            customer.CUR_MooEn = model.mooEn;
            customer.CUR_VillageNameEn = model.villageNameEn;
            customer.CUR_VillageNoEn = model.mooEn;
         }

         if (model.provinceId != null)
            customer.CUR_Province = model.provinceId;

         if (model.subDistrictId != null)
            customer.CUR_Tumbon = model.subDistrictId;

         if (model.districtId != null)
            customer.CUR_Aumper = model.districtId;

         if (!string.IsNullOrEmpty(model.postalCode))
            customer.CUR_ZipCode = model.postalCode;

         /*address En*/
         if (model.provinceIdEn != null)
            customer.CUR_ProvinceEn = model.provinceIdEn;
         else if (model.provinceId != null)
            customer.CUR_ProvinceEn = model.provinceId;

         if (model.subDistrictIdEn != null)
            customer.CUR_TumbonEn = model.subDistrictIdEn;
         else if (model.subDistrictId != null)
            customer.CUR_TumbonEn = model.subDistrictId;

         if (model.districtIdEn != null)
            customer.CUR_AumperEn = model.districtIdEn;
         else if (model.districtId != null)
            customer.CUR_AumperEn = model.districtId;

         if (!string.IsNullOrEmpty(model.postalCodeEn))
            customer.CUR_ZipCodeEn = model.postalCodeEn;
         else if (!string.IsNullOrEmpty(model.postalCode))
            customer.CUR_ZipCodeEn = model.postalCode;

         if (!string.IsNullOrEmpty(model.facebookFlag) && model.ID <= 0)
            customer.FacebookFlag = model.facebookFlag;

         if (!string.IsNullOrEmpty(model.birthdate))
            customer.DOB = DateUtil.ToDate(model.birthdate);
         else if (model.birthdateDay > 0 && model.birthdateMonth> 0 && model.birthdateYear > 0)
            customer.DOB = DateUtil.ToDate(model.birthdateDay, model.birthdateMonth, model.birthdateYear);


         if (!string.IsNullOrEmpty(model.status))
            customer.Status = model.status.toUserStatus();

         if (!string.IsNullOrEmpty(model.refCode))
            customer.RefCode = model.refCode;

         if (customer.User == null)
            customer.User = new User();

         if (string.IsNullOrEmpty(model.username))
            model.username = model.email;

         if (!string.IsNullOrEmpty(model.email))
            customer.User.Email = model.email;

         if (!string.IsNullOrEmpty(model.moblieNo))
            customer.User.PhoneNumber = model.moblieNo;

         if (!string.IsNullOrEmpty(model.username))
            customer.User.UserName = model.username;

         customer.UserLevel = model.userLevel;
         customer.User.UserRoleID = 2;

         if (!string.IsNullOrEmpty(model.status))
            customer.User.Status = customer.Status;

         if (!string.IsNullOrEmpty(model.password))
         {
            customer.User.Password = DataEncryptor.Encrypt(model.password);

            if (string.IsNullOrEmpty(customer.BCryptPwd))
               customer.BCryptPwd = BCrypt.Net.BCrypt.HashPassword(customer.User.Password);

            customer.ResetedPwd = true;
         }

         if (customer.ID > 0)
            customer.RefCode = GetRefCode(customer);

         customer.Syned = true;

         customer.Channel = model.channelInt;
         //customer.PromotionCode = model.promotionCode;

         if (!string.IsNullOrEmpty(model.friendCode))
            customer.FriendCode = model.friendCode;

         if (!string.IsNullOrEmpty(model.facebookID))
            customer.FacebookID = model.facebookID;

         customer.DoSendReisterEmail = true;
         customer.Update_On = DateUtil.Now();

         if (customer.CustomerPoints == null)
            customer.CustomerPoints = new List<CustomerPoint>();
         return customer;
      }

      public static string GetRefCode(Customer customer)
      {
         var refcode = "TIPSO";
         refcode += customer.Create_On.Value.Year.ToString().Substring(2, 2);
         refcode += customer.Create_On.Value.Month.ToString("00");
         refcode += customer.ID.ToString("00000000");
         return refcode;
      }


      private static int _checksum_ean13(String data)
      {
         // Test string for correct length
         if (data.Length != 12 && data.Length != 13)
            return -1;

         // Test string for being numeric
         for (int i = 0; i < data.Length; i++)
         {
            if (data[i] < 0x30 || data[i] > 0x39)
               return -1;
         }

         int sum = 0;

         for (int i = 11; i >= 0; i--)
         {
            int digit = data[i] - 0x30;
            if ((i & 0x01) == 1)
               sum += digit;
            else
               sum += digit * 3;
         }
         int mod = sum % 10;
         return mod == 0 ? 0 : 10 - mod;
      }

      public static CustomerDTO Binding(Customer customer)
      {
         var model = new CustomerDTO();
         model.ID = customer.ID;
         model.username = customer.User.UserName;
         model.pEncyprt = customer.User.Password;
         model.email = customer.Email;
         model.prefixInt = customer.PrefixTh;
         model.firstName = customer.NameTh;
         model.lastName = customer.SurNameTh;
         model.prefixEnInt = customer.PrefixEn;
         model.firstNameEn = customer.NameEn;
         model.lastNameEn = customer.SurNameEn;
         model.moblieNo = customer.MoblieNo;
         model.lineId = customer.LineID;
         model.citizenId = customer.IDCard;
         model.citizenIdInit = customer.IDCard;
         model.userLevel = customer.UserLevel;
         model.userID = customer.UserID;
         model.passport = customer.Passport;

         model.address = customer.CUR_Address;
         model.houseNo = customer.CUR_HouseNo;
         model.houseNoEn = customer.CUR_HouseNoEn;
         model.provinceId = customer.CUR_Province;
         model.provinceIdEn = customer.CUR_ProvinceEn;
         model.subDistrictId = customer.CUR_Tumbon;
         model.subDistrictIdEn = customer.CUR_TumbonEn;
         model.districtId = customer.CUR_Aumper;
         model.districtIdEn = customer.CUR_AumperEn;
         model.postalCode = customer.CUR_ZipCode;
         model.postalCodeEn = customer.CUR_ZipCodeEn;
         model.villageNo = customer.CUR_VillageNo;
         if (!string.IsNullOrEmpty(model.villageNo))
            model.villageNo = customer.CUR_Moo;

         model.villageName = customer.CUR_VillageName;
         model.villageNameEn = customer.CUR_VillageNameEn;

         model.moo = customer.CUR_Moo;
         model.mooEn = customer.CUR_MooEn;
         model.soi = customer.CUR_Soi;
         model.soiEn = customer.CUR_SoiEn;
         model.lane = customer.CUR_Lane;
         if (!string.IsNullOrEmpty(model.lane))
            model.lane = customer.CUR_Soi;

         model.laneEn = customer.CUR_LaneEn;
         if (!string.IsNullOrEmpty(model.laneEn))
            model.laneEn = customer.CUR_SoiEn;

         model.road = customer.CUR_Road;
         model.roadEn = customer.CUR_RoadEn;
         model.channelInt = customer.Channel;
         model.promotionCode = customer.PromotionCode;
         model.friendCode = customer.FriendCode;
         model.facebookFlag = customer.FacebookFlag;

         model.birthdate = DateUtil.ToDisplayDate(customer.DOB);
         model.status = customer.Status.toStatusName();
         model.customerNo = customer.CustomerNo;
         model.refCode = customer.RefCode;
         model.customerClassID = customer.CustomerClassID;
         model.userLevel = customer.UserLevel;
         model.facebookID = customer.FacebookID;
         model.updateon = customer.Update_On;
         model.createon = customer.Create_On;
         model.FirstLogedIn = customer.FirstLogedIn;
         model.IIAIgnoreSyned = customer.IIAIgnoreSyned;

         if (customer.CustomerClass != null)
            model.customerClassName = customer.CustomerClass.Name;
         return model;
      }
      public static string RandomString(int length)
      {
         var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
         var stringChars = new char[8];
         var random = new Random();

         for (int i = 0; i < stringChars.Length; i++)
         {
            stringChars[i] = chars[random.Next(chars.Length)];
         }

         var finalString = new String(stringChars);
         return finalString;
      }
   }
}
