using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
  public class UserRole
  {
    [Key]
    public int UserRoleID { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public bool UnEditable { get; set; }
    public StatusType Status { get; set; }
    public string Create_By { get; set; }
    public DateTime? Create_On { get; set; }
    public string Update_By { get; set; }
    public DateTime? Update_On { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<PageRole> PageRoles { get; set; }

    [NotMapped]
    public IList<PageRole> PageRoleList
    {
      get
      {
        return this.PageRoles != null ? this.PageRoles.ToList() : null;
      }
      set
      {
        this.PageRoles = value;
      }
    }
  }

  public class Page
  {
    [Key]
    public int PageID { get; set; }

    public string PageCode { get; set; }
    public string PageName { get; set; }
    public string Description { get; set; }
  }



  public class PageRole
  {
    [Key]
    public int ID { get; set; }

    public int UserRoleID { get; set; }
    public int PageID { get; set; }
    //public virtual UserRole UserRole { get; set; }
    //public virtual Page Page { get; set; }

  }

}
