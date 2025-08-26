using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Keys;
using GameLocalization.Api.Models.Responses.Common;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
    /// <summary>
    /// Provides operations for managing localization keys.
    /// </summary>
    [Authorize(Policy = AppRoles.Member)]
    [ApiController]
    [Route("api/v{version:apiVersion}/localization-keys")]
    [ApiVersion("1.0")]
    public class LocalizationKeysController : ControllerBase
    {
        private readonly ILocalizationKeyService _service;
        private readonly IMapper _mapper;

        public LocalizationKeysController(ILocalizationKeyService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a single localization key by its ID.
        /// </summary>
        /// <param name="id">The localization key identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocalizationTableRowResponse>> GetById(
            Guid id,
            CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, false, ct);

            return this.ToOkWrapped<LocalizationTableRowDto, LocalizationTableRowResponse>(
                result,
                _mapper
            );
        }

        /// <summary>
        /// Creates a new localization key.
        /// </summary>
        /// <remarks>
        /// When a key is created, empty translation placeholders are also generated
        /// for all active languages, so the key is immediately usable in the UI.
        /// </remarks>
        /// <param name="request">The request containing the key data.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocalizationTableRowResponse>> Create(
            [FromBody] CreateKeyRequest request,
            CancellationToken ct = default)
        {
            var result = await _service
                .CreateAsync(_mapper.Map<CreateLocalizationKeyDto>(request), ct);

            return this.ToActionResult<LocalizationTableRowDto, LocalizationTableRowResponse>(
                result,
                _mapper,
                onSuccess: r => CreatedAtAction(nameof(GetById), new { id = r.KeyId }, r)
            );
        }

        /// <summary>
        /// Deletes a localization key along with all its translations.
        /// </summary>
        /// <param name="id">The localization key identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            var result = await _service.DeleteAsync(id, ct);
            return this.ToActionResult(result);
        }
    }
}
