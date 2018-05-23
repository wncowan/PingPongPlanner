using Microsoft.EntityFrameworkCore;

namespace PingPongPlanner.Models
{
    public class PingPongPlannerContext : DbContext
    {
        // INCLUDE ALL MODELS AS DBSETS: ie. public DbSet<User> Users { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }
        
    



        public PingPongPlannerContext(DbContextOptions<PingPongPlannerContext> options) : base(options)
        { }
    }
}