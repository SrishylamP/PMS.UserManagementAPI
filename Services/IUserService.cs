using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.UserManagementAPI.Models;

namespace PMS.UserManagementAPI.Services
{
    public  interface IUserService
    {
        Task<ResponseMessage> RegisterUser(UserModel model);
        Task<bool> BlockUser(ActivateUserModel model);
        Task<bool> ActivateUser(ActivateUserModel model);        
         Task<RoleModel> GetRolesById(int userId);
        Task<RoleModel> GetRolesByEmail(string email);
        Task<List<UserModel>> GetAllHospitalUsers();
        Task<ResponseMessage> UpdateUser(UserModel model);
        Task<List<UserModel>> GetAllPhysicians();

        Task<List<UserModel>> GetAllPatientUsers();
        Task<List<RoleModel>> GetAllRoles();
        Task<ResponseMessage> ChangePassword(LoginModel model);
        Task<ResponseMessage> ForgotPassword(string email);
    }
}
