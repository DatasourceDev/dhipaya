using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
  public class MerchantCategory
  {
    [Key]
    public int CategoryID { get; set; }

    public string CategoryName { get; set; }
    public string RedWord { get; set; }
    public StatusType Status { get; set; }
    public string Description { get; set; }
    public string Logo { get; set; }
    public string Url { get; set; }
    public int? Index { get; set; }

    public string Create_By { get; set; }
    public DateTime? Create_On { get; set; }
    public string Update_By { get; set; }
    public DateTime? Update_On { get; set; }
    public virtual ICollection<Merchant> Merchants { get; set; }
  }
}
