using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        ///<summary>
        ///Получение списка аудиторий
        ///</summary>
        ///<response code="200">Успешная операция</response>
        ///<response code="500">Ошибка сервера</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Classroom>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Classrooms.OrderBy(x => x.Name).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение аудитории по ID
        ///</summary>
        ///<response code="200">Успешная операция</response>
        ///<response code="404">Не найдено</response>
        ///<response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Classroom), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Classrooms.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Аудитория не найдена");
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
        ///Метод добавления аудитории
        ///</summary>
        ///<response code="200">Успешная операция</response>
        ///<response code="400">Ошибка запроса</response>
        ///<response code="500">Ошибка сервера</response>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] Classroom item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        return StatusCode(400, "Ошибка запроса. Название аудитории обязательно");
                    }
                    context.Classrooms.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Аудитория успешно добавлена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения аудитории
        ///</summary>
        ///<response code="200">Успешное изменение</response>
        ///<response code="400">Ошибка запроса</response>
        ///<response code="500">Ошибка сервера</response>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] Classroom item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }
                    var edit = context.Classrooms.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Аудитория не найдена");
                    }
                    edit.Name = item.Name;
                    edit.ShortName = item.ShortName;
                    edit.ResponsibleUserId = item.ResponsibleUserId;
                    edit.TempResponsibleUserId = item.TempResponsibleUserId;
                    context.SaveChanges();
                    return StatusCode(200, "Аудитория успешно изменена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления аудитории
        ///</summary>
        ///<response code="200">Успешное удаление</response>
        ///<response code="400">Ошибка запроса</response>
        ///<response code="500">Ошибка сервера</response>
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
                    var del = context.Classrooms.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Аудитория не найдена");
                    }

                    var hasEquipment = context.Equipment.Any(x => x.ClassroomId == id);
                    if (hasEquipment)
                    {
                        return StatusCode(400, "Нельзя удалить аудиторию, к которой привязано оборудование");
                    }

                    context.Classrooms.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Аудитория успешно удалена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}