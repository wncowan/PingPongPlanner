using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PingPongPlanner.Models
{
    public class Guest : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Match Match { get; set; }
        public int MatchId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }

    }
}