using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTgBot.DB
{
    class AppDbContext : DbContext
    {
        public AppDbContext() : base("DbConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserLogEntry> UserLogs { get; set; }

        public DbSet<Chat> Chats { get; set; }
    }
}
