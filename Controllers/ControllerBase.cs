using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dhipaya.DTO.Accounts;
using Microsoft.Extensions.Logging;
using Dhipaya.Services;
using Dhipaya.DAL;
using Dhipaya.Models;
using Microsoft.EntityFrameworkCore;
using Dhipaya.DTO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Dhipaya.Extensions;
using Microsoft.Extensions.Options;
using System.ServiceModel;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using OfficeOpenXml;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{
   public class ControllerBase : Controller
   {
      public IPrivilegeRepository _priRepo;
      public ICustomerRepository _cusRepo;
      public IReportRepository _rptRepo;

      public ChFrontContext _context;
      public ILoginServices _loginServices;
      public ILogger _logger;
      public TIPMobile _mobile;
      public IIA _iia;
      public Smtp _smtp;
      public SystemConf _conf;


      public ControllerBase()
      {
      }

      public ControllerBase(ChFrontContext context, ILogger<Controller> logger, IOptions<TIPMobile> _mobile, IOptions<IIA> _iia, IOptions<Smtp> smtp, ILoginServices loginServices, IOptions<SystemConf> conf, ICustomerRepository cusRepo, IReportRepository rptRepo)
      {
         this._context = context;
         this._logger = logger;
         this._mobile = _mobile.Value;
         this._smtp = smtp.Value;
         this._conf = conf.Value;
         this._iia = _iia.Value;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
         this._loginServices = loginServices;
      }

      public string[] GetRoles()
      {

         var controller = this.ControllerContext.RouteData.Values["Controller"].ToString();
         var page = this._context.Pages.Where(w => w.PageCode == controller).FirstOrDefault();
         if (page == null)
         {
            return null;
         }
         var roles = this._context.UserRoles.Where(w => w.PageRoles.Where(w2 => w2.PageID == page.PageID).FirstOrDefault() != null).Select(s => s.RoleName);
         return roles.ToArray();
      }
      public string[] GetErrorModelState()
      {
         return this.ViewData.ModelState.SelectMany(m => m.Value.Errors, (m, error) => (m.Key + " : " + error.ErrorMessage)).ToArray();
      }

      #region  Validation
      public bool isExistFriendCode(CustomerDTO model)
      {
         if (!string.IsNullOrEmpty(model.friendCode))
         {
            var query = this._context.Customers.Where(c => c.RefCode == model.friendCode).FirstOrDefault();
            return (query != null);
         }
         return false;
      }

      public bool isExistRefCode(CustomerDTO model)
      {
         if (string.IsNullOrEmpty(model.refCode))
            return false;
         var query = this._context.Customers.Where(c => c.RefCode == model.refCode).FirstOrDefault();
         return (query != null);
      }

      public bool isExistIDCard(CustomerDTO model)
      {
         if (string.IsNullOrEmpty(model.citizenId))
            return false;
         var query = this._context.Customers.Where(c => c.IDCard == model.citizenId & (model.ID > 0 ? c.ID != model.ID : true)).FirstOrDefault();
         return (query != null);
      }

      public bool isExistEmail(CustomerDTO model)
      {
         if (model.ID > 0)
         {
            var customer = this._context.Customers.Where(c => c.ID == model.ID).FirstOrDefault();
            if (customer != null)
            {
               if (customer.Email == model.email)
                  return false;
            }
         }
         var query = this._context.Customers.Where(c => c.Email == model.email & (model.ID > 0 ? c.ID != model.ID : true)).FirstOrDefault();
         return (query != null);
      }
      public bool isExistUserName(CustomerDTO model)
      {
         var query = this._context.Users.Where(c => c.UserName == model.username & (model.userID.HasValue ? c.ID != model.userID : true)).FirstOrDefault();
         return (query != null);
      }

      public bool isExistName(CustomerDTO model)
      {
         var query = this._context.Customers.Where(c => c.NameTh == model.firstName & c.SurNameTh == model.lastName & (model.ID > 0 ? c.ID != model.ID : true)).FirstOrDefault();
         return (query != null);
      }
      public bool isExistMobileNo(CustomerDTO model)
      {
         var query = this._context.Customers.Where(c => c.MoblieNo == model.moblieNo & (model.ID > 0 ? c.ID != model.ID : true)).FirstOrDefault();
         return (query != null);
      }

      public bool isExistUserName(User model)
      {
         var query = this._context.Users.Where(c => c.UserName == model.UserName & (model.ID > 0 ? c.ID != model.ID : true)).FirstOrDefault();
         return (query != null);
      }

      #endregion

      #region  Repair
      public async Task<bool> Repair(string u, string p, string f, string sname = "login", string bcrypt = null)
      {
         var rg = new RijndaelCrypt();
         using (var client = new HttpClient())
         {
            client.BaseAddress = new Uri(_mobile.Url + "/rewardpoint/customerprofile/" + sname);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var dto = new SSODTO();
            dto.u = rg.Encrypt(u);
            dto.p = rg.Encrypt(p);
            if (!string.IsNullOrEmpty(f))
               dto.flag = rg.Encrypt(f);
            _logger.LogWarning(JsonConvert.SerializeObject(dto));
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
            if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
               var responseresult = await response.Content.ReadAsStringAsync();
               var result = JsonConvert.DeserializeObject<CheckLoginDTO>(responseresult);

               if (result.customerProfile == null)
                  return false;

               var json = JsonConvert.SerializeObject(result.customerProfile);

               var cmodel = JsonConvert.DeserializeObject<CustomerDTO>(json);
               if (!string.IsNullOrEmpty(cmodel.citizenId))
                  cmodel.isDhiMember = true;
               cmodel.username = u;
               cmodel.password = null;

               if (f == "y")
                  cmodel.facebookFlag = "Y";
               var customer = new Customer();
               customer.Create_On = DateUtil.Now();
               customer.ChannelUpdate = CustomerChanal.Mobile;
               customer = CustomerBinding.Binding(customer, cmodel);
               customer.Syned = false;
               customer.BCryptPwd = p;
               customer.Create_On = DateUtil.Now();
               customer.Create_By = customer.User.UserName;
               customer.Update_On = DateUtil.Now();
               customer.Update_By = customer.User.UserName;
               if (!string.IsNullOrEmpty(bcrypt))
                  customer.BCryptPwd = bcrypt;

               GetCustomerClass(customer);

               this._context.Customers.Add(customer);
               this._context.SaveChanges();
               this._context.Entry(customer).GetDatabaseValues();
               customer.RefCode = CustomerBinding.GetRefCode(customer);
               this._context.Users.Attach(customer.User);
               this._context.Entry(customer.User).Property(user => user.Email).IsModified = true;
               this._context.Entry(customer.User).Property(user => user.PhoneNumber).IsModified = true;
               this._context.Update(customer);
               this._context.SaveChanges();
               return true;
            }
         }
         return false;
      }
      #endregion

      #region  GetPolicyActive
      private XmlDocument CreateSoapGetPolicyActive(string id, string name, string surname)
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.Append(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">");
         requiredXML.Append(@"<soap-env:Header/>");
         requiredXML.Append(@"<soap-env:Body>");
         requiredXML.Append(@"<tem:CheckMemberStatus>");
         requiredXML.Append(@"<tem:idCardNo>" + id + "</tem:idCardNo>");
         requiredXML.Append(@"<tem:firstname>" + name + "</tem:firstname>");
         requiredXML.Append(@"<tem:lastname>" + surname + "</tem:lastname>");
         requiredXML.Append(@"</tem:CheckMemberStatus>");
         requiredXML.Append(@"</soap-env:Body>");
         requiredXML.Append(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }
      protected IIAMemberResult GetRequestPolicyActiveDev(string id, string name, string surname)
      {
         using (var client = new HttpClient())
         {
            client.BaseAddress = new Uri(_conf.Url + "/rewardpoint/customerprofile/IIACheck?name=" + name + "&surname=" + surname + "&idc=" + id);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;
            if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
               var responseresult = response.Content.ReadAsStringAsync().Result;
               var result = JsonConvert.DeserializeObject<IIAMemberResult>(responseresult);
               if (result.data != null)
                  result.data = result.data.Where(w => !string.IsNullOrEmpty(w.PolicyNo)).OrderByDescending(o => DateUtil.ToDate(o.EffectiveDate, monthfirst: true)).ToArray();
               return result;
            }
         }
         return new IIAMemberResult();
      }
      protected IIAMemberResult GetRequestPolicyActive(IIA _iia, string id, string name, string surname)
      {
         _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Begin: " + id + " " + name + " " + surname);
         var _result = new IIAMemberResult();
         XmlDocument soapRequest = CreateSoapGetPolicyActive(id, name, surname);
         using (var client = new HttpClient())
         {
            try
            {
               var request = new HttpRequestMessage()
               {
                  RequestUri = new Uri(_iia.EndPoint),
                  Method = HttpMethod.Post,
                  Content = new StringContent(soapRequest.InnerXml, Encoding.UTF8, "text/xml")
               };

               request.Headers.Clear();
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
               request.Headers.Add("ContentType", "text/xml;charset=\"utf-8\";");

               request.Headers.Add("SOAPAction", _iia.CheckMemberStatus); //I want to call this method 
               HttpResponseMessage response = client.SendAsync(request).Result;

               if (!response.IsSuccessStatusCode)
               {
                  _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Error: " + id + " " + response.RequestMessage.ToString());
                  _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Error Input: " + id + " " + soapRequest.InnerXml);
                  return new IIAMemberResult() { resultCode = "-1", resultDescription = response.RequestMessage.ToString() };
               }

               Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
               Stream stream = streamTask.Result;
               var sr = new StreamReader(stream);
               XDocument soapResponse = XDocument.Load(sr);
               XElement el = soapResponse.Descendants().First(x => x.Name.LocalName == "CheckMemberStatusResult");
               var val = el.Value;
               _result = JsonConvert.DeserializeObject<IIAMemberResult>(val);
               if (_result.data != null)
                  _result.data = _result.data.Where(w => !string.IsNullOrEmpty(w.PolicyNo)).OrderByDescending(o => DateUtil.ToDate(o.EffectiveDate, monthfirst: true)).ToArray();

               _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Result: " + val);
               //do some other stuff...
               return _result;
            }
            catch (Exception ex)
            {
               _result.resultDescription = ex.Message;
               _result.resultCode = "-1";

               _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Exception: " + ex.Message);
               _logger.LogWarning(DateUtil.Now() + " CheckMemberStatus Exception Input : " + soapRequest.InnerXml);
            }
            return _result;
         }
      }

      protected void GetCustomerClass(Customer customer, bool dosave = false, bool getpoint = true)
      {
         var oldclass = customer.CustomerClassID;
         customer.CustomerClassID = 1;
         ViewBag.CustomerClass = "TIP Silver";
         var connected = false;
         if (!string.IsNullOrEmpty(customer.IDCard))
         {
            IIAMemberResult iia = null;
            if (_conf.Environment == "Dev")
               iia = GetRequestPolicyActiveDev(customer.IDCard, customer.NameTh, customer.SurNameTh);
            else
               iia = GetRequestPolicyActive(_iia, customer.IDCard, customer.NameTh, customer.SurNameTh);

            if (iia != null && (iia.resultCode == "Y" | iia.resultCode == "N"))
            {
               connected = true;
            }
            var avaliable = false;

            if (iia != null && iia.resultCode == "Y")
            {
               var havecustom = false;
               if (iia.data != null)
               {
                  foreach (var item in iia.data)
                  {
                     if (DateUtil.ToDate(item.ExpiryDate, monthfirst: true).Value.AddDays(1) >= DateUtil.Now())
                        avaliable = true;

                     foreach (var c in _context.CustomerClasses.Where(w => !w.UnEditable & w.Status == StatusType.Active))
                     {
                        if (item.ProjectCode == c.ProjectCode && DateUtil.ToDate(item.ExpiryDate, monthfirst: true).Value.AddDays(1) >= DateUtil.Now())
                        {
                           _logger.LogWarning(item.ProjectCode + " " + item.ExpiryDate);
                           havecustom = true;
                           customer.CustomerClassID = c.ID;
                           ViewBag.CustomerClass = c.Name;
                           break;
                        }
                     }

                  }
               }
               if (havecustom == false && avaliable)
               {
                  customer.CustomerClassID = 2;
                  ViewBag.CustomerClass = "TIP Gold";
               }

            }
            customer.IIASyned = true;
            if (customer.IIAIgnoreSyned & customer.CustomerClassID == 1 && avaliable)
            {
               customer.CustomerClassID = 2;
               ViewBag.CustomerClass = "TIP Gold";
            }

            if (customer.ID > 0 && connected)
            {
               if (oldclass != customer.CustomerClassID)
               {
                  var latest = _context.CustomerClassChanges.Where(w => w.CustomerID == customer.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                  var log = new CustomerClassChange();
                  log.FromID = oldclass;
                  log.ToID = customer.CustomerClassID;
                  var oldc = _context.CustomerClasses.Where(w => w.ID == log.FromID).FirstOrDefault();
                  if (oldc != null)
                     log.From = oldc.Name;
                  var newc = _context.CustomerClasses.Where(w => w.ID == log.ToID).FirstOrDefault();
                  if (newc != null)
                     log.To = newc.Name;

                  log.Create_On = DateUtil.Now();
                  log.Create_By = customer.Update_By;
                  log.CustomerID = customer.ID;

                  if (latest == null || latest.FromID != log.FromID || latest.ToID != log.ToID)
                  {
                     _context.CustomerClassChanges.Add(log);
                  }
               }

               if (iia != null && iia.resultCode == "Y" && getpoint)
               {
                  foreach (var item in iia.data)
                  {
                     if (!string.IsNullOrEmpty(item.PolicyNo))
                     {
                        var pointed = _context.CustomerPoints.Where(w => w.PolicyNo == item.PolicyNo & w.CustomerID == customer.ID & w.ChannelType == ChannelType.IIA).FirstOrDefault();
                        if (pointed == null)
                        {
                           var trantype = TransacionTypeID.BuyInsure;
                           if (!string.IsNullOrEmpty(item.PreviousPolicyNo))
                              trantype = TransacionTypeID.Renew;
                           IEnumerable<PointCondition> cons = cons = this.GetPointCondition(customer, trantype, item.InsuranceClass, item.OutletCode, ChannelType.IIA, item.SubClass);
                           if (cons != null)
                           {
                              foreach (var con in cons)
                              {
                                 var p = this.GetPoint(con, customer, limited: false);
                                 if (p > 0)
                                 {
                                    var point = this.GetCustomerPointByIIA(con, customer, p, (int)trantype, item);
                                    point.CustomerClassName = ViewBag.CustomerClass;
                                    if (!string.IsNullOrEmpty(item.InsuranceClass))
                                    {
                                       var product = _context.Products.Where(w => w.ProductCode == item.InsuranceClass).FirstOrDefault();
                                       if (product != null)
                                          point.ProductID = product.ProductID;
                                    }
                                    this._context.Add(point);
                                 }
                              }
                           }
                        }
                     }
                  }
               }
            }
         }


         if (dosave)
         {
            customer.Update_On = DateUtil.Now();
            customer.Update_By = "IIACheck";
            _context.SaveChanges();
         }

      }
      #endregion

      #region  SMS
      private XmlDocument CreateSoapSMS(Customer model, string message)
      {
         XmlDocument soapEnvelop = new XmlDocument();
         var requiredXML = new StringBuilder();
         requiredXML.AppendLine(@"<?xml version='1.0' encoding='utf-8'?>");
         requiredXML.AppendLine(@"<soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">");
         requiredXML.AppendLine(@"<soap-env:Header/>");
         requiredXML.AppendLine(@"<soap-env:Body>");
         requiredXML.AppendLine(@"<tem:call_sms>");
         requiredXML.AppendLine(@"<tem:source>TipSocity</tem:source>");
         requiredXML.AppendLine(@"<tem:key_name1>Customer No</tem:key_name1>");
         requiredXML.AppendLine(@"<tem:key_descript1>" + model.CustomerNo + "</tem:key_descript1>");
         requiredXML.AppendLine(@"<tem:key_name2>?</tem:key_name2>");
         requiredXML.AppendLine(@"<tem:key_descript2>?</tem:key_descript2>");
         requiredXML.AppendLine(@"<tem:userid>" + model.CustomerNo + "</tem:userid>");
         requiredXML.AppendLine(@"<tem:mobileno>" + model.MoblieNo + "</tem:mobileno>");
         requiredXML.AppendLine(@"<tem:message>" + message + "</tem:message>");
         requiredXML.AppendLine(@"<tem:sender>Dhipaya</tem:sender>");
         requiredXML.AppendLine(@"<tem:Language>?</tem:Language>");
         requiredXML.AppendLine(@"<tem:key_date1>Register Date</tem:key_date1>");
         requiredXML.AppendLine(@"<tem:key_date_descript1>" + DateUtil.ToDisplayDate(model.Create_On) + "</tem:key_date_descript1>");
         requiredXML.AppendLine(@"</tem:call_sms>");
         requiredXML.AppendLine(@"</soap-env:Body>");
         requiredXML.AppendLine(@"</soap-env:Envelope>");
         soapEnvelop.LoadXml(requiredXML.ToString());
         return soapEnvelop;
      }

      public IActionResult SendSMS(int? id = null)
      {
         try
         {
            if (!id.HasValue)
               return Json(new { result = "Customer: Data has not found." });

            var model = this._context.Customers.Include(i => i.CustomerClass).Where(w => w.ID == id).Include(i => i.User).FirstOrDefault();
            string message = "ยินดีต้อนรับ สู่ TIP Society โซเชียลแห่งความสุข สนุกได้ทุกไลฟ์สไตล์ คุณเป็นสมาชิกระดับ " + model.CustomerClass.Name + " หมายเลขสมาชิก: " + model.RefCode + " รหัสผู้ใช้งาน: " + model.User.UserName + " และ รหัสผ่าน : " + DataEncryptor.Decrypt(model.User.Password) + " ติดตามรายละเอียดได้ที่ https://tipsociety.dhipaya.co.th";
            return Json(new { result = SendRequestSMS(model, message) });
         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
      }
      public IActionResult ReSendSMS(int? id = null)
      {
         try
         {
            if (!id.HasValue)
               return Json(new { result = "Customer: Data has not found." });

            var model = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(w => w.ID == id).FirstOrDefault();
            string message = "ท่านเคยลงทะเบียนสมัครเป็นสมาชิก TIP Society แล้ว สามารถ log in โดยใช้ รหัสผู้ใช้งาน: " + model.User.UserName + " เพื่อรับสิทธิพิเศษกับร้านค้าที่ร่วมโครงการฯได้เลยค่ะ ขอบคุณค่ะ ติดตามรายละเอียดได้ที่ https://tipsociety.dhipaya.co.th";
            return Json(new { result = SendRequestSMS(model, message) });
         }
         catch (Exception ex)
         {
            return Json(new { msg = ex.Message });
         }
      }
      protected string SendRequestSMS(Customer model, string message)
      {
         XmlDocument soapRequest = CreateSoapSMS(model, message);
         using (var client = new HttpClient())
         {
            var request = new HttpRequestMessage()
            {
               RequestUri = new Uri(_iia.sms_endpoint),
               Method = HttpMethod.Post,
               Content = new StringContent(soapRequest.InnerXml, Encoding.UTF8, "text/xml"),
            };


            request.Headers.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            request.Headers.Add("ContentType", "text/xml;charset=\"utf-8\";");
            request.Headers.Add("SOAPAction", _iia.call_sms); //I want to call this method 

            HttpResponseMessage response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
               return response.RequestMessage.ToString();
            }

            Task<Stream> streamTask = response.Content.ReadAsStreamAsync();
            Stream stream = streamTask.Result;
            var sr = new StreamReader(stream);
            XDocument soapResponse = XDocument.Load(sr);
            XElement el = soapResponse.Descendants().First(x => x.Name.LocalName == "call_smsResult");
            var val = el.Value;
            var _result = JsonConvert.DeserializeObject<string>(val);
            //do some other stuff...
            return _result;
         }
      }
      #endregion

      protected SelectList ListPrefix()
      {
         var list = new List<CustomerPrefix>();
         list.AddRange(this._context.CustomerPrefixs.OrderBy(o => o.ID));
         return new SelectList(list, "ID", "Name");
      }
      protected SelectList ListPrefixEn()
      {
         var list = new List<CustomerPrefix>();
         list.AddRange(this._context.CustomerPrefixs.Where(w=>!string.IsNullOrEmpty(w.NameEng)).OrderBy(o => o.ID));
         return new SelectList(list, "ID", "NameEng");
      }
      #region  Address
      protected SelectList ListProvince()
      {
         var list = new List<Province>();
         list.Add(new Province() { ProvinceName = "-" });
         list.AddRange(this._context.Provinces.OrderBy(o => o.ProvinceName));
         return new SelectList(list, "ProvinceID", "ProvinceName");
      }
      protected SelectList ListProvinceEn()
      {
         var list = new List<Province>();
         list.Add(new Province() { ProvinceNameEn = "-" });
         list.AddRange(this._context.Provinces.OrderBy(o => o.ProvinceNameEn));
         return new SelectList(list, "ProvinceID", "ProvinceNameEn");
      }
      protected SelectList ListAumphur(int? pID)
      {
         var list = new List<Aumphur>();
         list.Add(new Aumphur() { AumphurName = "-" });
         list.AddRange(this._context.Aumphurs.Where(w => w.ProvinceID == pID).OrderBy(o => o.AumphurName));
         return new SelectList(list, "AumphurID", "AumphurName");
      }
      protected SelectList ListAumphurEn(int? pID)
      {
         var list = new List<Aumphur>();
         list.Add(new Aumphur() { AumphurNameEn = "-" });
         list.AddRange(this._context.Aumphurs.Where(w => w.ProvinceID == pID).OrderBy(o => o.AumphurNameEn));
         return new SelectList(list, "AumphurID", "AumphurNameEn");
      }
      protected SelectList ListTumbon(int? aID)
      {
         var list = new List<Tumbon>();
         list.Add(new Tumbon() { TumbonName = "-" });
         list.AddRange(this._context.Tumbons.Where(w => w.AumphurID == aID).OrderBy(o => o.TumbonName));
         return new SelectList(list, "TumbonID", "TumbonName");
      }
      protected SelectList ListTumbonEn(int? aID)
      {
         var list = new List<Tumbon>();
         list.Add(new Tumbon() { TumbonNameEn = "-" });
         list.AddRange(this._context.Tumbons.Where(w => w.AumphurID == aID).OrderBy(o => o.TumbonNameEn));
         return new SelectList(list, "TumbonID", "TumbonNameEn");
      }

      [HttpGet]
      public ActionResult LoadAumphur(int? pID, string lang)
      {
         if (lang == "EN")
            return Json(ListAumphurEn(pID));
         else
            return Json(ListAumphur(pID));

      }
      [HttpGet]
      public ActionResult LoadTumbon(int? aID, string lang)
      {
         if (lang == "EN")
            return Json(ListTumbonEn(aID));
         else
            return Json(ListTumbon(aID));

      }
      [HttpGet]
      public ActionResult LoadPostal(int? tID)
      {
         var tumbon = this._context.Tumbons.Where(w => w.TumbonID == tID).FirstOrDefault();
         if (tumbon != null)
            return Json(tumbon.PostalCode);
         else
            return Json("");
      }

      #endregion

      #region Mail
      public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
      {
         if (string.IsNullOrEmpty(viewName))
         {
            viewName = this.ControllerContext.ActionDescriptor.ActionName;
         }

         this.ViewData.Model = model;

         using (var writer = new StringWriter())
         {
            IViewEngine viewEngine = this.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            ViewEngineResult viewResult = viewEngine.FindView(this.ControllerContext, viewName, !partial);

            if (viewResult.Success == false)
            {
               return $"A view with the name {viewName} could not be found";
            }

            ViewContext viewContext = new ViewContext(
                this.ControllerContext,
                viewResult.View,
                this.ViewData,
                this.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
         }
      }

      public async Task<IActionResult> MailReActivateAcc(string email, int? id)
      {
         var model = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.ID == id).FirstOrDefault();
         if (model != null)
         {
            var htmlToConvert = await RenderViewAsync("MailReActivateAcc", model, true);
            var msg = EmailUtil.sendNotificationEmail(_smtp, email, "คุณเป็นสมาชิก TIP Society แล้ว ใช้สิทธิ์ได้ทันที", htmlToConvert.ToString());
            if (string.IsNullOrEmpty(msg))
            {
               //model.SentReisterEmail = true;
               model.Update_On = DateUtil.Now();
            }
            else
            {
               _logger.LogWarning(DateUtil.Now() + " Sent Reactivate Error: " + email + " " + msg);

               //model.SentReisterEmail = true;
               model.SentReisterMsg = msg;
               model.Update_On = DateUtil.Now();
            }
            this._context.SaveChanges();

            return Json(new { Msg = msg });
         }
         return Json(new { Msg = "customer not found" });
      }

      public async Task<IActionResult> MailActivateAcc(string email, int? id)
      {
         var model = this._context.Customers.Include(i => i.CustomerClass).Include(i => i.User).Where(c => c.ID == id).FirstOrDefault();
         if (model != null)
         {
            var htmlToConvert = await RenderViewAsync("MailActivateAcc", model, true);
            var msg = "";
            if (model.Channel == CustomerChanal.ShareHolderImport)
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "ขอเชิญสมัครสมาชิก TIP Society รับสิทธิพิเศษ จากทิพยประกันภัย", htmlToConvert.ToString());
            if (model.Channel == CustomerChanal.AmazonImport)
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "เชิญสมัครฟรี! สมาชิก TIP Society รับสิทธิพิเศษมากมายจาก ทิพยประกันภัย", htmlToConvert.ToString());
            else
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "สมัครสมาชิกสำเร็จ " + model.RefCode, htmlToConvert.ToString());
            if (string.IsNullOrEmpty(msg))
            {
               model.SentReisterEmail = true;
               model.Update_On = DateUtil.Now();
            }
            else
            {
               _logger.LogWarning(DateUtil.Now() + " Sent Register Error: " + email + " " + msg);

               model.SentReisterEmail = true;
               model.SentReisterMsg = msg;
               model.Update_On = DateUtil.Now();
            }
            this._context.SaveChanges();

            return Json(new { Msg = msg });
         }
         return Json(new { Msg = "customer not found" });
      }

      public async Task<bool> MailActivateAcc(string email, Customer model)
      {
         if (model != null)
         {
            _logger.LogWarning(DateUtil.Now() + " Sent Mail: " + model.RefCode + " To " + email);
            var htmlToConvert = await RenderViewAsync("MailActivateAcc", model, true);
            var msg = "";
            if (model.Channel == CustomerChanal.ShareHolderImport)
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "ขอเชิญสมัครสมาชิก TIP Society รับสิทธิพิเศษ จากทิพยประกันภัย", htmlToConvert.ToString());
            else if (model.Channel == CustomerChanal.AmazonImport)
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "เชิญสมัครฟรี! สมาชิก TIP Society รับสิทธิพิเศษมากมายจาก ทิพยประกันภัย", htmlToConvert.ToString());
            else if (model.Channel == CustomerChanal.DhiMemberImport)
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "เชิญสมัครฟรี! สมาชิก TIP Society รับสิทธิพิเศษมากมายจาก ทิพยประกันภัย", htmlToConvert.ToString());
            else
               msg = EmailUtil.sendNotificationEmail(_smtp, email, "สมัครสมาชิกสำเร็จ " + model.RefCode, htmlToConvert.ToString());
            if (string.IsNullOrEmpty(msg))
            {
               model.SentReisterEmail = true;
               model.Update_On = DateUtil.Now();
               return true;
            }
            else
            {
               _logger.LogWarning(DateUtil.Now() + " Sent Register Error: " + email + " " + msg);

               model.SentReisterEmail = true;
               model.SentReisterMsg = msg;
               model.Update_On = DateUtil.Now();
               return false;
            }
         }
         return false;
      }

      public async Task<IActionResult> MailInviteFriend(string email, Customer friend, Customer customer, int point)
      {
         if (friend != null && customer != null)
         {
            var conf = _context.Configurations.Where(w => w.Name == ConfigurationeCode.SentMailInviteFriend).FirstOrDefault();
            if (conf != null && conf.Value == "1")
            {
               var model = new MailFriendGetFriendDTO();
               model.point = point;
               model.friend = friend;
               model.customer = customer;
               var htmlToConvert = await RenderViewAsync("MailInviteFriend", model, true);
               var msg = EmailUtil.sendNotificationEmail(_smtp, email, "คุณได้รับคะแนนสะสมจาก TIP Society (friend get friend)", htmlToConvert.ToString());
               return Json(new { Msg = msg });
            }
            else
            {
               return Json(new { Msg = "" });
            }
         }
         return Json(new { Msg = "customer not found" });
      }

      public async Task<IActionResult> MailRedeemSendAddrestoAdmin(Customer customer, Redeem redeem)
      {
         if (customer != null && redeem != null)
         {

            var model = new MailRedeemSendAddrestoAdminDTO();
            model.redeem = redeem;
            model.customer = customer;
            model.privilege = _context.Privileges.Where(w => w.PrivilegeID == redeem.PrivilegeID).FirstOrDefault();
            if (model.privilege != null)
               model.merchant = _context.Merchants.Where(w => w.MerchantID == model.privilege.MerchantID).FirstOrDefault();
            var htmlToConvert = await RenderViewAsync("MailRedeemSendAddrestoAdmin", model, true);
            var msg = EmailUtil.sendNotificationEmail(_smtp, _conf.SupportEmail, "จัดส่งสินค้าทางไปรษณีย์ให้ " + customer.NameTh + " " + customer.SurNameTh, htmlToConvert.ToString());
            return Json(new { Msg = msg });

         }
         return Json(new { Msg = "customer or redeem has not found" });
      }

      public async Task<IActionResult> MailDeleteAccount(IQueryable<Customer> customers, List<string> codes)
      {
         var model = new MailSendDeleteAccountDTO();
         model.customers = customers;
         model.codes = codes;

         var htmlToConvert = await RenderViewAsync("MailDeleteAccount", model, true);        
         var email = "";
         foreach (var customer in customers)
         {
            if (string.IsNullOrEmpty(customer.FacebookID))
            {
               email += customer.User.UserName;
               email += ",";
            }
         }
         if (email.Length > 0)
         {
            if (email.Substring(email.Length - 1, 1) == ",")
               email = email.Substring(0, email.Length - 1);
         }
         var msg = EmailUtil.sendNotificationEmail(_smtp, email, "แจ้งลบบัญชีสมาชิก TIP Society", htmlToConvert.ToString());
         return Json(new { Msg = msg });

      }

      #endregion

      #region Point


      public int GetPoint(PointCondition condition, Customer model, decimal? purchaseAmt = 0, bool limited = true)
      {
         int point = 0;
         if (condition != null)
         {
            /*Point Type*/
            if (condition.PointType == PointType.Percentage)
            {
               if (condition.Percent > 0 && purchaseAmt > 0)
               {
                  point = NumUtil.ParseInteger(purchaseAmt.Value * (condition.Percent.Value / 100));
               }
            }
            else if (condition.PointType == PointType.Calculate)
            {
               if (condition.CalPoint > 0 && condition.CalPointPurchaseAmt > 0 && purchaseAmt > 0)
               {
                  point = NumUtil.ParseInteger(Math.Floor(purchaseAmt.Value / condition.CalPointPurchaseAmt.Value) * condition.CalPoint.Value);
               }
            }
            else if (condition.PointType == PointType.Tier)
            {
               if (purchaseAmt > 0)
               {
                  var tiers = this._context.PointConditionTiers.Where(w => w.ConditionID == condition.ConditionID && w.PurchaseAmtFrom <= purchaseAmt && w.PurchaseAmtTo >= purchaseAmt);
                  foreach (var tier in tiers)
                  {
                     if (tier.NumberType == NumberType.Percent)
                     {
                        if (tier.Percent > 0)
                           point += NumUtil.ParseInteger(purchaseAmt.Value * (tier.Percent.Value / 100));
                     }
                     else
                     {
                        if (tier.Point > 0)
                           point += NumUtil.ParseInteger(tier.Point.Value);
                     }
                  }
               }
            }
            else
               point = NumUtil.ParseInteger(condition.Point.HasValue ? condition.Point.Value : 0);

            if (limited == true)
            {
               /*Calculate Limit*/

               var limit = this._context.PointLimits.FirstOrDefault();
               if (limit != null)
               {
                  var now = DateUtil.Now();
                  if (limit.Period == Period.Once)
                  {
                     if (limit.LimitedOnce > 0 & point > limit.LimitedOnce)
                     {
                        return limit.LimitedOnce.Value;
                     }
                  }
                  else if (limit.Period == Period.Day)
                  {
                     if (limit.LimitedDay > 0)
                     {
                        var allpoint = this._context.CustomerPoints.Where(w => w.CustomerID == model.ID & w.Create_On.Value.Date == now.Date).Sum(w => w.Point);
                        if (allpoint >= limit.LimitedDay)
                           return 0;
                        else if (point + allpoint > limit.LimitedDay)
                           return limit.LimitedDay.Value - allpoint;
                     }
                  }
                  else if (limit.Period == Period.Week)
                  {
                     if (limit.LimitedWeek > 0)
                     {
                        var allpoint = this._context.CustomerPoints.Where(w => w.CustomerID == model.ID & w.Create_On.Value.Date > now.Date.AddDays(-7) & w.Create_On.Value.Date < now.Date.AddDays(7)).Sum(w => w.Point);
                        if (allpoint >= limit.LimitedWeek)
                           return 0;
                        else if (point + allpoint > limit.LimitedWeek)
                           return limit.LimitedWeek.Value - allpoint;
                     }

                  }
                  else if (limit.Period == Period.Month)
                  {
                     if (limit.LimitedMonth > 0)
                     {
                        var allpoint = this._context.CustomerPoints.Where(w => w.CustomerID == model.ID & w.Create_On.Value.Month == now.Month).Sum(w => w.Point);
                        if (allpoint >= limit.LimitedMonth)
                           return 0;
                        else if (point + allpoint > limit.LimitedMonth)
                           return limit.LimitedMonth.Value - allpoint;
                     }
                  }
               }
            }


         }

         return point;
      }
      public IEnumerable<PointCondition> GetPointCondition(Customer model, TransacionTypeID trantype, string code = null, string outlet = null, ChannelType ch = ChannelType.Other, string subclass = null)
      {
         if (model == null)
            return null;
         if (model.Status == UserStatusType.BlockAward)
            return null;

         var conditions = this._context.PointConditions
            .Include(i => i.PointConditionProducts)
            .Where(w => w.TransacionTypeID == (int)trantype
            & w.Status == StatusType.Active
            & (!w.StartDate.HasValue || w.StartDate.Value.Date <= DateUtil.Now().Date)
            & (!w.EndDate.HasValue || w.EndDate.Value.Date >= DateUtil.Now().Date));



         if (trantype == TransacionTypeID.BuyInsure | trantype == TransacionTypeID.Renew)
         {
            if (outlet == OutletCode.MobileApplication | outlet == OutletCode.TipInsureWeb)
            {
               conditions = conditions.Where(w => w.OutletCode == outlet);
            }
            else
            {
               conditions = conditions.Where(w => w.OutletCode == OutletCode.Other);
            }

            if (ch != ChannelType.Other)
            {
               conditions = conditions.Where(w => w.ChannelType == ch);
            }
         }

         var currentdate = DateUtil.Now();

         if (!string.IsNullOrEmpty(code) & (trantype == TransacionTypeID.BuyInsure | trantype == TransacionTypeID.Renew))
         {
            var products = this._context.Products.Where(w => w.ProductCode == code);
            if (!string.IsNullOrEmpty(subclass))
               products = products.Where(w => (w.SubProductCode == subclass | string.IsNullOrEmpty(w.SubProductCode)));

            var product = products.FirstOrDefault();
            if (product != null)
               conditions = conditions.Where(w => w.ConditionCode == code | w.PointConditionProducts.Where(w2 => w2.ProductID == product.ProductID).FirstOrDefault() != null);
            else
               conditions = conditions.Where(w => w.ConditionCode == code);
         }
         if (ch != ChannelType.IIA)
            conditions = conditions.Where(w => w.PointConditionCustomerClasses.Any(s => s.CustomerClassID == model.CustomerClassID));

         if (model.DOB.HasValue)
         {
            if (model.DOB.Value.Day != currentdate.Day | model.DOB.Value.Month != currentdate.Month)
               conditions = conditions.Where(w => w.IsForBirthday == false);
         }
         else
            conditions = conditions.Where(w => w.IsForBirthday == false);

         if (currentdate.DayOfWeek == DayOfWeek.Sunday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsSun == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Monday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsMon == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Tuesday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsTue == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Wednesday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsWed == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Thursday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsThu == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Friday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsFri == true);
         else if (currentdate.DayOfWeek == DayOfWeek.Saturday)
            conditions = conditions.Where(w => w.IsAllDay == true | w.IsSat == true);
         return conditions;
      }

      public CustomerPoint GetCustomerPointByIIA(PointCondition item, Customer customer, int pointamt, int typeID, IIAMemberData data)
      {
         var point = new CustomerPoint();
         point.Code = item.ConditionCode;
         point.Name = item.Name;
         point.Point = pointamt;
         point.TransacionTypeID = typeID;
         point.CustomerChanal = customer.Channel;
         point.Source = "iia-offline";
         point.ChannelType = ChannelType.IIA;
         point.CustomerID = customer.ID;
         point.ProjectCode = data.ProjectCode;
         point.ProjectName = data.ProjectName;
         point.PolicyNo = data.PolicyNo;
         point.PreviousPolicyNo = data.PreviousPolicyNo;
         point.OutletCode = data.OutletCode;
         point.InsuranceClass = data.InsuranceClass;
         point.Subclass = data.SubClass;
         point.EffectiveDate = DateUtil.ToDate(data.EffectiveDate, monthfirst: true);
         point.ExpiryDate = DateUtil.ToDate(data.ExpiryDate, monthfirst: true);

         point.Create_On = DateUtil.Now();
         point.Create_By = customer.User.UserName;
         point.Update_On = DateUtil.Now();
         point.Update_By = customer.User.UserName;
         return point;
      }

      public CustomerPoint GetCustomerPoint(PointCondition item, Customer customer, int pointamt, int typeID, CustomerChanal chanal, string source)
      {
         var point = new CustomerPoint();
         point.Code = item.ConditionCode;
         point.Name = item.Name;
         point.Point = pointamt;
         point.TransacionTypeID = typeID;
         point.CustomerChanal = chanal;
         point.Source = source;
         point.ChannelType = ChannelType.Online;
         point.Create_On = DateUtil.Now();
         point.Create_By = customer.User.UserName;
         point.Update_On = DateUtil.Now();
         point.Update_By = customer.User.UserName;
         return point;
      }
      #endregion

      public string CreateAccountCode()
      {
         var AccountCode = Guid.NewGuid().ToString().Replace("-", "");

         var AccountCodeAlreadyExists = _context.AccountCodes.Where(w => w.Code == AccountCode).FirstOrDefault();

         if (AccountCodeAlreadyExists != null)
         {
            AccountCode = CreateAccountCode();
         }

         return AccountCode;
      }
   }
}
