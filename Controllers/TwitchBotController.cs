using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using twitchbot.Bot;
using twitchbot.Models;

namespace twitchbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitchBotController : ControllerBase
    {
        private IConfiguration configuration;
        private IBot mBot;

        public TwitchBotController(IConfiguration iConfig, IBot bot){
            configuration = iConfig;
            mBot = bot;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("addUser")]
        public ActionResult<string> AddUser(User user){
            mBot.AddClient(user.userName);
            return Ok("user added");
        }

        [HttpPost("removeUser")]
        public ActionResult<string> RemoveUser(User user){
            mBot.RemoveClient(user.userName);
            return Ok("user removed");
        }
    }
}
