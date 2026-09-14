using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Opus127.Dengue.Api.Contracts;
using Opus127.Dengue.Application.Abstractions;

namespace Opus127.Dengue.Api.Controllers;

[ApiController]
[Route("api/dengue")]
[Produces("application/json")]
public sealed class DengueController(IDengueQueryService queryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DengueWeekResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DengueWeekResponse>> GetByWeek(
        [FromQuery, Range(1, 53)] int ew,
        [FromQuery, Range(1900, 9999)] int ey,
        CancellationToken cancellationToken)
    {
        var result = await queryService.GetByWeekAsync(ey, ew, cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Epidemiological week not found",
                detail: $"No stored dengue data was found for {ey}-{ew:D2}.");
        }

        return Ok(DengueWeekResponse.From(result));
    }

    [HttpGet("weeks/latest")]
    [ProducesResponseType<IReadOnlyList<EpidemiologicalWeekResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<EpidemiologicalWeekResponse>>> GetLatestWeeks(
        [FromQuery, Range(1, 12)] int count = 3,
        CancellationToken cancellationToken = default)
    {
        var weeks = await queryService.GetLatestWeeksAsync(count, cancellationToken);
        return Ok(weeks.Select(EpidemiologicalWeekResponse.From).ToArray());
    }
}
