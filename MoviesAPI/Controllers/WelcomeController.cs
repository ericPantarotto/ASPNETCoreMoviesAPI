using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    public class WelcomeController : Controller
    {
        [HttpGet]
        public string Welcome()
        {
            string[] words = { "Welcome to our Movies API\r\n\r\n " ,
            "         __________________________\r\n        /   | |______| |___     __/ \r\n       /  , | |  /\\  | | ^ |   |       ,--.  \r\n     ,' ,'| | |.'  `.| |/ \\|   |      /    \\  \r\n   ,' ,'__| | |______| |___|   |      \\    /    \r\n  /         |          |   |   |     _ `--'      \r\n  [   ,--.  |          |,--|   |]   (_) \r\n  |__/    \\_|__________/    \\__|= o \r\n     \\    /            \\    /\r\n   `--'              `--'\r\n\r\n",
            $"©ECR - {DateTime.Now.Year}"};
            string welcomeMessage = string.Join(" ", words);
            return welcomeMessage;
        }
    }
}
