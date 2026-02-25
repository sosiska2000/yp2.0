using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v3")]
    [ApiController]
    public class EquipmentConsumablesController : ControllerBase
    {
        ///<summary>
        ///Получение списка связей оборудование-расходники
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<EquipmentConsumable>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.EquipmentConsumables.ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение расходников по оборудованию
        ///</summary>
        [HttpGet("ByEquipment/{equipmentId}")]
        [ProducesResponseType(typeof(List<Consumable>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetConsumablesByEquipment(int equipmentId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var consumables = context.EquipmentConsumables
                        .Where(ec => ec.EquipmentId == equipmentId)
                        .Join(context.Consumables, ec => ec.ConsumableId, c => c.Id, (ec, c) => c)
                        .OrderBy(c => c.Name)
                        .ToList();
                    return Ok(consumables);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение оборудования по расходнику
        ///</summary>
        [HttpGet("ByConsumable/{consumableId}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetEquipmentByConsumable(int consumableId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var equipment = context.EquipmentConsumables
                        .Where(ec => ec.ConsumableId == consumableId)
                        .Join(context.Equipment, ec => ec.EquipmentId, e => e.Id, (ec, e) => e)
                        .OrderBy(e => e.Name)
                        .ToList();
                    return Ok(equipment);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления связи оборудование-расходник
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] int equipmentId, [FromForm] int consumableId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (equipmentId <= 0 || consumableId <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса. ID оборудования и расходника обязательны");
                    }

                    var equipmentExists = context.Equipment.Any(x => x.Id == equipmentId);
                    if (!equipmentExists)
                    {
                        return StatusCode(400, "Оборудование не существует");
                    }

                    var consumableExists = context.Consumables.Any(x => x.Id == consumableId);
                    if (!consumableExists)
                    {
                        return StatusCode(400, "Расходный материал не существует");
                    }

                    var exists = context.EquipmentConsumables.Any(x => x.EquipmentId == equipmentId && x.ConsumableId == consumableId);
                    if (exists)
                    {
                        return StatusCode(400, "Данная связь уже существует");
                    }

                    context.EquipmentConsumables.Add(new EquipmentConsumable
                    {
                        EquipmentId = equipmentId,
                        ConsumableId = consumableId
                    });
                    context.SaveChanges();

                    return StatusCode(200, "Связь успешно добавлена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления связи оборудование-расходник
        ///</summary>
        [HttpDelete("Delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Delete([FromForm] int equipmentId, [FromForm] int consumableId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var del = context.EquipmentConsumables
                        .FirstOrDefault(x => x.EquipmentId == equipmentId && x.ConsumableId == consumableId);

                    if (del == null)
                    {
                        return StatusCode(400, "Связь не найдена");
                    }

                    context.EquipmentConsumables.Remove(del);
                    context.SaveChanges();

                    return StatusCode(200, "Связь успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}