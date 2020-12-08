using Microsoft.EntityFrameworkCore;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace MissileCommand
{
    public class ScoreContext : DbContext
    {
        public DbSet<ScoreEntry> ScoreEntries { get; set; }
        public ScoreContext()
        {
            Database.EnsureCreated();
        }

        public static string ScoreFile { get; private set; } = Path.GetFullPath("scores.db");
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={ScoreFile}");
    }
    public class ScoreEntry
    {
        [Key]
        public int Id { get; protected set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public ScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
