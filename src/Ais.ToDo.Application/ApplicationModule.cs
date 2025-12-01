using System.Runtime.CompilerServices;
using Ais.Commons.CQRS.Behaviors;
using Ais.Commons.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Ais.ToDo.Tests")]
namespace Ais.ToDo.Application;

public sealed class ApplicationModule : Module
{
    public override void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(ApplicationModule).Assembly;
        
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(assembly);

            options.AddOpenBehaviors([
                typeof(CacheableBehavior<,>),
                typeof(ValidationBehavior<,>),
                typeof(Behaviors.TransactionalBehavior<,>),
                typeof(PublishBehavior<,>),
                typeof(NotifyBehavior<,>)
            ]);
        });

        services.AddAutoMapper(options =>
        {
        }, assembly);
        
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }
}