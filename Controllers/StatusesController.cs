using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<Status>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Statuses.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Status), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Statuses.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Статус не найден");
                    }
                    return Ok(item);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Status item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название статуса обязательно");
                    }
                    context.Statuses.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Статус успешно добавлен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Status item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.Statuses.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Статус не найден");
                    }
                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Статус успешно изменен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Delete(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var del = context.Statuses.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Статус не найден");
                    }

                    var hasEquipment = context.Equipment.Any(x => x.StatusId == id);
                    if (hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить статус, используемый в оборудовании");
                    }

                    context.Statuses.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Статус успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}