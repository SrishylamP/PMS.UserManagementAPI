using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMS.UserManagementAPI.Data;
using PMS.UserManagementAPI.Models;

namespace PMS.UserManagementAPI.Services
{
    public class UserDbService:IUserService
    {
        private readonly PMSDbContext _context;

        public UserDbService(PMSDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseMessage> RegisterUser(UserModel model)
        {
            try
            {
                var resObj = new ResponseMessage();
                var objEmail = _context.Users.FirstOrDefault((e) => e.Email == model.Email);
                var EmpId = _context.Users.FirstOrDefault((e) => e.EmployeeId == model.EmployeeId);

                if (objEmail != null && EmpId != null)
                {
                    resObj.message = Constants.EmailAndEmpIdExists;

                }
                else if (EmpId != null)
                {
                    resObj.message = Constants.DuplicateEmpIdExists;
                }
                else if (objEmail != null)
                {
                    resObj.message = Constants.EmailAlreadyRegistered;
                }
                else
                {

                    User user = new User();
                    user.Title = model.Title;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Dob = model.DOB.ToLocalTime();
                    user.EmployeeId = model.EmployeeId;
                    user.Password = Common.CreateMD5(model.Password);
                    user.NoOfWrongAttempts = model.NoOfWrongAttempts;
                    user.RoleId = model.RoleId;
                    user.IsActive = true;
                    user.IsFirstTimeUser = true;
                    user.CreatedDate = DateTime.Now;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    var audit = new AuditModel
                    {
                        Operation = Constants.Create,
                        ObjectName = "Hospital User",
                        Description = $"Hospital User Created with Id: {user.UserId}",
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                    resObj.IsSuccess = true;
                    resObj.message = "Registration successful";
                }
                if (resObj.IsSuccess == true)
                {
                    bool f = Common.SendDefaultPasswordEmail(Constants.FromEmail, Constants.RegisterSubject, Constants.RegisterMessage, model.Email, model.Password);
                }
                return resObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> BlockUser(ActivateUserModel model)
        {
            try
            {
                bool isUnlocked = false;
                var obj = _context.Users.FirstOrDefault(e => e.Email == model.Email);
                if (obj != null)
                {
                    obj.IsBlocked = !model.IsBlocked;
                    obj.IsActive = false;
                    await _context.SaveChangesAsync();
                    isUnlocked = true;
                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Hospital User",
                        Description = $"Hospital User Blocked Id: {obj.UserId}",
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.Now
                    };
                   AuditMe(audit);
                }
                else
                {
                    isUnlocked = false;

                }
                if (isUnlocked)
                {
                   
                    Common.SendEmail(obj.FirstName, Constants.FromEmail, model.Email, Constants.BlockedSubject, Constants.BlockedMessage);
                }
                return isUnlocked;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RoleModel> GetRolesById(int userId)
        {
            var obj = await (from u in _context.Users
                             join r in _context.UserRoles
                             on u.RoleId equals r.RoleId
                             where u.UserId == userId
                             select new RoleModel { Id = r.RoleId, Name = r.RoleName }).FirstOrDefaultAsync();
            return obj;
        }
        public async Task<RoleModel> GetRolesByEmail(string email)
        {
            var obj = await (from u in _context.Users
                             join r in _context.UserRoles
                             on u.RoleId equals r.RoleId
                             where u.Email == email
                             select new RoleModel { Id = r.RoleId, Name = r.RoleName }).FirstOrDefaultAsync();
            return obj;
        }
        public async Task<List<UserModel>> GetAllHospitalUsers()
        {
            var list = await (from u in _context.Users
                              join r in _context.UserRoles
                              on u.RoleId equals r.RoleId
                              where u.RoleId != 4
                              select new UserModel
                              {
                                  Title = u.Title,
                                  UserId = u.UserId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Phone = u.Phone,
                                  DOB = u.Dob,
                                  EmployeeId = u.EmployeeId,
                                  NoOfWrongAttempts = u.NoOfWrongAttempts,
                                  Role = r.RoleName,
                                  RoleId = r.RoleId,
                                  IsActive = u.IsActive,
                                  IsBlocked = u.IsBlocked,
                                  Status = u.IsBlocked ? "Blocked" : u.IsActive ? "Active" : "InActive",
                                  CreatedDate = u.CreatedDate
                              }).ToListAsync();
            return list;
        }

        public async Task<bool> ActivateUser(ActivateUserModel model)
        {
            try
            {
              
                bool isUpdated = false;
                var obj = _context.Users.FirstOrDefault(e => e.Email == model.Email);
                if (model.IsBlocked || (!model.IsBlocked && !model.IsActive))
                {

                    var password = Common.GeneratePassword();
                    //string firstName = _logindbService.UserDetail(model.Email);
                    Common.SendEmail(obj.FirstName, Constants.FromEmail, model.Email, Constants.ActivateUserSubject, Constants.ActivateUserMessage, null, password);

                    await DefaultForgotPassword(model.Email, Common.CreateMD5(password));
                }
                if (obj != null)
                {
                    obj.IsActive = model.IsActive;
                    obj.IsBlocked = false;
                    obj.IsFirstTimeUser = true;
                    obj.NoOfWrongAttempts = 0;
                    await _context.SaveChangesAsync();
                    isUpdated = true;

                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Hospital User",
                        Description = $"Hospital User Activated: {obj.UserId}",
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                }
                else
                {
                    isUpdated = false;

                }
               
                return isUpdated;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseMessage> UpdateUser(UserModel model)
        {
            try
            {
                var resObj = new ResponseMessage();
                var obj = _context.Users.FirstOrDefault((e) => e.UserId == model.UserId);

                if (obj != null)
                {
                    obj.FirstName = model.FirstName;
                    obj.LastName = model.LastName;
                    obj.Phone = model.Phone;
                    obj.RoleId = model.RoleId;
                    //obj.EmployeeId = model.EmployeeId;
                    await _context.SaveChangesAsync();
                    resObj.IsSuccess = true;
                    resObj.message = Constants.UpdatedSuccessfully;

                    var audit = new AuditModel
                    {
                        Operation = Constants.Update,
                        ObjectName = "Hospital User",
                        Description = $"Hospital User Updated Id: {obj.UserId}",
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.Now
                    };
                    AuditMe(audit);
                }
                return resObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserModel>> GetAllPhysicians()
        {
            var list = await (from u in _context.Users
                              join r in _context.UserRoles
                              on u.RoleId equals r.RoleId
                              where u.RoleId == 2
                              select new UserModel
                              {
                                  Title = u.Title,
                                  UserId = u.UserId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Phone = u.Phone,
                                  DOB = u.Dob,
                                  EmployeeId = u.EmployeeId,
                                  NoOfWrongAttempts = u.NoOfWrongAttempts,
                                  Role = r.RoleName,
                                  RoleId = r.RoleId,
                                  IsActive = u.IsActive,
                                  IsBlocked = u.IsBlocked,
                                  Status = u.IsBlocked ? "Blocked" : u.IsActive ? "Active" : "InActive",
                                  CreatedDate = u.CreatedDate
                              }).ToListAsync();
            return list;
        }
        public async Task<List<UserModel>> GetAllPatientUsers()
        {
            var list = await (from u in _context.Users
                              join r in _context.UserRoles
                              on u.RoleId equals r.RoleId
                              join pd in _context.PatientDetails
                              on u.UserId equals pd.UserId
                              where u.RoleId == 4
                              select new UserModel
                              {
                                  Title = u.Title,
                                  UserId = u.UserId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Phone = u.Phone,
                                  DOB = u.Dob,
                                  EmployeeId = u.EmployeeId,
                                  NoOfWrongAttempts = u.NoOfWrongAttempts,
                                  Role = r.RoleName,
                                  RoleId = r.RoleId,
                                  IsActive = u.IsActive,
                                  IsBlocked = u.IsBlocked,
                                  Status = u.IsBlocked ? "Blocked" : u.IsActive ? "Active" : "InActive",
                                  CreatedDate = u.CreatedDate,
                                  Patient = u.PatientDetails.Select(p => new PatientModel
                                  {
                                      PatientId = p.PatientId
                                  }).ToList()
                              }).ToListAsync();
            return list;
        }
        public void AuditMe(AuditModel model)
        {
            var aObj = new Audit
            {
                Operation = model.Operation,
                Description = model.Description,
                ObjectName = model.ObjectName,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };
            _context.Audits.Add(aObj);
             _context.SaveChanges();
        }
        public async Task<List<RoleModel>> GetAllRoles()
        {
            var list = await _context.UserRoles.Select(e => new RoleModel { Id = e.RoleId, Name = e.RoleName }).ToListAsync();
            return list;
        }
        public async Task<ResponseMessage> ChangePassword(LoginModel model)
        {
            try
            {
                var resObj = new ResponseMessage();

                var userObj = await _context.Users.Where(e => e.Email == model.Email).FirstOrDefaultAsync();
                if (userObj.Password == Common.CreateMD5(model.OldPassword))
                {
                    if (userObj.Password != Common.CreateMD5(model.Password))
                    {
                        userObj.Password = Common.CreateMD5(model.Password);
                        userObj.IsFirstTimeUser = false;
                        await _context.SaveChangesAsync();
                        resObj.IsSuccess = true;
                        resObj.message = Constants.PasswordChangedSuccessfully;
                        var audit = new AuditModel
                        {
                            Operation = Constants.Update,
                            ObjectName = "User",
                            Description = $"Password is changed for the userId: {userObj.UserId}",
                            CreatedBy = userObj.UserId,
                            CreatedDate = DateTime.Now
                        };
                        AuditMe(audit);
                    }
                    else
                    {
                        resObj.IsSuccess = false;
                        resObj.message = Constants.NewPasswordOldPasswordSame;
                    }
                }
                else
                {
                    resObj.IsSuccess = false;
                    resObj.message = Constants.OldPasswordIsInvalid;
                }

                return resObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseMessage> ForgotPassword(string email)
        {
            string para = "You recently requested to reset your password for your account. Click the link below to reset it.";
            string from = "sendtestmail15@gmail.com";
            string sub = "Password Reset Request";
            string msg = "Your one time password is : ";
            string message = "Reset password link has been sent to your email id.";
            string mesg = "Please enter your registered email id.";
            var password = Common.GeneratePassword();
            string firstName = UserDetail(email);
            string htmlString = "<h3>" + "Hi, " + firstName + "</h3>" + "<p style='font-size:15px;'>" + para + " </p>" + "<p style='font-size:20px;'>" + msg + password + " </p>";
            ResponseMessage response = new ResponseMessage();

            var exist = await IsUserExist(email);

            if (exist)
            {
                bool f = Common.SendEmail(firstName, from, email, sub, msg, htmlString);
                if (f)
                {
                    var resp = await DefaultForgotPassword(email, Common.CreateMD5(password));
                    response.IsSuccess = true;
                    response.message = message;
                }

            }
            else
            {
                response.IsSuccess = false;
                response.message = mesg;
            }
            return response;

        }
        public async Task<bool> IsUserExist(string email)
        {
            try
            {
                var userobj = await _context.Users.Where(e => e.Email == email && e.IsActive).FirstOrDefaultAsync();
                if (userobj != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string UserDetail(string email)
        {
            try
            {
                var userobj = _context.Users.Where(e => e.Email == email && e.IsActive).FirstOrDefault();
                if (userobj != null)
                    return userobj.FirstName.ToString();
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DefaultForgotPassword(string email, string password)
        {
            try
            {
                var userObj = await _context.Users.Where(e => e.Email == email).FirstOrDefaultAsync();

                userObj.Password = password;
                userObj.IsFirstTimeUser = true;
                await _context.SaveChangesAsync();
                return true;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
