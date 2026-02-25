using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    public class EquipmentSoftwareController : ControllerBase
    {
        ///<summary>
        ///Получение списка связей оборудования и ПО
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<EquipmentSoftware>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.EquipmentSoftware.ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение ПО по оборудованию
        ///</summary>
        [HttpGet("ByEquipment/{equipmentId}")]
        [ProducesResponseType(typeof(List<Software>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetSoftwareByEquipment(int equipmentId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var software = context.EquipmentSoftware
                        .Where(es => es.EquipmentId == equipmentId)
                        .Join(context.Software, es => es.SoftwareId, s => s.Id, (es, s) => s)
                        .OrderBy(s => s.Name)
                        .ToList();
                    return Ok(software);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение оборудования по ПО
        ///</summary>
        [HttpGet("BySoftware/{softwareId}")]
        [ProducesResponseType(typeof(List<Equipment>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetEquipmentBySoftware(int softwareId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var equipment = context.EquipmentSoftware
                        .Where(es => es.SoftwareId == softwareId)
                        .Join(context.Equipment, es => es.EquipmentId, e => e.Id, (es, e) => e)
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
        ///Метод добавления связи оборудование-ПО
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] int equipmentId, [FromForm] int softwareId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (equipmentId <= 0 || softwareId <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса. ID оборудования и ПО обязательны");
                    }

                    var equipmentExists = context.Equipment.Any(x => x.Id == equipmentId);
                    if (!equipmentExists)
                    {
                        return StatusCode(400, "Оборудование не существует");
                    }

                    var softwareExists = context.Software.Any(x => x.Id == softwareId);
                    if (!softwareExists)
                    {
                        return StatusCode(400, "Программа не существует");
                    }

                    var exists = context.EquipmentSoftware.Any(x => x.EquipmentId == equipmentId && x.SoftwareId == softwareId);
                    if (exists)
                    {
                        return StatusCode(400, "Данная связь уже существует");
                    }

                    context.EquipmentSoftware.Add(new EquipmentSoftware
                    {
                        EquipmentId = equipmentId,
                        SoftwareId = softwareId
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
        ///Метод удаления связи оборудование-ПО
        ///</summary>
        [HttpDelete("Delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Delete([FromForm] int equipmentId, [FromForm] int softwareId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var del = context.EquipmentSoftware
                        .FirstOrDefault(x => x.EquipmentId == equipmentId && x.SoftwareId == softwareId);

                    if (del == null)
                    {
                        return StatusCode(400, "Связь не найдена");
                    }

                    context.EquipmentSoftware.Remove(del);
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