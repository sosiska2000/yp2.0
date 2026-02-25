using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v3")]
    [ApiController]
    public class ConsumablesController : ControllerBase
    {
        ///<summary>
        ///Получение списка расходных материалов
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Consumable>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Consumables.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение расходного материала по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Consumable), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Consumables.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Расходный материал не найден");
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
        ///Получение расходных материалов по типу
        ///</summary>
        [HttpGet("ByType/{typeId}")]
        [ProducesResponseType(typeof(List<Consumable>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByType(int typeId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Consumables
                        .Where(x => x.ConsumableTypeId == typeId)
                        .OrderBy(x => x.Name)
                        .ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение расходных материалов по оборудованию
        ///</summary>
        [HttpGet("ByEquipment/{equipmentId}")]
        [ProducesResponseType(typeof(List<Consumable>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByEquipment(int equipmentId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.EquipmentConsumables
                        .Where(ec => ec.EquipmentId == equipmentId)
                        .Join(context.Consumables, ec => ec.ConsumableId, c => c.Id, (ec, c) => c)
                        .OrderBy(x => x.Name)
                        .ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение характеристик расходного материала
        ///</summary>
        [HttpGet("{id}/Attributes")]
        [ProducesResponseType(typeof(List<ConsumableAttribute>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetAttributes(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var attributes = context.ConsumableAttributes
                        .Where(x => x.ConsumableId == id)
                        .OrderBy(x => x.AttributeName)
                        .ToList();
                    return Ok(attributes);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления расходного материала
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Consumable item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название обязательно");
                    }

                    // Проверка количества
                    if (item.Quantity < 0)
                    {
                        return StatusCode(400, "Количество не может быть отрицательным");
                    }

                    // Проверка даты (формат будет проверяться в C#)

                    // Проверка существования связанных записей
                    if (item.ResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.ResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный ответственный пользователь не существует");
                    }

                    if (item.TempResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.TempResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный временно ответственный пользователь не существует");
                    }

                    if (item.ConsumableTypeId.HasValue && !context.ConsumableTypes.Any(x => x.Id == item.ConsumableTypeId))
                    {
                        return StatusCode(400, "Указанный тип расходного материала не существует");
                    }

                    context.Consumables.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Расходный материал успешно добавлен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления характеристики к расходному материалу
        ///</summary>
        [HttpPost("AddAttribute")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult AddAttribute([FromForm] ConsumableAttribute attribute)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (attribute == null || attribute.ConsumableId <= 0 ||
                        string.IsNullOrEmpty(attribute.AttributeName) || string.IsNullOrEmpty(attribute.AttributeValue))
                    {
                        return StatusCode(400, "Ошибка запроса. Все поля характеристики обязательны");
                    }

                    var consumableExists = context.Consumables.Any(x => x.Id == attribute.ConsumableId);
                    if (!consumableExists)
                    {
                        return StatusCode(400, "Указанный расходный материал не существует");
                    }

                    context.ConsumableAttributes.Add(attribute);
                    context.SaveChanges();
                    return StatusCode(200, "Характеристика успешно добавлена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения расходного материала
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Consumable item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.Consumables.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Расходный материал не найден");
                    }

                    // Проверка количества
                    if (item.Quantity < 0)
                    {
                        return StatusCode(400, "Количество не может быть отрицательным");
                    }

                    // Проверка существования связанных записей
                    if (item.ResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.ResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный ответственный пользователь не существует");
                    }

                    if (item.TempResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.TempResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный временно ответственный пользователь не существует");
                    }

                    if (item.ConsumableTypeId.HasValue && !context.ConsumableTypes.Any(x => x.Id == item.ConsumableTypeId))
                    {
                        return StatusCode(400, "Указанный тип расходного материала не существует");
                    }

                    edit.Name = item.Name;
                    edit.Description = item.Description;
                    edit.ReceiptDate = item.ReceiptDate;
                    edit.PhotoPath = item.PhotoPath;
                    edit.Quantity = item.Quantity;
                    edit.ResponsibleUserId = item.ResponsibleUserId;
                    edit.TempResponsibleUserId = item.TempResponsibleUserId;
                    edit.ConsumableTypeId = item.ConsumableTypeId;

                    context.SaveChanges();
                    return StatusCode(200, "Расходный материал успешно изменен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения характеристики
        ///</summary>
        [HttpPut("EditAttribute")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult EditAttribute([FromForm] ConsumableAttribute attribute)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (attribute == null || attribute.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.ConsumableAttributes.FirstOrDefault(x => x.Id == attribute.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Характеристика не найдена");
                    }

                    edit.AttributeName = attribute.AttributeName;
                    edit.AttributeValue = attribute.AttributeValue;

                    context.SaveChanges();
                    return StatusCode(200, "Характеристика успешно изменена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления расходного материала
        ///</summary>
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
                    var del = context.Consumables.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Расходный материал не найден");
                    }

                    // Проверка связей
                    var hasAttributes = context.ConsumableAttributes.Any(x => x.ConsumableId == id);
                    var hasEquipment = context.EquipmentConsumables.Any(x => x.ConsumableId == id);

                    if (hasAttributes || hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить расходный материал, у которого есть характеристики или он привязан к оборудованию");
                    }

                    context.Consumables.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Расходный материал успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления характеристики
        ///</summary>
        [HttpDelete("DeleteAttribute/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult DeleteAttribute(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var del = context.ConsumableAttributes.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Характеристика не найдена");
                    }

                    context.ConsumableAttributes.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Характеристика успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}