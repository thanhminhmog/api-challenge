using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class CV
    {
        [Key]
        public Guid CVId { get; set; }
        public string FilePath { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
