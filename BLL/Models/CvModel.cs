﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Models
{
    public class CvModel
    {
        public string Email { get; set; }
        public DateTime UploadDate { get; set; }
        public IFormFile File { get; set; }
    }
}
