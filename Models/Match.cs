using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PingPongPlanner.Models
{
    public class Match : BaseEntity
    {
        public int Id { get; set; }
        public string PlayerOne { get; set; }
        public string PlayerTwo { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public User Creator { get; set; }
        public int CreatorId { get; set; }
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        public List<Guest> Guests { get; set; }
        public List<Post> Posts { get; set; }

        public Match()
        {
            List<Guest> Guests = new List<Guest>();
            List<Post> Posts = new List<Post>();
        }
    }
}