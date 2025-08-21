using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Keys;
using GameLocalization.Api.Models.Responses.Common;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{

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
        /// Returns a single localization key as a table row (includes empty values by language).
        /// </summary>
        /// <param name="id">Key ID.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet("{id:guid}")]
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
        /// Creates a new localization key and empty translations for all active languages.
        /// Returns a ready-to-render table row.
        /// </summary>
        /// <param name="request">Create key request.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPost]
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
        /// Delete a localization key and all related translations.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            var result = await _service.DeleteAsync(id, ct);
            return this.ToActionResult(result);
        }
    }
}
