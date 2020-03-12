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
using System.Security.Claims;
using BLL.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace API
{
    [Route("base")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        

    }
}