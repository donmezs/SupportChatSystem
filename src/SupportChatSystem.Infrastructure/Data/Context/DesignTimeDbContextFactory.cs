using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SupportChatSystem.Infrastructure.Data.Context;
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly IMediator _mediator;

    public DesignTimeDbContextFactory()
    {
    }

    public DesignTimeDbContextFactory(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("supportchatsystem");

        return new ApplicationDbContext(optionsBuilder.Options, _mediator);
    }
}

