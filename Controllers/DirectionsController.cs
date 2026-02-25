using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class DirectionsController : ControllerBase
    {
        ///<summary>
        ///Получение списка направлений
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Direction>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Directions.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение направления по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Direction), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Directions.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Направление не найдено");
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
        ///Метод добавления направления
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Direction item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название направления обязательно");
                    }
                    context.Directions.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Направление успешно добавлено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения направления
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Direction item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.Directions.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Направление не найдено");
                    }
                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Направление успешно изменено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления направления
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
                    var del = context.Directions.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Направление не найдено");
                    }

                    var hasEquipment = context.Equipment.Any(x => x.DirectionId == id);
                    if (hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить направление, используемое в оборудовании");
                    }

                    context.Directions.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Направление успешно удалено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}