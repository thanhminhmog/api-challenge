﻿using BLL.Helpers;
using BLL.Interfaces;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace API.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        #region Constructor that takes GuestLogic, HelpPage

        private IOptions<LoginGuide> _loginGuide;

        public LoginController(IGuestLogic guestLogic,
            IOptions<HelpPage> helpPage,
            IOptions<LoginGuide> login) : base(guestLogic, helpPage)
        {
            _loginGuide = login;
        }

        #endregion Constructor that takes GuestLogic, HelpPage

        [HttpGet]
        public IActionResult Help()
        {
            string login = _loginGuide.Value.Message;
            return Ok(login);
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
        [HttpPost]

        #region RepCode 200 400 404 500

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        #endregion RepCode 200 400 404 500

        public IActionResult Login(string email, string confirmationCode)
        {
            UserLogin user = new UserLogin
            {
                Email = email.ToLower(),
                ConfirmationCode = confirmationCode,
            };
            string response = "";

            //  Check For Bad Inputs
            if (user.Email == null || user.ConfirmationCode == null)
            {
                return BadRequest("Error : Can not get appropriate email or confirmation code");
            }
            if (user.Email.Length <= 0 || user.ConfirmationCode.Length <= 0)
            {
                return BadRequest("Email and ConfimationCode can not be empty");
            }
            if (user.Email.Length <= 8)
            {
                return BadRequest("Email and ConfimationCode must be 8 or more characters");
            }
            if (user.ConfirmationCode.Length < 32)
            {
                return BadRequest("ConfimationCode must be 32 characters we sent you when you registered");
            }

            try
            {
                response = _logic.Login(user);
                //  If token was an empty string, it mean username or password were incorrect
                //  In theory it should not reach this if-block, instead throws ArgumentNullException
                //  This is here just for safety measure
                if (response.Length == 0)
                {
                    return NotFound("Wrong Email or Confimation code");
                }
            }
            //  Exception When User Is Not Found
            catch (ArgumentNullException arn)
            {
                return BadRequest(arn.Message);
            }
            //  Exception When Position Assigned To User Is Not Found
            catch (ArgumentException ar)
            {
                return BadRequest(ar.Message);
            }

            return Ok(response);
        }
    }

    [AllowAnonymous]
    public class RegisterController : BaseController
    {
        #region Constructor that takes GuestLogic, HelpPage

        private IOptions<RegisterGuide> _registerGuide;

        public RegisterController(IGuestLogic logic,
            IOptions<HelpPage> helpPage,
            IOptions<RegisterGuide> register) : base(logic, helpPage)
        {
            _registerGuide = register;
        }

        #endregion Constructor that takes GuestLogic, HelpPage

        [HttpGet]
        public IActionResult Help()
        {
            string loginFormat = _registerGuide.Value.Message;
            return Ok(loginFormat);
        }

        /// <summary>
        /// Register with Email - Phone - FullName - PositionName
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///     {
        ///         "Email" : "name@name.com",
        ///         "Phone" : "123456"
        ///         "FullName" : "John Doe"
        ///         "PositionName" : "Junior"
        ///     }
        /// </remarks>
        /// <returns>ConfirmationCode</returns>
        /// <response code="200">Successfully registered. All info valid.</response>
        /// <response code="400">Invalid Input</response>
        /// <response code="404">Server Denied Access</response>
        /// <response code="500">Server Is Down</response>
        [HttpPost]
        [AllowAnonymous]

        #region RepCode 200 400 404 500

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        #endregion RepCode 200 400 404 500

        public IActionResult Register(UserRegister user)
        {
            //  Input : UserRegister includes :
            //  Email - Phone - FullName - PositionName
            UserLogin userLogin = new UserLogin();
            try
            {
                //  check input from client
                if (user == null || user.PositionName.ToLower() == "admin")
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
            //  Return : UserLogin includes :
            //  Email - ConfirmationCode
            return Ok(userLogin);
        }
    }
}