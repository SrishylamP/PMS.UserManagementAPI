using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.UserManagementAPI.Models;
using PMS.UserManagementAPI.Services;

namespace PMS.UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }
        [HttpPost]
        [Route("RegisterUser")]
        [Authorize]
        public async Task<IActionResult> RegisterUser([FromBody] UserModel model)
        {
            try
            {
                var result = await this.userService.RegisterUser(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpPost]
        [Route("BlockUser")]
        [Authorize]
        public async Task<IActionResult> BlockUser(ActivateUserModel model)
        {
            try
            {
                var result = await this.userService.BlockUser(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpPost]
        [Route("ActivateUser")]
        [Authorize]
        public async Task<IActionResult> ActivateUser(ActivateUserModel model)
        {
            try
            {
                var result = await this.userService.ActivateUser(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpGet]
        [Route("GetRolesById")]
        [Authorize]
        public async Task<IActionResult> GetRolesById(int userId)
        {
            try
            {
                var result = await this.userService.GetRolesById(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpGet]
        [Route("GetRolesByEmail")]
        [Authorize]
        public async Task<IActionResult> GetRolesByEmail([FromQuery] string email)
        {
            try
            {
                var result = await this.userService.GetRolesByEmail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpGet]
        [Route("GetAllHospitalUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllHospitalUsers()
        {
            try
            {
                var result = await this.userService.GetAllHospitalUsers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel model)
        {
            try
            {
                var result = await this.userService.UpdateUser(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }

        }
        [HttpGet]
        [Route("GetAllPhysicians")]
        [Authorize]
        public async Task<IActionResult> GetAllPhysicians()
        {
            try
            {
                var result = await this.userService.GetAllPhysicians();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error,ex,ex.Message);
                return BadRequest(ex);

            }
        }
        [HttpGet]
        [Route("GetAllPatientUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllPatientUsers()
        {
            try
            {
                var result = await this.userService.GetAllPatientUsers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }
        [HttpGet]
        [Route("GetAllRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var result = await userService.GetAllRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] LoginModel model)
        {
            try
            {
                var result = await userService.ForgotPassword(model.Email);
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest("Email not send");
            }


        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] LoginModel model)
        {
            try
            {
                var result = await userService.ChangePassword(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex.Message);
                return BadRequest(ex);

            }

        }
    }
}
