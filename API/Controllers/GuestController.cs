using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.BussinessLogics;
using BLL.Helpers;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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




        /// <summary>
        /// Register with Email - Phone - FullName - PositionName
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// Sample Request:
        /// 
        /// 
        ///     {
        ///         "Email" : "name@name.com",
        ///         "Phone" : "123456"
        ///         "FullName" : "John Doe"
        ///         "PositionName" : "junior"
        ///     }
        ///     
        /// </remarks>
        /// 
        /// <param name="register">This is param</param>
        /// 
        /// <returns>ConfirmationCode</returns>
        /// 
        /// <response code="200">Successfully registered. All info valid.</response>
        /// <response code="400">Invalid Input</response>
        /// <response code="404">Server Denied Access</response>
        /// <response code="500">Server Is Down</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Register(UserRegister user)
        {
            //  ///////////////////////////////////
            //  Input : UserRegister includes :
            //  Email - Phone - FullName - PositionName
            //  ///////////////////////////////////
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
            catch (NullReferenceException nre)
            {
                return BadRequest(nre.Message + "\n" + nre.StackTrace);
            }
            catch (InvalidOperationException ioe)
            {
                return BadRequest(ioe.Message + "\n" + ioe.StackTrace);
            }
            //  ///////////////////////////////////
            //  Return : UserLogin includes :
            //  Email - ConfirmationCode
            //  ///////////////////////////////////
            return Ok(userLogin);
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin user)
        {
            string email = user.Email;
            string confirmationCode = user.ConfirmationCode;
            string token = "";

            //  Check For Null Inputs
            if (email.Length <= 0 || confirmationCode.Length <= 0)
            {
                return BadRequest("Email and ConfimationCode can not be empty");
            }
            try
            {
                token = _logic.Login(user);
                //  ///////////////////////////////////
                //  If token was an empty string, it mean username or password were incorrect
                //  In theory it should not reach this if-block, and throws ArgumentNullException instead
                //  This is here just for safety measure
                //  ///////////////////////////////////
                if (token.Length == 0)
                {
                    return NotFound("Wrong Email or Confimation code");
                }
            }
            //  ///////////////////////////////////
            //  Exception When User Is Not Found
            //  ///////////////////////////////////
            catch (ArgumentNullException arn)
            {
                return BadRequest(arn.Message + "\n" + arn.StackTrace);
            }
            //  ///////////////////////////////////
            //  Exception When Position Assigned To User Is Not Found
            //  ///////////////////////////////////
            catch (ArgumentException ar)
            {
                return BadRequest(ar.Message + "\n" + ar.StackTrace);
            }

            return Ok(token);
        }
    }
}