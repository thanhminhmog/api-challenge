using BLL.BussinessLogics;
using BLL.Helpers;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace API.Controllers
{

    [Route("start")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        protected readonly IOptions<IndexPage> _indexPage;

        public WelcomeController(IOptions<IndexPage> indexPage)
        {
            _indexPage = indexPage;
        }


        /// <summary>
        /// View Start page
        /// </summary>
        /// <returns>Start page</returns>
        /// <response code="200">Start page</response>
        /// <response code="500">Internal Error</response>
        [HttpGet]
        #region RepCode 200 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult Guest()
        {
            var index = _indexPage.Value;
            return Ok(index.Message);
        }
    }


    [Route("help")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        protected readonly IOptions<HelpPage> _helpPage;

        public HelpController(IOptions<HelpPage> helpPage)
        {
            _helpPage = helpPage;
        }


        /// <summary>
        /// View Help page
        /// </summary>
        /// <returns>Help page</returns>
        /// <response code="200">Help page</response>
        /// <response code="500">Internal Error</response>
        [HttpGet]
        #region RepCode 200 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult Help()
        {
            var helplist = _helpPage.Value;
            string contentList = string.Join("\n\n", helplist.ContentList.ToArray());

            return Ok(helplist.Header + "\n\n"
                + helplist.Guildline + "\n\n"
                + contentList.ToString());
        }


        [HttpGet("login")]
        public IActionResult LoginHelp()
        {
            string registerFormat = "POST /Login" +
                "\n{" +
                "\n    \"email\": \"string\"" +
                "\n    \"confirmationCode\": \"string\"" +
                "\n}";
            return Ok(registerFormat);
        }



        [HttpGet("register")]
        public IActionResult RegisterHelp()
        {
            string loginFormat = "Positions we are hiring : Junior, Mid-level, Senior" +
                "\n\nPOST /Register" +
                "\n{" +
                "\n    \"phone\": \"string\"" +
                "\n    \"fullName\": \"string\"" +
                "\n    \"positionName\": \"string\"" +
                "\n    \"email\": \"string\"" +
                "\n}";
            return Ok(loginFormat);
        }
    }



    [Route("")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        #region objects and constructors
        private IGuestLogic _logic;

        public GuestController(IGuestLogic guestLogic)
        {
            _logic = guestLogic;
        }
        #endregion




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
        ///         "PositionName" : "Junior"
        ///     }
        ///     
        /// </remarks>
        /// 
        /// 
        /// 
        /// <returns>ConfirmationCode</returns>
        /// 
        /// <response code="200">Successfully registered. All info valid.</response>
        /// <response code="400">Invalid Input</response>
        /// <response code="404">Server Denied Access</response>
        /// <response code="500">Server Is Down</response>
        [HttpPost("register")]
        [AllowAnonymous]
        #region RepCode 200 400 404 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult Register(UserRegister user)
        {
            //  Input : UserRegister includes :
            //  Email - Phone - FullName - PositionName
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





        /// <summary>
        /// Login to get Access Token
        /// </summary>
        /// <remarks>
        /// 
        /// Sample Request:
        /// 
        /// 
        ///     {
        ///         "email" : "name@name.com"
        ///         "confirmationCode" : "string"
        ///     }
        ///     
        /// </remarks>
        /// <returns>Access token</returns>
        /// <response code="200">Logged in</response>
        /// <response code="400">Not have enough infomation</response>
        /// <response code="404">User Not Exist</response>
        /// <response code="500">Internal Error</response>
        [HttpPost("login")]
        #region RepCode 200 400 404 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult Login(string email, string confirmationCode)
        {
            UserLogin user = new UserLogin
            {
                Email = email.ToLower(),
                ConfirmationCode = confirmationCode,
            };

            //  Check For Null Inputs

            string token = "";

            //  Check For Null Inputs
            if (user.Email.Length <= 0 || user.ConfirmationCode.Length <= 0)
            {
                return BadRequest("Email and ConfimationCode can not be empty");
            }
            if (email.Length <= 8)
            {
                return BadRequest("Email and ConfimationCode must be 8 or more characters");
            }

            if (confirmationCode.Length < 32)
            {
                return BadRequest("ConfimationCode must be 32 characters we sent you when you registered");
            }


            try
            {
                token = _logic.Login(user);
                //  ///////////////////////////////////
                //  If token was an empty string, it mean username or password were incorrect
                //  In theory it should not reach this if-block, instead throws ArgumentNullException
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