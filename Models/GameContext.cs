using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class GameMgrContext : DbContext
    {
        public GameMgrContext(DbContextOptions<GameMgrContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }
        public DbSet<GameConfig>? GameSetup { get; set; }
        public DbSet<GameView>? GameView { get; set; }
        public DbSet<NextGame>? NextGame { get; set; }
        public DbSet<TempGame>? TempGame { get; set; }
        public DbSet<School>? Schools { get; set; }
        public DbSet<TempSchool>? TempSchool { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<Player>? Players { get; set; }
        public DbSet<PlayerSorted>? Teams { get; set; }
        public DbSet<Sponsor>? Sponsors { get; set; }
        public DbSet<jwt_seed>? jwt_seed { get; set; }
        public DbSet<UserRecord>? Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=games.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameConfig>().ToTable(nameof(GameConfig));
            modelBuilder.Entity<Game>().HasOne<School>(h => h.home).WithMany(s => s.HomeGames).HasForeignKey(h => h.homeId);
            modelBuilder.Entity<Game>().HasOne<School>(a => a.away).WithMany(s => s.AwayGames).HasForeignKey(a => a.awayId);
        }
    }
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public GameContext()
        {
        }

        public DbSet<ScoreData>? ScoreboardData { get; set; }
        public DbSet<ClockData>? Clocks { get; set; }
        // In-memory cache of currently displayed player hightlight
        public DbSet<Player>? Hilight { get; set; }
        // In-memory cache of currently displayed sponsor/advertisement
        public DbSet<Sponsor>? Ad { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ScoreBoard");
        }
    }
}