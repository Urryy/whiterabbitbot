using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram;

public class ApplicationDatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
	public DbSet<NFT> NFTs { get; set; }
	public DbSet<TokenWС> TokenWCs { get; set; }
	public DbSet<Clann> Clanns { get; set; }
	public DbSet<ClannItem> ClannItems { get; set; }
	public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasKey(e => e.Id);
		modelBuilder.Entity<NFT>().HasKey(e => e.Id);
		modelBuilder.Entity<TokenWС>().HasKey(e => e.Id);
		modelBuilder.Entity<Clann>().HasKey(e => e.Id);
	}
}
