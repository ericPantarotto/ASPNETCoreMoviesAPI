using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MoviesAPI.DTOs;
using MoviesAPI.Services;
using System;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SecurityController: ControllerBase
    {

        private readonly IDataProtector _protector;
        private readonly IConfiguration configuration;

        private readonly HashService hashService;

        public SecurityController(IDataProtectionProvider protectionProvider,
            IConfiguration configuration,
            HashService hashService
            )
        {
            // _protector = protectionProvider.CreateProtector("value_secret_and_unique");
            _protector = protectionProvider.CreateProtector(configuration["protector_secret"]); //Replacing with an env variable
            this.configuration = configuration;
            this.hashService = hashService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Felipe Gavilán";
            string encryptedText = _protector.Protect(plainText);
            string decryptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("TimeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();

            string plainText = "Felipe Gavilán";
            string encryptedText = protectorTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            await Task.Delay(6000);
            try
            {
                string decryptedText = protectorTimeBound.Unprotect(encryptedText);
                return Ok(new { plainText, encryptedText, decryptedText });
                
            }
            catch (Exception ex)
            {
                return Ok( new { ex.Message });
            }
        }

        [HttpGet("hash")]
        public IActionResult GetHash()
        {
            var plainText = "Felipe Gavilán";
            HashResult hashResult1 = hashService.Hash(plainText);
            HashResult hashResult2 = hashService.Hash(plainText);
            return Ok(new { plainText, hashResult1, hashResult2 });
        }

    }
}
