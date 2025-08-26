using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Translations;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
    /// <summary>
    /// Provides operations for managing translations.
    /// </summary>
    [Authorize(Policy = AppRoles.Member)]
    [ApiController]
    [Route("api/v{version:apiVersion}/translations")]
    [ApiVersion("1.0")]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationService _service;
        private readonly IMapper _mapper;

        public TranslationsController(
            ITranslationService service,
            IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Updates the value of a translation by its unique identifier.
        /// </summary>
        /// <remarks>
        /// Typically called from the localization table editor.  
        /// The request contains the new text value; if empty, the translation is considered "unset".
        /// </remarks>
        /// <param name="id">The translation ID.</param>
        /// <param name="request">The request body containing the updated translation value.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateTranslationRequest request,
            CancellationToken ct = default)
        {
            var dto = _mapper.Map<UpdateTranslationDto>(request);

            var result = await _service.UpdateAsync(id, dto, ct);
            return this.ToActionResult(result);
        }
    }
}
 