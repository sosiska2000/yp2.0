using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        ///<summary>
        ///Получение списка оборудования
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Equipment.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение оборудования по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Equipment), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Equipment.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Оборудование не найдено");
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
        ///Поиск оборудования по наименованию
        ///</summary>
        [HttpGet("Search/{name}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult Search(string name)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Equipment
                        .Where(x => x.Name.Contains(name))
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
        ///Получение оборудования по аудитории
        ///</summary>
        [HttpGet("ByClassroom/{classroomId}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByClassroom(int classroomId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Equipment
                        .Where(x => x.ClassroomId == classroomId)
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
        ///Получение оборудования по ответственному
        ///</summary>
        [HttpGet("ByResponsible/{userId}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByResponsible(int userId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Equipment
                        .Where(x => x.ResponsibleUserId == userId)
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
        ///Метод добавления оборудования
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Equipment item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.InventoryNumber))
                    {
                        return StatusCode(400, "Ошибка запроса. Название и инвентарный номер обязательны");
                    }

                    // Проверка инвентарного номера (только цифры)
                    if (!item.InventoryNumber.All(char.IsDigit))
                    {
                        return StatusCode(400, "Инвентарный номер должен содержать только цифры");
                    }

                    // Проверка уникальности инвентарного номера
                    var invExists = context.Equipment.Any(x => x.InventoryNumber == item.InventoryNumber);
                    if (invExists)
                    {
                        return StatusCode(400, "Оборудование с таким инвентарным номером уже существует");
                    }

                    // Проверка стоимости (если указана)
                    if (item.Cost.HasValue && item.Cost < 0)
                    {
                        return StatusCode(400, "Стоимость не может быть отрицательной");
                    }

                    // Проверка существования связанных записей
                    if (item.ClassroomId.HasValue && !context.Classrooms.Any(x => x.Id == item.ClassroomId))
                    {
                        return StatusCode(400, "Указанная аудитория не существует");
                    }

                    if (item.ResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.ResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный ответственный пользователь не существует");
                    }

                    if (item.TempResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.TempResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный временно ответственный пользователь не существует");
                    }

                    if (item.DirectionId.HasValue && !context.Directions.Any(x => x.Id == item.DirectionId))
                    {
                        return StatusCode(400, "Указанное направление не существует");
                    }

                    if (item.StatusId.HasValue && !context.Statuses.Any(x => x.Id == item.StatusId))
                    {
                        return StatusCode(400, "Указанный статус не существует");
                    }

                    if (item.ModelId.HasValue && !context.Models.Any(x => x.Id == item.ModelId))
                    {
                        return StatusCode(400, "Указанная модель не существует");
                    }

                    context.Equipment.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Оборудование успешно добавлено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения оборудования
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Equipment item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.Equipment.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Оборудование не найдено");
                    }

                    // Проверка инвентарного номера (только цифры)
                    if (!string.IsNullOrEmpty(item.InventoryNumber) && !item.InventoryNumber.All(char.IsDigit))
                    {
                        return StatusCode(400, "Инвентарный номер должен содержать только цифры");
                    }

                    // Проверка уникальности инвентарного номера (если меняется)
                    if (!string.IsNullOrEmpty(item.InventoryNumber) && edit.InventoryNumber != item.InventoryNumber)
                    {
                        var invExists = context.Equipment.Any(x => x.InventoryNumber == item.InventoryNumber);
                        if (invExists)
                        {
                            return StatusCode(400, "Оборудование с таким инвентарным номером уже существует");
                        }
                        edit.InventoryNumber = item.InventoryNumber;
                    }

                    // Проверка стоимости (если указана)
                    if (item.Cost.HasValue && item.Cost < 0)
                    {
                        return StatusCode(400, "Стоимость не может быть отрицательной");
                    }

                    // Проверка существования связанных записей
                    if (item.ClassroomId.HasValue && !context.Classrooms.Any(x => x.Id == item.ClassroomId))
                    {
                        return StatusCode(400, "Указанная аудитория не существует");
                    }

                    if (item.ResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.ResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный ответственный пользователь не существует");
                    }

                    if (item.TempResponsibleUserId.HasValue && !context.Users.Any(x => x.Id == item.TempResponsibleUserId))
                    {
                        return StatusCode(400, "Указанный временно ответственный пользователь не существует");
                    }

                    if (item.DirectionId.HasValue && !context.Directions.Any(x => x.Id == item.DirectionId))
                    {
                        return StatusCode(400, "Указанное направление не существует");
                    }

                    if (item.StatusId.HasValue && !context.Statuses.Any(x => x.Id == item.StatusId))
                    {
                        return StatusCode(400, "Указанный статус не существует");
                    }

                    if (item.ModelId.HasValue && !context.Models.Any(x => x.Id == item.ModelId))
                    {
                        return StatusCode(400, "Указанная модель не существует");
                    }

                    edit.Name = item.Name;
                    edit.PhotoPath = item.PhotoPath;
                    edit.ClassroomId = item.ClassroomId;
                    edit.ResponsibleUserId = item.ResponsibleUserId;
                    edit.TempResponsibleUserId = item.TempResponsibleUserId;
                    edit.Cost = item.Cost;
                    edit.DirectionId = item.DirectionId;
                    edit.StatusId = item.StatusId;
                    edit.ModelId = item.ModelId;
                    edit.Comment = item.Comment;

                    context.SaveChanges();
                    return StatusCode(200, "Оборудование успешно изменено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления оборудования
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
                    var del = context.Equipment.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Оборудование не найдено");
                    }

                    // Проверка связей
                    var hasNetworkSettings = context.NetworkSettings.Any(x => x.EquipmentId == id);
                    var hasSoftware = context.EquipmentSoftware.Any(x => x.EquipmentId == id);
                    var hasConsumables = context.EquipmentConsumables.Any(x => x.EquipmentId == id);
                    var hasInventoryItems = context.InventoryItems.Any(x => x.EquipmentId == id);

                    if (hasNetworkSettings || hasSoftware || hasConsumables || hasInventoryItems)
                    {
                        return StatusCode(400, "Нельзя удалить оборудование, у которого есть сетевые настройки, ПО, расходники или оно участвует в инвентаризации");
                    }

                    context.Equipment.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Оборудование успешно удалено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}