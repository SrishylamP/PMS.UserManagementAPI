using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManagementAPI.Models
{
  public  class ActivateUserModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
    }
}
