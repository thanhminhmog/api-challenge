using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Cv
    {
        [Key]
        public Guid KeyName { get; set; }
        public DateTime UploadDate { get; set; }
        public string FileName { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
