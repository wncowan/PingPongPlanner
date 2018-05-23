using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PingPongPlanner.Models
{
    public class MatchViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="You must enter a name")]
        public string PlayerOne { get; set; }

        [Required(ErrorMessage="You must enter a name")]
        public string PlayerTwo { get; set; }

        [Required(ErrorMessage="You must enter a date")]

        public DateTime Date { get; set; }

        [Required(ErrorMessage="You must enter an address")]
        public string Address { get; set; }
        //may be able to delete below
        public User User { get; set; }
        public int UserId { get; set; }
        public List<Guest> Guests { get; set; }
    }
}