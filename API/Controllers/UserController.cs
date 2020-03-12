using BLL.BussinessLogics;
using BLL.Helpers;
using BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        protected readonly IOptions<AppSetting> _options;
        public UserController(IUserLogic userLogic, IOptions<AppSetting> options)
        {
            _userLogic = userLogic;
            _options = options;
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

        /// <summary>
        /// Upload Cv file 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("cv")]
        public async Task<IActionResult> UploadCV(IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest("please provide valid file");
            }
            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .TrimStart().ToString();
            var folderName = Request.Form.ContainsKey("folder") ? Request.Form["folder"].ToString() : null;
            bool status;
            using (var fileStream = file.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                status = await _userLogic.WritingAnObjectAsync(ms, fileName, folderName);
            }
            return status ? Ok("success")
                          : StatusCode((int)HttpStatusCode.InternalServerError, $"error uploading {fileName}");
        }
    }
}