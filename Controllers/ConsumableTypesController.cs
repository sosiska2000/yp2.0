using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class ConsumableTypesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ConsumableType>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.ConsumableTypes.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConsumableType), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.ConsumableTypes.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Тип расходного материала не найден");
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
        public ActionResult Add([FromForm] ConsumableType item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название типа обязательно");
                    }
                    context.ConsumableTypes.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Тип расходного материала успешно добавлен");
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
        public ActionResult Edit([FromForm] ConsumableType item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.ConsumableTypes.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Тип расходного материала не найден");
                    }
                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Тип расходного материала успешно изменен");
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
                    var del = context.ConsumableTypes.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Тип расходного материала не найден");
                    }

                    var hasConsumables = context.Consumables.Any(x => x.ConsumableTypeId == id);
                    if (hasConsumables)
                    {
                        return StatusCode(400, "Нельзя удалить тип, к которому привязаны расходные материалы");
                    }

                    context.ConsumableTypes.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Тип расходного материала успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}