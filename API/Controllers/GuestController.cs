using BLL.BussinessLogics;
using BLL.Helpers;
using BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace API.Controllers
{
    [Route("")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        #region objects and constructors
        private IGuestLogic _logic;
        protected readonly IOptions<HelpPage> _helpPage;
        protected readonly IOptions<IndexPage> _indexPage;

        public GuestController(IGuestLogic guestLogic, IOptions<HelpPage> helpPage, IOptions<IndexPage> indexPage)
        {
            _logic = guestLogic;
            _helpPage = helpPage;
            _indexPage = indexPage;
        }
        #endregion


        [HttpGet]
        public IActionResult Guest()
        {
            var index = _indexPage.Value;
            return Ok(index.Message);
        }

        [HttpGet("help")]
        public IActionResult Help()
        {
            var helplist = _helpPage.Value;
            string contentList = string.Join("\n\n", helplist.ContentList.ToArray());

            return Ok(helplist.Header + "\n\n"
                + helplist.Guildline + "\n\n"
                + contentList.ToString());
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegister user)
        {
            //  Input : UserRegister includes :
            //  Email - Phone - FullName - PositionName
            //  ///////////////////////////////////
            //  Return : UserLogin includes :
            //  Email - ConfirmationCode
            UserLogin userLogin = new UserLogin();
            try
            {
                //  check input from client
                if (user == null)
                {
                    return BadRequest("Invalid Input");
                }
                //  initiate register function
                userLogin = _logic.Register(user);
                //  check if register functions successfully
                if (userLogin == null)
                {
                    return BadRequest("Register Failed");
                }
            }
            //  GuestLogic received UserRegister = null
            catch (ArgumentNullException arn)
            {
                return BadRequest(arn.Message + "\n" + arn.StackTrace);
            }
            //  GuestLogic - Position Applied Not Available 
            catch (ArgumentException ar)
            {
                return BadRequest(ar.Message + "\n" + ar.StackTrace);
            }
            return Ok(userLogin);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string confirmationCode)
        {
            UserLogin user = new UserLogin
            {
                Email = email,
                ConfirmationCode = confirmationCode,
            };

            //  Check For Null Inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(confirmationCode))
            {
                return BadRequest("Email and ConfimationCode can not be empty");
            }
            if (email.Length <= 8 || confirmationCode.Length <= 8)
            {
                return BadRequest("Email and ConfimationCode must be 8 or more characters");
            }

            string token;
            try
            {
                token = _logic.Login(user);
                //  If token was an empty string, it mean username or password were incorrect
                //  In theory it should not reach this if-block, and throws ArgumentNullException instead
                //  This is here just for safety measure
                if (token.Length == 0)
                {
                    return NotFound("Wrong Email or Confimation code");
                }
            }
            //  Exception When User Is Not Found
            catch (ArgumentNullException arn)
            {
                return BadRequest(arn.Message + "\n" + arn.StackTrace);
            }
            //  Exception When Position Assigned To User Is Not Found
            catch (ArgumentException ar)
            {
                return BadRequest(ar.Message + "\n" + ar.StackTrace);
            }

            return Ok(token);
        }
    }
}