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
    public class LoginController : ControllerBase
    {

        private IUserService _userService;
        private ILoggerManager _loggerManager;
        private IMessageService _messageService;

        public LoginController(IUserService userService, ILoggerManager loggerManager, IMessageService messageService)
        {
            this._userService = userService;
            this._messageService = messageService;
            this._loggerManager = loggerManager;
        }

        /*        // GET: api/<LoginController>
                [HttpGet]
                public IEnumerable<string> Get()
                {
                    return new string[] { "value1", "value2" };
                }*/

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public long Get([FromBody] UserRequest req)
        {

            return 0;
        }

        // POST api/<LoginController>
/*        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
