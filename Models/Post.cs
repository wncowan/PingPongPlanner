using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PingPongPlanner.Models
{
    public class Post : BaseEntity
    {
        [Key]
        public int PostId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }

        public Match Match { get; set; }

        public int MatchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Comment> Comments { get; set; }

        public Post()
        {
            Comments = new List<Comment>();
        }
    }
}