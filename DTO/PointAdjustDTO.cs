using Dhipaya.Extensions;
using Dhipaya.Models;
using Dhipaya.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Dhipaya.DTO
{
   public class PointAdjustDTO : ModelBaseDTO
   {
      public bool valid { get; set; }
      public int result { get; set; }
      public IList<PointAdjust> PointAdjusts { get; set; }
      public IList<PointAdjustFail> PointAdjustFails { get; set; }

   }
   public class PointAdjustFail
   {
      public object username { get; set; }
      public object conditioncode { get; set; }
      public string message { get; set; }
      public int row { get; set; }

   }
}
