using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class EquipmentTypesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<EquipmentType>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.EquipmentTypes.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EquipmentType), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.EquipmentTypes.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Тип оборудования не найден");
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
        public ActionResult Add([FromForm] EquipmentType item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название типа обязательно");
                    }
                    context.EquipmentTypes.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Тип оборудования успешно добавлен");
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
        public ActionResult Edit([FromForm] EquipmentType item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.EquipmentTypes.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Тип оборудования не найден");
                    }
                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Тип оборудования успешно изменен");
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
                    var del = context.EquipmentTypes.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Тип оборудования не найден");
                    }

                    var hasModels = context.Models.Any(x => x.EquipmentTypeId == id);
                    if (hasModels)
                    {
                        return StatusCode(400, "Нельзя удалить тип, к которому привязаны модели");
                    }

                    context.EquipmentTypes.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Тип оборудования успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}