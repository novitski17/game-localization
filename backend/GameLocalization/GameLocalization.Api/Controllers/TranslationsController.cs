using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Translations;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
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
        /// Updates the value of a translation by its ID.
        /// </summary>
        /// <param name="id">Translation ID.</param>
        /// <param name="request">Update translation request.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPatch("{id:guid}")]
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
 