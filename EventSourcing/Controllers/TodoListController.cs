using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.Mediator;
using EventStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class TodoListController
    {
        private readonly IMediator _mediator;

        public TodoListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<CreatedDto> CreateTodoList([FromBody] CreateTodoListCommand command, CancellationToken cancellationToken)
        {
            var revision = await _mediator.Handle(command, cancellationToken);
            return new CreatedDto(command.Id, revision);
        }
        
        [HttpPost, Route("{todoListId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<CreatedDto> AddItemToList(Guid todoListId, [FromBody] AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            command.Id = todoListId;
            var revision = await _mediator.Handle(command, cancellationToken);
            return new CreatedDto(todoListId, revision);
        }

    }

    public class CreatedDto
    {
        public Guid Id { get; set; }
        public long Revision { get; set; }

        public CreatedDto(Guid id, long revision = -1)
        {
            Id = id;
            Revision = revision;
        }
    }
}