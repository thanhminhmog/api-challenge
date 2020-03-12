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

        [HttpGet]
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

        [HttpGet("{id}")]
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