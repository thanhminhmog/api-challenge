using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using BLL.BussinessLogics;

namespace API
{
    [Route("base")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IUserLogic _userLogic;

        public BaseController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        [HttpPost]
        public IActionResult WritingObject()
        {
            _userLogic.WritingAnObjectAsync().Wait();
            return Ok("Uploaded");
        }


    }
}