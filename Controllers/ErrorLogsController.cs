using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    public class ErrorLogsController : ControllerBase
    {
        ///<summary>
        ///Получение списка ошибок (для разработчика)
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ErrorLog>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.ErrorLogs.OrderByDescending(x => x.Date).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение ошибки по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ErrorLog), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.ErrorLogs.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Ошибка не найдена");
                    }
                    return Ok(item);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления ошибки (используется системой)
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] ErrorLog item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Message))
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    item.Date = DateTime.Now;
                    context.ErrorLogs.Add(item);
                    context.SaveChanges();

                    return StatusCode(200, "Ошибка залогирована");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Очистка лога ошибок
        ///</summary>
        [HttpDelete("Clear")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult Clear()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    context.ErrorLogs.RemoveRange(context.ErrorLogs);
                    context.SaveChanges();

                    return StatusCode(200, "Лог ошибок очищен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}