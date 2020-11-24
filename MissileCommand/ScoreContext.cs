using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MissileCommand
{
    class ScoreContext : DbContext
    {
        public DbSet<Score> Scores { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=scores.db");
    }
    public class Score
    {
        public int ScoreId { get; set; }
        public string name { get; set; }
        public int score { get; set; }
    }
}
