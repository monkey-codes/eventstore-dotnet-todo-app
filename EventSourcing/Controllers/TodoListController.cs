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
        public async Task<OkDto> CreateTodoList([FromBody] CreateTodoListCommand command, CancellationToken cancellationToken)
        {
            var revision = await _mediator.Handle(command, cancellationToken);
            return new OkDto(command.Id, revision.Revision, revision.Response);
        }
        
        [HttpPost, Route("{todoListId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<OkDto> AddItemToList(Guid todoListId, [FromBody] AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            command.Id = todoListId;
            var revision = await _mediator.Handle(command, cancellationToken);
            return new OkDto(command.Id, revision.Revision, revision.Response);
        }

    }
    
    public class OkDto
    {
        public Guid Id { get; set; }
        public long Revision { get; set; }
        
        public object Response { get; set; }
    
        public OkDto(Guid id, long revision = -1, object response = null)
        {
            Id = id;
            Revision = revision;
            Response = response;
        }
    }
}