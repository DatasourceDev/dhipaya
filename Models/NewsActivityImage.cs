using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
  public class NewsActivityImage
  {
    [Key]
    public int ID { get; set; }
    public int NewsActivityID { get; set; }

    public string Url { get; set; }

    public virtual NewsActivity NewsActivity { get; set; }

  }
}
