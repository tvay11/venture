using Microsoft.EntityFrameworkCore;
using stock.Model.Entity;

namespace stock.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockPrice> StockPrices { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    
    public DbSet<StockHolding> StockHoldings { get; set; }
    public DbSet<UserHistory> UserHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<StockPrice>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<UserProfile>()
            .HasMany(up => up.StockHoldings)
            .WithOne(sh => sh.User)
            .HasForeignKey(sh => sh.UserId);
        
        modelBuilder.Entity<UserProfile>()
            .Property(u => u.Cash)
            .HasPrecision(18, 2);

        modelBuilder.Entity<UserProfile>()
            .Property(u => u.NetWorth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<StockHolding>()
            .HasKey(sh => sh.Id); 
        
        modelBuilder.Entity<StockHolding>()
            .HasOne(sh => sh.User)
            .WithMany(u => u.StockHoldings)
            .HasForeignKey(sh => sh.UserId);

        modelBuilder.Entity<StockHolding>()
            .HasOne(sh => sh.Stock)
            .WithMany(s => s.StockHoldings)
            .HasForeignKey(sh => sh.StockId);
        
        modelBuilder.Entity<UserHistory>()
            .Property(unwh => unwh.NetWorth)
            .HasPrecision(18, 2);
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    }
}