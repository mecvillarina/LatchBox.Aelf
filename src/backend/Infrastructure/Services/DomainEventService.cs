using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;

namespace Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IPublisher _mediator;

        public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent)
        {
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        }

        private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            return (INotification) Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
        }
    }
}