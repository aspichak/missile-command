using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MissileCommand
{
    class ScoreContext : DbContext
    {
        public DbSet<Score> Scores { get; set; }

        public ScoreContext()
        {
            LogFile = Path.GetFullPath("log.db");
            Database.EnsureCreated();
        }

        public static string LogFile { get; private set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={LogFile}");
    }
    public class Score
    {
        public int ScoreId { get; set; }
        public string name { get; set; }
        public int score { get; set; }
    }
}
