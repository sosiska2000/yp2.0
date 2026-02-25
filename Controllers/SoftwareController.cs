using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v5")]
    [ApiController]
    public class SoftwareController : ControllerBase
    {
        ///<summary>
        ///Получение списка программного обеспечения
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Software>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Software.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение программы по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Software), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Software.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Программа не найдена");
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
        ///Получение программ по разработчику
        ///</summary>
        [HttpGet("ByDeveloper/{developerId}")]
        [ProducesResponseType(typeof(List<Software>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByDeveloper(int developerId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Software.Where(x => x.DeveloperId == developerId).OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления программы
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Software item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название программы обязательно");
                    }

                    if (item.DeveloperId.HasValue)
                    {
                        var devExists = context.SoftwareDevelopers.Any(x => x.Id == item.DeveloperId);
                        if (!devExists)
                        {
                            return StatusCode(400, "Указанный разработчик не существует");
                        }
                    }

                    context.Software.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Программа успешно добавлена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения программы
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Software item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.Software.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Программа не найдена");
                    }

                    if (item.DeveloperId.HasValue)
                    {
                        var devExists = context.SoftwareDevelopers.Any(x => x.Id == item.DeveloperId);
                        if (!devExists)
                        {
                            return StatusCode(400, "Указанный разработчик не существует");
                        }
                        edit.DeveloperId = item.DeveloperId;
                    }

                    edit.Name = item.Name;
                    edit.Version = item.Version;
                    context.SaveChanges();
                    return StatusCode(200, "Программа успешно изменена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления программы
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
                    var del = context.Software.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Программа не найдена");
                    }

                    var hasEquipment = context.EquipmentSoftware.Any(x => x.SoftwareId == id);
                    if (hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить программу, установленную на оборудовании");
                    }

                    context.Software.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Программа успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}