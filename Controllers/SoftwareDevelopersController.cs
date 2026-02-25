using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class SoftwareDevelopersController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<SoftwareDeveloper>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.SoftwareDevelopers.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SoftwareDeveloper), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.SoftwareDevelopers.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Разработчик не найден");
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
        public ActionResult Add([FromForm] SoftwareDeveloper item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название разработчика обязательно");
                    }
                    context.SoftwareDevelopers.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Разработчик успешно добавлен");
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
        public ActionResult Edit([FromForm] SoftwareDeveloper item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.SoftwareDevelopers.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Разработчик не найден");
                    }
                    edit.Name = item.Name;
                    context.SaveChanges();
                    return StatusCode(200, "Разработчик успешно изменен");
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
                    var del = context.SoftwareDevelopers.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Разработчик не найден");
                    }

                    var hasSoftware = context.Software.Any(x => x.DeveloperId == id);
                    if (hasSoftware)
                    {
                        return StatusCode(400, "Нельзя удалить разработчика, у которого есть программы");
                    }

                    context.SoftwareDevelopers.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Разработчик успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}