namespace MissileCommand
{
    public class Difficulty
    {
        public static readonly Difficulty Easy = new Difficulty
        {
            CityRebuildDelay = 2,
            PlayerMissileSpeed = 2.0,
            EnemySpeed = 0.8
        };

        public static readonly Difficulty Normal = new Difficulty
        {
            CityRebuildDelay = 3,
            PlayerMissileSpeed = 1.0,
            EnemySpeed = 1.0
        };

        public static readonly Difficulty Hard = new Difficulty
        {
            CityRebuildDelay = 5,
            PlayerMissileSpeed = 0.8,
            EnemySpeed = 1.25
        };

        public static readonly Difficulty Debug = new Difficulty
        {
            CityRebuildDelay = 2,
            PlayerMissileSpeed = 50,
            EnemySpeed = 4
        };

        public int CityRebuildDelay { get; private set; }
        public double PlayerMissileSpeed { get; private set; }
        public double EnemySpeed { get; private set; }
    }
}
