using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;
using EventSourcing.Query;
using EventSourcing.Query.Projection;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> ListAllTodoLists([FromQuery(Name = "using")] string strategy, CancellationToken cancellationToken)
        {
            if (strategy == "projection")
            {
                return  (await _mediator.Query(new ListAllTodoListsUsingProjectionQuery(), cancellationToken));
            }
            return await _mediator.Query(new ListAllTodoListsQuery(), cancellationToken);
        }

        [HttpGet, Route("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetTodoList([FromRoute] GetTodoListQuery query,
            [FromQuery(Name = "using")] string strategy,
            CancellationToken cancellationToken)
        {
            if (strategy == "projection")
            {
                return  (await _mediator.Query(new GetTodoListUsingProjectionQuery
                {
                    Id = query.Id
                }, cancellationToken));
            }
            return await _mediator.Query(query, cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<RevisionedResponse<object>> CreateTodoList([FromBody] CreateTodoListCommand command,
            CancellationToken cancellationToken)
        {
            var revision = await _mediator.Handle(command, cancellationToken);
            return revision;
        }

        [HttpPost, Route("{todoListId}/items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<RevisionedResponse<object>> AddItemToList(Guid todoListId,
            [FromBody] AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            command.Id = todoListId;
            var revision = await _mediator.Handle(command, cancellationToken);
            return revision;
        }
    }
}