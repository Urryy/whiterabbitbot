using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram;

public class ApplicationDatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
	public DbSet<NFT> NFTs { get; set; }
	public DbSet<TokenWB> TokenWBs { get; set; }
	public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasKey(e => e.Id);
		modelBuilder.Entity<NFT>().HasKey(e => e.Id);
		modelBuilder.Entity<TokenWB>().HasKey(e => e.Id);
	}
}
