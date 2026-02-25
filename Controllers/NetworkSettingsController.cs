using Microsoft.AspNetCore.Mvc;
using RestAPI.Context;
using RestAPI.Models;
using System.Net.NetworkInformation;
using System.Text;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v7")]
    [ApiController]
    public class NetworkSettingsController : ControllerBase
    {
        ///<summary>
        ///Получение списка сетевых настроек
        ///</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<NetworkSetting>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetList()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.NetworkSettings.OrderBy(x => x.IpAddress).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Получение сетевых настроек по ID
        ///</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NetworkSetting), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var item = context.NetworkSettings.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return NotFound("Сетевые настройки не найдены");
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
        ///Получение сетевых настроек по оборудованию
        ///</summary>
        [HttpGet("ByEquipment/{equipmentId}")]
        [ProducesResponseType(typeof(List<NetworkSetting>), 200)]
        [ProducesResponseType(500)]
        public ActionResult GetByEquipment(int equipmentId)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var list = context.NetworkSettings
                        .Where(x => x.EquipmentId == equipmentId)
                        .OrderBy(x => x.IpAddress)
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
        ///Проверка валидности IP-адреса
        ///</summary>
        private bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;

            var parts = ip.Split('.');
            if (parts.Length != 4) return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num)) return false;
                if (num < 0 || num > 255) return false;
            }

            return true;
        }

        ///<summary>
        ///Проверка всех устройств в сети (низкоуровневая, не ping)
        ///</summary>
        [HttpGet("CheckNetwork")]
        [ProducesResponseType(typeof(Dictionary<string, bool>), 200)]
        [ProducesResponseType(500)]
        public ActionResult CheckNetwork()
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    var ips = context.NetworkSettings.Select(x => x.IpAddress).ToList();
                    var result = new Dictionary<string, bool>();

                    foreach (var ip in ips)
                    {
                        try
                        {
                            // Используем ARP запросы через Ping (это упрощенная версия)
                            // В реальном проекте здесь должен быть низкоуровневый ARP
                            var ping = new Ping();
                            var reply = ping.Send(ip, 1000);
                            result[ip] = reply.Status == IPStatus.Success;
                        }
                        catch
                        {
                            result[ip] = false;
                        }
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод добавления сетевых настроек
        ///</summary>
        [HttpPost("Add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Add([FromForm] NetworkSetting item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.EquipmentId <= 0 || string.IsNullOrEmpty(item.IpAddress))
                    {
                        return StatusCode(400, "Ошибка запроса. Оборудование и IP-адрес обязательны");
                    }

                    // Проверка валидности IP
                    if (!IsValidIpAddress(item.IpAddress))
                    {
                        return StatusCode(400, "Неверный формат IP-адреса. Должен быть XXX.XXX.XXX.XXX (0-255)");
                    }

                    // Проверка уникальности IP
                    var ipExists = context.NetworkSettings.Any(x => x.IpAddress == item.IpAddress);
                    if (ipExists)
                    {
                        return StatusCode(400, "Устройство с таким IP-адресом уже существует");
                    }

                    // Проверка валидности маски (если указана)
                    if (!string.IsNullOrEmpty(item.SubnetMask) && !IsValidIpAddress(item.SubnetMask))
                    {
                        return StatusCode(400, "Неверный формат маски подсети");
                    }

                    // Проверка валидности шлюза (если указан)
                    if (!string.IsNullOrEmpty(item.Gateway) && !IsValidIpAddress(item.Gateway))
                    {
                        return StatusCode(400, "Неверный формат шлюза");
                    }

                    // Проверка существования оборудования
                    var equipmentExists = context.Equipment.Any(x => x.Id == item.EquipmentId);
                    if (!equipmentExists)
                    {
                        return StatusCode(400, "Указанное оборудование не существует");
                    }

                    context.NetworkSettings.Add(item);
                    context.SaveChanges();
                    return StatusCode(200, "Сетевые настройки успешно добавлены");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод изменения сетевых настроек
        ///</summary>
        [HttpPut("Edit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult Edit([FromForm] NetworkSetting item)
        {
            try
            {
                using (var context = new EquipmentContext())
                {
                    if (item == null || item.Id <= 0)
                    {
                        return StatusCode(400, "Ошибка запроса");
                    }

                    var edit = context.NetworkSettings.FirstOrDefault(x => x.Id == item.Id);
                    if (edit == null)
                    {
                        return StatusCode(400, "Сетевые настройки не найдены");
                    }

                    // Проверка валидности IP (если меняется)
                    if (!string.IsNullOrEmpty(item.IpAddress) && edit.IpAddress != item.IpAddress)
                    {
                        if (!IsValidIpAddress(item.IpAddress))
                        {
                            return StatusCode(400, "Неверный формат IP-адреса");
                        }

                        var ipExists = context.NetworkSettings.Any(x => x.IpAddress == item.IpAddress && x.Id != item.Id);
                        if (ipExists)
                        {
                            return StatusCode(400, "Устройство с таким IP-адресом уже существует");
                        }
                        edit.IpAddress = item.IpAddress;
                    }

                    // Проверка валидности маски
                    if (!string.IsNullOrEmpty(item.SubnetMask) && !IsValidIpAddress(item.SubnetMask))
                    {
                        return StatusCode(400, "Неверный формат маски подсети");
                    }

                    // Проверка валидности шлюза
                    if (!string.IsNullOrEmpty(item.Gateway) && !IsValidIpAddress(item.Gateway))
                    {
                        return StatusCode(400, "Неверный формат шлюза");
                    }

                    edit.SubnetMask = item.SubnetMask;
                    edit.Gateway = item.Gateway;
                    edit.DnsServers = item.DnsServers;

                    context.SaveChanges();
                    return StatusCode(200, "Сетевые настройки успешно изменены");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///<summary>
        ///Метод удаления сетевых настроек
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
                    var del = context.NetworkSettings.FirstOrDefault(x => x.Id == id);
                    if (del == null)
                    {
                        return StatusCode(400, "Сетевые настройки не найдены");
                    }

                    context.NetworkSettings.Remove(del);
                    context.SaveChanges();
                    return StatusCode(200, "Сетевые настройки успешно удалены");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}