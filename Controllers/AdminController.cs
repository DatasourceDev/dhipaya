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
using System.Text;
using Dhipaya.Extensions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Xml;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Dhipaya.ModelsDapper;

namespace Dhipaya.Controllers
{
   public class AdminController : ControllerBase
   {
     
      public readonly ILoggerFactory _loggerFactory;

      public AdminController(ICustomerRepository cusRepo, IReportRepository rptRepo,ChFrontContext context, ILoginServices loginServices, ILogger<AdminController> logger, IOptions<SystemConf> conf, IOptions<Smtp> smtp, IOptions<IIA> _iia, IOptions<TIPMobile> _mobile) : base(context, logger, _mobile, _iia, smtp, loginServices, conf, cusRepo, rptRepo)
      {
         this._logger = logger;
         this._context = context;
         this._loginServices = loginServices;
         this._smtp = smtp.Value;
         this._iia = _iia.Value;
         this._mobile = _mobile.Value;
         this._conf = conf.Value;
         this._rptRepo = rptRepo;
         this._cusRepo = cusRepo;
      }

      public IActionResult Index()
      {
         return View();
      }

   }
}
