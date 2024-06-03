using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yarl_Vibe_api.Models;

namespace Yarl_Vibe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(SignInManager<IdentityUser> sm, UserManager<IdentityUser> um) : ControllerBase
    {
        private readonly SignInManager<IdentityUser> signInManager = sm;
        private readonly UserManager<IdentityUser> userManager = um;

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(Login login)
        {
            string message = "";

            try
            {
                IdentityUser user_ = await userManager.FindByNameAsync(login.Username);

                var result = await signInManager.PasswordSignInAsync(user_, login.Password, login.Remember, false);

                if(!result.Succeeded)
                {
                    return Unauthorized("Check your login credential and try again");
                }

                message = "Login successfully";

            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, Please try again. " + ex.Message);
            }

            return Ok(new { message = message });
        }

        [HttpGet("logout"), Authorize]
        public async Task<ActionResult> LogoutUser()
        {
            string message = "You are free to go!";
            try
            {
                await signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, please try again. " + ex.Message);
            }

            return Ok(new { message = message });
        }
    }
}
