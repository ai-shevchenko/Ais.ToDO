using Ais.Commons.Application.Contracts.Models;
using Ais.ToDo.Application.Features;
using Ais.ToDo.Contracts;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ais.ToDo.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{v:apiVersion}/todo-items")]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public class ToDoItemsController : ControllerBase
{

    [HttpGet("{id:guid}")]
    [MapToApiVersion(1.0)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ToDoItemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetToDoItemByIdAsync(
        [FromServices] ISender sender, 
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetToDoItemById.Request(id), cancellationToken);
        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpPost]
    [MapToApiVersion(1.0)]
    [ProducesResponseType(typeof(BaseItemCreatedDto<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateToDoItemAsync(
        [FromServices] ISender sender,
        [FromBody] CreateToDoItemDto request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new CreateToDoItem.Command(request), cancellationToken);
        var routeValues = new { Version = 1.0, response.Id };
        var value = new { response.Id };
        
        return CreatedAtAction("GetToDoItemById", routeValues, value);
    }

    [HttpPut]
    [MapToApiVersion(1.0)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ToDoItemUpdatedDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateToDoItemDto(
        [FromServices] ISender sender,
        [FromBody] UpdateToDoItemDto request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new UpdateToDoItem.Command(request), cancellationToken);
        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpGet("page")]
    [MapToApiVersion(1.0)]
    [ProducesResponseType(typeof(PagedToDoListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedToDoListAsync(
        [FromServices] ISender sender,
        [FromQuery] GetPagedToDoListDto request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetPagedToDoList.Query(request), cancellationToken);
        return Ok(response);
    }
}