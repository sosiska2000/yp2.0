using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v6")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        ///<summary>
        ///Получение списка инвентаризаций
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Inventory>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Inventories.OrderByDescending(x => x.StartDate).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение инвентаризации по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Inventory), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Inventories.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Инвентаризация не найдена");
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
        ///Получение состава инвентаризации
        ///</summary>
        [HttpGet("{id}/Items")]
        [ProducesResponseType(typeof(List<InventoryItem>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetItems(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var items = context.InventoryItems
                        .Where(x => x.InventoryId == id)
                        .ToList();
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение оборудования, ожидающего инвентаризации для пользователя
        ///</summary>
        [HttpGet("Pending/{userId}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetPendingForUser(int userId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    // Находим активные инвентаризации
                    var activeInventories = context.Inventories
                        .Where(i => i.StartDate <= DateTime.Now &&
                                    (!i.EndDate.HasValue || i.EndDate.Value >= DateTime.Now))
                        .Select(i => i.Id)
                        .ToList();

                    // Находим оборудование пользователя, которое еще не проинвентаризировано
                    var userEquipment = context.Equipment
                        .Where(e => e.ResponsibleUserId == userId)
                        .Select(e => e.Id)
                        .ToList();

                    var alreadyChecked = context.InventoryItems
                        .Where(ii => activeInventories.Contains(ii.InventoryId) &&
                                     userEquipment.Contains(ii.EquipmentId))
                        .Select(ii => ii.EquipmentId)
                        .ToList();

                    var pendingEquipment = context.Equipment
                        .Where(e => e.ResponsibleUserId == userId &&
                                    !alreadyChecked.Contains(e.Id))
                        .ToList();

                    return Ok(pendingEquipment);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод создания новой инвентаризации (для администратора)
        ///</summary>
        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Create([FromForm] Inventory inventory, [FromForm] List<int> equipmentIds)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (inventory == null || string.IsNullOrEmpty(inventory.Name) ||
                        inventory.StartDate == default)
                    {
                        return StatusCode(400, "Ошибка запроса. Название и дата начала обязательны");
                    }

                    if (inventory.CreatedByUserId.HasValue &&
                        !context.Users.Any(x => x.Id == inventory.CreatedByUserId))
                    {
                        return StatusCode(400, "Указанный пользователь не существует");
                    }

                    // Создаем инвентаризацию
                    context.Inventories.Add(inventory);
                    context.SaveChanges();

                    // Добавляем оборудование в инвентаризацию
                    if (equipmentIds != null && equipmentIds.Any())
                    {
                        foreach (var equipmentId in equipmentIds)
                        {
                            // Проверяем, что оборудование существует
                            if (context.Equipment.Any(x => x.Id == equipmentId))
                            {
                                context.InventoryItems.Add(new InventoryItem
                                {
                                    InventoryId = inventory.Id,
                                    EquipmentId = equipmentId
                                });
                            }
                        }
                        context.SaveChanges();
                    }

                    return StatusCode(200, "Инвентаризация успешно создана");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления оборудования в инвентаризацию
        ///</summary>
        [HttpPost("AddEquipment")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult AddEquipment([FromForm] int inventoryId, [FromForm] int equipmentId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var inventory = context.Inventories.FirstOrDefault(x => x.Id == inventoryId);
                    if (inventory == null)
                    {
                        return StatusCode(400, "Инвентаризация не найдена");
                    }

                    var equipment = context.Equipment.FirstOrDefault(x => x.Id == equipmentId);
                    if (equipment == null)
                    {
                        return StatusCode(400, "Оборудование не найдено");
                    }

                    // Проверяем, не добавлено ли уже
                    var exists = context.InventoryItems.Any(x => x.InventoryId == inventoryId && x.EquipmentId == equipmentId);
                    if (exists)
                    {
                        return StatusCode(400, "Оборудование уже добавлено в инвентаризацию");
                    }

                    context.InventoryItems.Add(new InventoryItem
                    {
                        InventoryId = inventoryId,
                        EquipmentId = equipmentId
                    });
                    context.SaveChanges();

                    return StatusCode(200, "Оборудование добавлено в инвентаризацию");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод проведения инвентаризации (для сотрудника/преподавателя)
        ///</summary>
        [HttpPost("Check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Check([FromForm] int inventoryId, [FromForm] int equipmentId,
                                   [FromForm] int? userId, [FromForm] string comment)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var inventoryItem = context.InventoryItems
                        .FirstOrDefault(x => x.InventoryId == inventoryId && x.EquipmentId == equipmentId);

                    if (inventoryItem == null)
                    {
                        return StatusCode(400, "Оборудование не найдено в данной инвентаризации");
                    }

                    // Проверяем, что пользователь имеет право (ответственный за это оборудование)
                    var equipment = context.Equipment.FirstOrDefault(x => x.Id == equipmentId);
                    if (equipment == null)
                    {
                        return StatusCode(400, "Оборудование не найдено");
                    }

                    if (userId.HasValue && equipment.ResponsibleUserId != userId)
                    {
                        var user = context.Users.FirstOrDefault(x => x.Id == userId);
                        if (user == null || user.Role != "admin")
                        {
                            return StatusCode(403, "У вас нет прав на инвентаризацию этого оборудования");
                        }
                    }

                    inventoryItem.CheckedByUserId = userId;
                    inventoryItem.Comment = comment;
                    context.SaveChanges();

                    return StatusCode(200, "Оборудование проинвентаризировано");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения инвентаризации
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Inventory item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.Inventories.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Инвентаризация не найдена");
                    }

                    edit.Name = item.Name;
                    edit.StartDate = item.StartDate;
                    edit.EndDate = item.EndDate;

                    context.SaveChanges();
                    return StatusCode(200, "Инвентаризация успешно изменена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления инвентаризации
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
                    var del = context.Inventories.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Инвентаризация не найдена");
                    }

                    // Items удалятся каскадно благодаря OnDelete(Cascade)
                    context.Inventories.Remove(del);
                    context.SaveChanges();

                    return StatusCode(200, "Инвентаризация успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}