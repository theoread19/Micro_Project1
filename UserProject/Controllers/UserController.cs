using Domain.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserProject.DTOs.Request;
using UserProject.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoggerManager _loggerManager;
        private readonly IMessageService _messageService;
        public UserController(IUserService userService, ILoggerManager loggerManager, IMessageService messageService)
        {
            this._userService = userService;
            this._messageService = messageService;
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
            catch (Exception e)
            {
                throw new Exception("Exception while creating a news to the storage." + e);
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

        [HttpGet("Message/sender={id}")]
        public List<MessageRequest> GetMessagesBySenderId(long id)
        {
            try
            {
                this._loggerManager.LogInfo("Fetching all the message have sender id=" + id + " from the storage");
                return this._messageService.GetMessageBySenderId(id);
            }
            catch (Exception)
            {
                throw new Exception("Exception while fetching all the message have sender id=" + id);
            }

        }

        [HttpPost("Message")]
        public void SendMessage(MessageRequest req)
        {
            try
            {
                this._loggerManager.LogInfo("Sending a message...");
                this._messageService.SendMessage(req);
            }
            catch(Exception)
            {
                throw new Exception("Exception while Sending a message");
            }
            
        }

        [HttpPut("Message")]
        public void ModifyMessage(MessageRequest req)
        {
            try
            {
                this._loggerManager.LogInfo("Modifying the message...");
                this._messageService.ModifiyMessage(req);
            }
            catch (Exception)
            {
                throw new Exception("Exception while modifying the message");

            }
            
        }

        [HttpDelete("Message/id={id}")]
        public void RemoveMessage(long id)
        {
            try
            {
                this._loggerManager.LogInfo("Removing the message...");
                this._messageService.RemoveMessage(id);

            }
            catch (Exception)
            {
                throw new Exception("Exception while removing the message");
            }
        }    
    }
}
