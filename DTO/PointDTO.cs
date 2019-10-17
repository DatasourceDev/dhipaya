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
   public class PointConditionDTO : ModelBaseDTO
   {
      public IEnumerable<PointCondition> PointConditions { get; set; }
   }
}
