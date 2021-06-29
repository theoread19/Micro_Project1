using Domain.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Request;
using UserProject.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private ILoggerManager _loggerManager;
        public UserController(IUserService userService, ILoggerManager loggerManager)
        {
            this._userService = userService;
            this._loggerManager = loggerManager;
        }
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<List<UserRequest>> GetAllUser()
        {
            try
            {
                this._loggerManager.LogInfo("Fetching all the user from the storage");
                return this._userService.GetAll();
            }
            catch (Exception)
            {
                throw new Exception("Exception while fetching all the user from the storage.");
            }
           
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public UserRequest GetUserById(long id)
        {
            try
            {
                this._loggerManager.LogInfo("Fetching a user from the storage");
                return this._userService.GetById(id);
            }
            catch (Exception)
            {
                throw new Exception("Exception while fetching a user from the storage.");
            }
            
        }

        // POST api/<UserController>
        [HttpPost]
        public void CreateUser([FromBody] UserRequest req)
        {
            try
            {
                this._loggerManager.LogInfo("Creating a user to the storage");
                this._userService.Insert(req);
                
            }
            catch (Exception)
            {
                throw new Exception("Exception while creating a news to the storage.");
            }
            
        }

        // PUT api/<UserController>/
        [HttpPut]
        public void UpdateUser([FromBody] UserRequest req)
        {
            try
            {
                this._loggerManager.LogInfo("Updating a user to the storage");
                this._userService.Update(req);
            }
            catch (Exception)
            {
                throw new Exception("Exception while updating a user to the storage.");
            }
            
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void DeleteUser(long id)
        {
            try
            {
                this._loggerManager.LogInfo("Deleting a the user to the storage");
                this._userService.Delete(id);
            }
            catch (Exception)
            {
                throw new Exception("Exception while deleting a user to the storage.");
            }
           
        }
    }
}
