using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Models.Responses.Languages;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
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
        /// Returns a list of languages. Optionally includes disabled languages.
        /// </summary>
        /// <param name="includeDisabled">If true, returns both enabled and disabled languages.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet]
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
        /// Returns details of a language by its ID.
        /// </summary>
        /// <param name="id">Language ID.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<LanguageResponse>> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);

            return this.ToOkWrapped<LanguageDto, LanguageResponse>(result, _mapper);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<LanguageResponse>> UpdateStatus(
            Guid id,
            [FromBody] UpdateLanguageStatusRequest request,
            CancellationToken ct = default)
        {
            var result = await _service.UpdateStatusAsync(id, request.IsEnabled, ct);
            return this.ToOkWrapped<LanguageDto, LanguageResponse>(result, _mapper);
        }
    }
}
