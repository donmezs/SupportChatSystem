using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using SupportChatSystem.Infrastructure.Data.Context;
using SupportChatSystem.Infrastructure.Data.Repositories;
using System.Diagnostics;

namespace SupportChatSystem.Infrastructure.Data;
public static class DbExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("supportchatsystem")
                .LogTo(message => Debug.WriteLine(message))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }

    public static void AddSampleData(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (!context.Agents.Any())
            {
                // Create Shifts
                var morningShift = new Shift(ShiftType.Morning, DateTime.Today.AddHours(8), DateTime.Today.AddHours(16));
                var nightShift = new Shift(ShiftType.Night, DateTime.Today.AddHours(16), DateTime.Today.AddHours(24));

                // Create Teams
                var teamA = new Team("Team A");
                var teamB = new Team("Team B");
                var teamC = new Team("Team C");

                context.Shifts.AddRange(morningShift, nightShift);
                context.Teams.AddRange(teamA, teamB, teamC);

                // Create Agents
                var agents = new List<Agent>
                {
                    new Agent("Alice", AgentSeniority.MidLevel, morningShift.Id, teamA.Id),
                    new Agent("Bob", AgentSeniority.Junior, morningShift.Id, teamA.Id),
                    new Agent("Charlie", AgentSeniority.Senior, nightShift.Id, teamB.Id),
                    new Agent("James", AgentSeniority.Senior, nightShift.Id, teamB.Id),
                };

                context.Agents.AddRange(agents);

                context.SaveChanges();
            }
        }
    }
}
