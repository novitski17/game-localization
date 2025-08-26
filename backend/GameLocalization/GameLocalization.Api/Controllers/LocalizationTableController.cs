using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.LocalizationTable;
using GameLocalization.Api.Models.Responses.Table;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
    /// <summary>
    /// Provides access to the localization table view.
    /// </summary>
    [Authorize(Policy = AppRoles.Member)]
    [ApiController]
    [Route("api/v{version:apiVersion}/localization-table")]
    [ApiVersion("1.0")]
    public class LocalizationTableController : ControllerBase
    {
        private readonly ILocalizationTableService _service;
        private readonly IMapper _mapper;

        public LocalizationTableController(ILocalizationTableService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a page of the localization table.
        /// </summary>
        /// <remarks>
        /// The response contains localization keys and their translations across all languages.
        /// Query parameters allow filtering, searching by text, and paging results.
        /// </remarks>
        /// <param name="request">Query parameters for filtering and paging (page size, search term, etc.).</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LocalizationTablePageResponse>> Get(
            [FromQuery] LocalizationTableQueryRequest request,
            CancellationToken ct = default)
        {
            var query = _mapper.Map<LocalizationTableQueryDto>(request);

            var result = await _service.GetAsync(query , ct);
            return this.ToOkWrapped<LocalizationTablePageDto, LocalizationTablePageResponse>(result, _mapper);
        }
    }
}
