using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManagementAPI
{
    public static class Constants
    {
        public const string EmailAndEmpIdExists = "Email Id and  Employee Id are  already Exists";
        public const string DuplicateEmpIdExists = "Duplicate Employee ID exists. Please contact the helpdesk or call on 123456 for more information.";
        public const string EmailAlreadyRegistered = "Email is already registered";       
        public const string UpdatedSuccessfully = "Updated successfully";
        public const string OldPasswordIsInvalid = "Old password is invalid";
        public const string NewPasswordOldPasswordSame = "Your old password and new password are same.";
        public const string PasswordChangedSuccessfully = "Password Changed successfully";

        //Audit operations
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";

    }
}
