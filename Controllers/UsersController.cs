using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v4")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ///<summary>
        ///Получение списка пользователей
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение пользователя по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.Users.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Пользователь не найден");
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
        ///Получение пользователей по роли
        ///</summary>
        [HttpGet("ByRole/{role}")]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByRole(string role)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.Users.Where(x => x.Role == role).OrderBy(x => x.LastName).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления пользователя
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] User item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || string.IsNullOrEmpty(item.Login) ||
                        string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.LastName))
                    {
                        return StatusCode(400, "Ошибка запроса. Логин, пароль и фамилия обязательны");
                    }

                    // Проверка уникальности логина
                    var loginExists = context.Users.Any(x => x.Login == item.Login);
                    if (loginExists)
                    {
                        return StatusCode(400, "Пользователь с таким логином уже существует");
                    }

                    // Проверка роли
                    var validRoles = new[] { "admin", "teacher", "employee" };
                    if (!string.IsNullOrEmpty(item.Role) && !validRoles.Contains(item.Role))
                    {
                        return StatusCode(400, "Роль должна быть admin, teacher или employee");
                    }

                    context.Users.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Пользователь успешно добавлен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения пользователя
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] User item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.Users.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Пользователь не найден");
                    }

                    // Проверка уникальности логина (если меняется)
                    if (edit.Login != item.Login)
                    {
                        var loginExists = context.Users.Any(x => x.Login == item.Login);
                        if (loginExists)
                        {
                            return StatusCode(400, "Пользователь с таким логином уже существует");
                        }
                        edit.Login = item.Login;
                    }

                    // Проверка роли
                    var validRoles = new[] { "admin", "teacher", "employee" };
                    if (!string.IsNullOrEmpty(item.Role) && !validRoles.Contains(item.Role))
                    {
                        return StatusCode(400, "Роль должна быть admin, teacher или employee");
                    }

                    edit.Password = item.Password;
                    edit.Role = item.Role;
                    edit.Email = item.Email;
                    edit.LastName = item.LastName;
                    edit.FirstName = item.FirstName;
                    edit.MiddleName = item.MiddleName;
                    edit.Phone = item.Phone;
                    edit.Address = item.Address;

                    context.SaveChanges();
                    return StatusCode(200, "Пользователь успешно изменен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления пользователя
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
                    var del = context.Users.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Пользователь не найден");
                    }

                    // Проверка связей
                    var hasEquipmentResponsible = context.Equipment.Any(x => x.ResponsibleUserId == id);
                    var hasEquipmentTemp = context.Equipment.Any(x => x.TempResponsibleUserId == id);
                    var hasClassroomResponsible = context.Classrooms.Any(x => x.ResponsibleUserId == id);
                    var hasClassroomTemp = context.Classrooms.Any(x => x.TempResponsibleUserId == id);
                    var hasConsumableResponsible = context.Consumables.Any(x => x.ResponsibleUserId == id);
                    var hasConsumableTemp = context.Consumables.Any(x => x.TempResponsibleUserId == id);
                    var hasInventoryCreated = context.Inventories.Any(x => x.CreatedByUserId == id);
                    var hasInventoryChecked = context.InventoryItems.Any(x => x.CheckedByUserId == id);

                    if (hasEquipmentResponsible || hasEquipmentTemp || hasClassroomResponsible ||
                        hasClassroomTemp || hasConsumableResponsible || hasConsumableTemp ||
                        hasInventoryCreated || hasInventoryChecked)
                    {
                        return StatusCode(400, "Нельзя удалить пользователя, который числится ответственным или участвует в инвентаризации");
                    }

                    context.Users.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Пользователь успешно удален");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}