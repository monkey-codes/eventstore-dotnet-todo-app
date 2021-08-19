using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.Mediator;
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
            return new CreatedDto(await _mediator.Handle(command, cancellationToken));
        }
        
        [HttpPost, Route("{todoListId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<CreatedDto> AddItemToList(Guid todoListId, [FromBody] AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            command.TodoListId = todoListId;
            return new CreatedDto(await _mediator.Handle(command, cancellationToken));
        }

    }

    public class CreatedDto
    {
        public Guid Id { get; set; }

        public CreatedDto(Guid id)
        {
            Id = id;
        }
    }
}