using Microsoft.EntityFrameworkCore;
using Ping_Project.Entities;
using Ping_Project.ValueObjects;
using Ping_Project.Services;

namespace Ping_Project.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options, Encrypt encrypt, Decrypt decrypt) : DbContext(options)
{
    public DbSet<Payload> HashedLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payload>().
            Property(x=>x.log).
            HasConversion(
                logObj => encrypt.EncryptService(logObj.Value),
                dbString => new Log(decrypt.DecryptService(dbString))
            );
    }
}