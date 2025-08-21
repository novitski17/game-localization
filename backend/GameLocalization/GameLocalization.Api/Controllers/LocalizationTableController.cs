using Asp.Versioning;
using AutoMapper;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Models.Requests.LocalizationTable;
using GameLocalization.Api.Models.Responses.Table;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLocalization.Api.Controllers
{
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

        [HttpGet]
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
