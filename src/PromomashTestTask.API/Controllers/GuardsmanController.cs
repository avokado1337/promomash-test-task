using Microsoft.AspNetCore.Mvc;
using PromomashTestTask.API.Dtos;
using PromomashTestTask.Core.Models;
using PromomashTestTask.Core.Services;
using PromomashTestTask.Infrastructure.Services;

namespace PromomashTestTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuardsmanController : ControllerBase
    {
        private readonly IGuardsmanService _userService;

        public GuardsmanController(IGuardsmanService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddGuardsmanDto addUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto
                {
                    Message = "Дух машины гневается! Проверьте введённые данные и повторите попытку.",
                    Description = "Введённые данные не соответствуют требованиям. Проверьте формат и обязательные поля."
                });

            try
            {
                var user = new Guardsman
                {
                    VoxAddress = addUserDto.VoxAddress,
                    SegmentId = addUserDto.SegmentId,
                    SystemId = addUserDto.SystemId,
                    PlanetId = addUserDto.PlanetId
                };
                await _userService.AddGuardsmanAsync(user, addUserDto.Password);
                return Ok(new ResponseDto
                {
                    Message = $"Новобранец успешно записан в ряды Имперской Гвардии!",
                    Description = $"Уникальный идентификатор новобранца: {user.Id}"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseDto
                {
                    Message = $"Ошибка рекрута: {ex.Message}. Омниссия требует исправления!",
                    Description = "Переданные данные содержат ошибки. Проверьте их и повторите попытку."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto
                {
                    Message = $"Внутренняя ошибка. Омниссия защити! Дух машины говорит: {ex.Message} Император наблюдает за нами!",
                    Description = "Произошла непредвиденная ошибка на сервере. Обратитесь к техножрецам для устранения проблемы."
                });
            }
        }
    }
}
