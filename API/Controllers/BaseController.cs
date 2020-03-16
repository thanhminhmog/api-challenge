using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        
    }
}