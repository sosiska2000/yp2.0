using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<Model>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Models.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Model), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Models.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Модель не найдена");
                    }
                    return Ok(item);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ByType/{typeId}")]
        [ProducesResponseType(typeof(List<Model>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByType(int typeId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Models.Where(x => x.EquipmentTypeId == typeId).OrderBy(x => x.Name).ToList();
                    return Ok(list);
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
        public ActionResult Add([FromForm] Model item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name) || item.EquipmentTypeId <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса. Название модели и тип обязательны");
                    }

                    var typeExists = context.EquipmentTypes.Any(x => x.Id == item.EquipmentTypeId);
                    if (!typeExists)
                    {
                        return StatusCode(400, "Указанный тип оборудования не существует");
                    }

                    context.Models.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Модель успешно добавлена");
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
        public ActionResult Edit([FromForm] Model item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.Models.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Модель не найдена");
                    }

                    if (item.EquipmentTypeId > 0)
                    {
                        var typeExists = context.EquipmentTypes.Any(x => x.Id == item.EquipmentTypeId);
                        if (!typeExists)
                        {
                            return StatusCode(400, "Указанный тип оборудования не существует");
                        }
                        edit.EquipmentTypeId = item.EquipmentTypeId;
                    }

                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Модель успешно изменена");
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
                    var del = context.Models.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Модель не найдена");
                    }

                    var hasEquipment = context.Equipment.Any(x => x.ModelId == id);
                    if (hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить модель, используемую в оборудовании");
                    }

                    context.Models.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Модель успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}