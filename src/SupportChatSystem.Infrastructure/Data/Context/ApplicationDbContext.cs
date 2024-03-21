using Microsoft.EntityFrameworkCore;
using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Infrastructure.Data.Context;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, MediatR.IMediator _mediator)
        : base(options)
    {
    }

    public DbSet<Agent> Agents { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Agent Configuration
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Shift)
                  .WithMany(s => s.Agents)
                  .HasForeignKey(e => e.ShiftId);
            entity.HasOne(e => e.Team)
                  .WithMany(t => t.Agents)
                  .HasForeignKey(e => e.TeamId);
            entity.Property(e => e.Seniority).HasConversion<string>();
            entity.HasMany(e => e.ChatSessions)
                  .WithOne(cs => cs.Agent)
                  .HasForeignKey(cs => cs.AgentId);
        });

        // ChatSession Configuration
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(cs => cs.Id);
            entity.Property(cs => cs.Status).HasConversion<string>();
            entity.HasOne(cs => cs.Agent)
                  .WithMany(a => a.ChatSessions)
                  .HasForeignKey(cs => cs.AgentId);
        });

        // Shift Configuration
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.ShiftType).HasConversion<string>();
        });

        // Team Configuration
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasMany(t => t.Agents)
                  .WithOne(a => a.Team)
                  .HasForeignKey(a => a.TeamId);
        });
    }
}
