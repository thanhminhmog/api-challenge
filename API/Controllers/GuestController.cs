﻿using BLL.BussinessLogics;
using BLL.Helpers;
using BLL.Models;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// View Help page
        /// </summary>
        /// <returns>Help page</returns>
        /// <response code="200">Help page</response>
        /// <response code="500">Internal Error</response>
        [HttpGet("help")]
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

        /// <summary>
        /// Register a User
        /// </summary>
        /// <returns>User's Email and User's Confirmation Code</returns>
        /// <response code="200">User Registed</response>
        /// <response code="400">Not have enough infomation</response>
        /// <response code="500">Internal Error</response>
        [HttpPost("register")]
        #region RepCode 200 400 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
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

        /// <summary>
        /// Login to get Access Token
        /// </summary>
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(confirmationCode))
            {
                return BadRequest("Email and ConfimationCode can not be empty");
            }
            if (email.Length <= 8)
            {
                return BadRequest("Email and ConfimationCode must be 8 or more characters");
            }

            if (confirmationCode.Length < 32)
            {
                return BadRequest("ConfimationCode must be 32 characters we send you when you register");
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