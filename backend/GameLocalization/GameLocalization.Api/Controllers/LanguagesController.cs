using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Models.Responses.Languages;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
    /// <summary>
    /// Provides operations for managing application languages.
    /// </summary>
    [Authorize(Policy = AppRoles.Member)]
    [ApiController]
    [Route("api/v{version:apiVersion}/languages")]
    [ApiVersion("1.0")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _service; 
        private readonly IMapper _mapper;
        public LanguagesController(ILanguageService service, IMapper mapper)
        { 
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a list of all languages.
        /// </summary>
        /// <param name="includeDisabled">If <c>true</c>, returns both enabled and disabled languages. Default is <c>false</c>.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<LanguageResponse>>> GetAll(
            [FromQuery] bool includeDisabled = false,
            CancellationToken ct = default)
        {
            var result = await _service.GetAllAsync(includeDisabled, ct);

            return this.ToOkWrapped<IReadOnlyList<LanguageDto>, IReadOnlyList<LanguageResponse>>(
                result,
                _mapper
            );
        }

        /// <summary>
        /// Retrieves details of a language by its unique identifier.
        /// </summary>
        /// <param name="id">The language ID.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LanguageResponse>> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);

            return this.ToOkWrapped<LanguageDto, LanguageResponse>(result, _mapper);
        }

        /// <summary>
        /// Updates the enabled/disabled status of a language.
        /// </summary>
        /// <param name="id">The language ID.</param>
        /// <param name="request">The request body containing the new status.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LanguageResponse>> UpdateStatus(
            Guid id,
            [FromBody] UpdateLanguageStatusRequest request,
            CancellationToken ct = default)
        {
            var result = await _service.UpdateStatusAsync(id, request.IsEnabled, ct);
            return this.ToOkWrapped<LanguageDto, LanguageResponse>(result, _mapper);
        }

        /// <summary>
        /// Updates the details of an existing language (name, code, enabled status).
        /// </summary>
        /// <param name="id">The language ID.</param>
        /// <param name="request">The request body with updated language data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The updated <see cref="LanguageResponse"/>.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LanguageResponse>> Update(
            Guid id,
            [FromBody] UpdateLanguageRequest request,
            CancellationToken ct = default)
        {
            var result = await _service
                .UpdateAsync(id,_mapper.Map<UpdateLanguageDto>(request), ct);

            return this.ToOkWrapped<LanguageDto, LanguageResponse>(result, _mapper);
        }

        /// <summary>
        /// Creates a new language entry.
        /// </summary>
        /// <param name="request">The request body containing language data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created <see cref="LanguageResponse"/>.</returns>
        [HttpPost]
        [Authorize(Policy = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LanguageResponse>> Create(
            [FromBody] CreateLanguageRequest request,
            CancellationToken ct = default)
        {
            var result = await _service
                .CreateAsync(_mapper.Map<CreateLanguageDto>(request), ct);

            return this.ToActionResult<LanguageDto, LanguageResponse>(
                result,
                _mapper,
                onSuccess: r => CreatedAtAction(nameof(GetById), new { id = r.Id }, r)
            );
        }

        /// <summary>
        /// Deletes a language by its ID.
        /// </summary>
        /// <param name="id">The language ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if deletion succeeded.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken ct = default)
        {
            var result = await _service.DeleteAsync(id, ct);
            return this.ToActionResult(result);
        }
    }
}
