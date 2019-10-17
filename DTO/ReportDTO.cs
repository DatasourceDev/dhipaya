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
   
   public class ReportDTO : ModelReportBaseDTO
   {
      public IEnumerable<Privilege> PrivilegeRanks { get; set; }
      public IEnumerable<Redeem> Redeems { get; set; }
      public IEnumerable<Contact> Contacts { get; set; }
      public IEnumerable<Subscriber> Subscribers { get; set; }
      public IEnumerable<CustomerPoint> CustomerPoints { get; set; }
      public IEnumerable<Customer> RedeemRanks { get; set; }
      public IEnumerable<Customer> InviteRanks { get; set; }
      public IEnumerable<CustomerClassChange> CustomerClassChanges { get; set; }
      public IEnumerable<IIAExpireDTO> IIAExpires { get; set; }
   }
}
