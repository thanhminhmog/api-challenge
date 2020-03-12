using BLL.BussinessLogics;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("challenges")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        /// <summary>
        /// View List of Challeges of User's position
        /// </summary>
        /// <returns>List of challenges</returns>
        /// <response code="200">List of challenge</response>
        /// <response code="400">Not have enough infomation</response>
        /// <response code="401">Unauthorize</response>
        /// <response code="404">Empty challenge list</response>
        /// <response code="500">Internal Error</response>
        [HttpGet]
        #region RepCode 200 400 401 404 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult ViewChallengeList()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            UserProfile userProfile = new UserProfile();
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                userProfile.Email = claims.FirstOrDefault(c => c.Type == "user_email").Value;
                userProfile.PositionName = claims.FirstOrDefault(c => c.Type == "position").Value;
            }
            if (userProfile.Email == null || userProfile.PositionName == null)
            {
                return BadRequest("User missing some vital infomation to use this feature");
            }

            var challenges = _userLogic.ViewChallengesList(userProfile);
            if (challenges == null)
            {
                return NotFound("There are no Challenge for this position");
            }
            return Ok(challenges);
        }

        /// <summary>
        /// View Challenge's context
        /// </summary>
        /// <returns>Challenge's context</returns>
        /// <response code="200">Challenge Description</response>
        /// <response code="400">Not have enough infomation</response>
        /// <response code="401">Unauthorize</response>
        /// <response code="403">User's position different from challenge's position</response>
        /// <response code="500">Internal Error</response>
        [HttpGet("{id}")]
        #region RepCode 200 400 401 403 500
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        #endregion
        public IActionResult GetChallengeDetail(Guid id)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            UserProfile userProfile = new UserProfile();
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                userProfile.PositionName = claims.FirstOrDefault(c => c.Type == "position").Value;
            }

            if (userProfile.PositionName == null)
            {
                return BadRequest("User not have pemission to access to this challenge");
            }

            var chal = _userLogic.ViewChallengeContent(id);

            if (!userProfile.PositionName.Equals(chal.PositionName))
            {
                return Forbid("Permission denied");
            }
            ChallengeInfoView challenge = new ChallengeInfoView
            {
                ChallengeName = chal.ChallengeName,
                ChallengeDescription = chal.ChallengeDescription
            };
            return Ok(challenge);
        }


    }
}