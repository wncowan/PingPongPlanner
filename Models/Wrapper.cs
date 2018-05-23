using System;
using System.Collections.Generic;

namespace PingPongPlanner.Models
{
    public class Wrapper : BaseEntity
    {
        public List<User> Users { get; set; }
        public List<Match> Matches { get; set; }
        public List<Guest> Guests { get; set; }

        public Wrapper(List<User> users, List<Match> matches, List<Guest> guests)
        {
            Users = users;
            Matches = matches;
            Guests = guests;
        }
    }
}