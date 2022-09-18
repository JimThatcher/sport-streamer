/*
Copyright (c) 2022 Jim Thatcher

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

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
        public DbSet<School>? Schools { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<Player>? Players { get; set; }
        public DbSet<Sponsor>? Sponsors { get; set; }
        public DbSet<jwt_seed>? jwt_seed { get; set; }
        public DbSet<UserRecord>? Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=games.db")
            .EnableSensitiveDataLogging(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

        public DbSet<DbScoreData>? ScoreboardData { get; set; }
        public DbSet<ClockData>? Clocks { get; set; }
        // In-memory cache of currently displayed player hightlight
        public DbSet<Player>? Hilight { get; set; }
        // In-memory cache of currently displayed sponsor/advertisement
        public DbSet<Sponsor>? Ad { get; set; }
        public DbSet<ConsoleInfo>? ConsoleVersion { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ScoreBoard");
        }
    }
}