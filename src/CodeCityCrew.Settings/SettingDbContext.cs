using CodeCityCrew.Settings.Model;
using Microsoft.EntityFrameworkCore;

namespace CodeCityCrew.Settings
{
    /// <summary>
    /// Settings Context.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class SettingDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingDbContext"/> class.
        /// </summary>
        public SettingDbContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SettingDbContext(DbContextOptions<SettingDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public virtual DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasKey(option => new { option.Id, option.EnvironmentName });
        }
    }
}
