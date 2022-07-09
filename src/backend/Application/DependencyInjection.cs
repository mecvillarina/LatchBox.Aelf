using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Behaviours;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Application
{
    [ExcludeFromCodeCoverageAttribute]
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}