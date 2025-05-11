using Microsoft.AspNetCore.Mvc;
using PromomashTestTask.API.Dtos;
using PromomashTestTask.Core.Services;

namespace PromomashTestTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SegmentController : ControllerBase
    {
        private readonly ISegmentService _segmentService;
        private readonly ILogger<SegmentController> _logger;

        public SegmentController(
            ISegmentService segmentService,
            ILogger<SegmentController> logger)
        {
            _segmentService = segmentService;
            _logger = logger;
        }

        /// <summary>
        /// Получение списка всех сегментов Империума
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SegmentDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto), 500)]
        public async Task<IActionResult> GetAllSegmentums()
        {
            try
            {
                _logger.LogInformation("Запрос всех сегментов Империума");
                var segments = await _segmentService.GetAllSegmentsAsync();

                return Ok(segments.Select(segment => new SegmentDto
                {
                    Id = segment.Id,
                    Name = segment.Name
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка доступа к данным сегментов");
                return StatusCode(500, new ResponseDto
                {
                    Message = "Ошибка Машинного Духа",
                    Description = $"Дух машины отказал в послушании. Техножрецы уведомлены. Детали: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Получение систем в указанном сегменте
        /// </summary>
        [HttpGet("{segmentId}/systems")]
        [ProducesResponseType(typeof(IEnumerable<SystemDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto), 404)]
        [ProducesResponseType(typeof(ResponseDto), 500)]
        public async Task<IActionResult> GetSystemsBySegment(Guid segmentId)
        {
            try
            {
                _logger.LogInformation("Запрос систем для сегмента {SegmentId}", segmentId);
                var systems = await _segmentService.GetSystemsBySegmentAsync(segmentId);

                if (systems == null || !systems.Any())
                {
                    _logger.LogWarning("Сегмент {SegmentId} не найден", segmentId);
                    return NotFound(new ResponseDto
                    {
                        Message = "Сегмент не найден",
                        Description = "Указанный сегмент отсутствует в архивах Администратума"
                    });
                }

                return Ok(systems.Select(system => new SystemDto
                {
                    Id = system.Id,
                    Name = system.Name,
                    SegmentId = system.SegmentId
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении систем сегмента {SegmentId}", segmentId);
                return StatusCode(500, new ResponseDto
                {
                    Message = "Ошибка Машинного Духа",
                    Description = $"Дух машины отказал в послушании. Техножрецы уведомлены. Детали: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Получение планет в указанной системе
        /// </summary>
        [HttpGet("systems/{systemId}/planets")]
        [ProducesResponseType(typeof(IEnumerable<PlanetDto>), 200)]
        [ProducesResponseType(typeof(ResponseDto), 404)]
        [ProducesResponseType(typeof(ResponseDto), 500)]
        public async Task<IActionResult> GetPlanetsBySystem(Guid systemId)
        {
            try
            {
                _logger.LogInformation("Запрос планет для системы {SystemId}", systemId);
                var planets = await _segmentService.GetPlanetsBySystemAsync(systemId);

                if (planets == null || !planets.Any())
                {
                    _logger.LogWarning("Система {SystemId} не содержит планет", systemId);
                    return NotFound(new ResponseDto
                    {
                        Message = "Планеты не найдены",
                        Description = "В указанной системе не зарегистрировано обитаемых планет"
                    });
                }

                return Ok(planets.Select(planet => new PlanetDto
                {
                    Id = planet.Id,
                    Name = planet.Name,
                    SystemId = planet.SystemId
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении планет системы {SystemId}", systemId);
                return StatusCode(500, new ResponseDto
                {
                    Message = "Ошибка Машинного Духа",
                    Description = $"Дух машины отказал в послушании. Техножрецы уведомлены. Детали: {ex.Message}"
                });
            }
        }
    }
}