
using RestAPI.Connect;
using RestAPI.Models;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace RestAPI.Services
{
    public class SetevyeNastroikiService
    {
        private readonly ApplicationDbContext _context;

        public SetevyeNastroikiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<SetevyeNastroiki> GetAll(string? search, string? sortBy)
        {
            var query = _context.SetevyeNastroiki.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.IpAdres.Contains(search));
            }

            return sortBy?.ToLower() switch
            {
                "ip" => query.OrderBy(s => s.IpAdres),
                _ => query.OrderBy(s => s.Id)
            };
        }

        public SetevyeNastroiki? GetById(int id)
        {
            return _context.SetevyeNastroiki.Find(id);
        }

        public void Create(SetevyeNastroiki nastroiki)
        {
            // Валидация IP адреса
            if (!IsValidIPAddress(nastroiki.IpAdres))
                throw new ArgumentException("Некорректный IP адрес");

            if (!IsValidIPAddress(nastroiki.MaskaPodseti))
                throw new ArgumentException("Некорректная маска подсети");

            if (!string.IsNullOrEmpty(nastroiki.GlavnyiShliuz) && !IsValidIPAddress(nastroiki.GlavnyiShliuz))
                throw new ArgumentException("Некорректный главный шлюз");

            if (!string.IsNullOrEmpty(nastroiki.Dns1) && !IsValidIPAddress(nastroiki.Dns1))
                throw new ArgumentException("Некорректный DNS сервер 1");

            if (!string.IsNullOrEmpty(nastroiki.Dns2) && !IsValidIPAddress(nastroiki.Dns2))
                throw new ArgumentException("Некорректный DNS сервер 2");

            _context.SetevyeNastroiki.Add(nastroiki);
            _context.SaveChanges();
        }

        public void Update(SetevyeNastroiki nastroiki)
        {
            _context.SetevyeNastroiki.Update(nastroiki);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var nastroiki = _context.SetevyeNastroiki.Find(id);
            if (nastroiki == null) return false;

            _context.SetevyeNastroiki.Remove(nastroiki);
            _context.SaveChanges();
            return true;
        }

        public bool IpExists(string ip)
        {
            return _context.SetevyeNastroiki.Any(s => s.IpAdres == ip);
        }

        // ПРОВЕРКА IP АДРЕСА ПО МАСКЕ XXX.XXX.XXX.XXX (0-255)
        private bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;

            var parts = ip.Split('.');
            if (parts.Length != 4) return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int number) || number < 0 || number > 255)
                    return false;
            }

            return true;
        }

        // ПРОВЕРКА УСТРОЙСТВА В СЕТИ (БЕЗ PING)
        public NetworkCheckResult CheckNetworkDevice(string ip, int port = 80, int timeout = 2000)
        {
            try
            {
                if (!IsValidIPAddress(ip))
                    return new NetworkCheckResult { IsValid = false, Error = "Некорректный IP адрес" };

                // Метод 1: Проверка через TCP соединение (наиболее надежный)
                var tcpResult = CheckViaTcp(ip, port, timeout);
                if (tcpResult.IsAvailable)
                    return new NetworkCheckResult { IsValid = true, Method = "TCP", ResponseTime = tcpResult.ResponseTime };

                // Метод 2: Проверка через ARP (если в локальной сети)
                var arpResult = CheckViaArp(ip, timeout);
                if (arpResult.IsAvailable)
                    return new NetworkCheckResult { IsValid = true, Method = "ARP", MacAddress = arpResult.MacAddress };

                // Метод 3: Проверка через UDP (для некоторых служб)
                var udpResult = CheckViaUdp(ip, 53, timeout); // DNS порт
                if (udpResult.IsAvailable)
                    return new NetworkCheckResult { IsValid = true, Method = "UDP", ResponseTime = udpResult.ResponseTime };

                return new NetworkCheckResult
                {
                    IsValid = false,
                    Error = "Устройство недоступно по TCP, ARP и UDP"
                };
            }
            catch (Exception ex)
            {
                return new NetworkCheckResult
                {
                    IsValid = false,
                    Error = $"Ошибка проверки: {ex.Message}"
                };
            }
        }

        // ПРОВЕРКА ВСЕХ УСТРОЙСТВ В СЕТИ
        public List<NetworkDeviceStatus> CheckAllNetworkDevices()
        {
            var devices = _context.SetevyeNastroiki.ToList();
            var results = new List<NetworkDeviceStatus>();

            foreach (var device in devices)
            {
                var checkResult = CheckNetworkDevice(device.IpAdres);
                var oborudovanie = _context.Oborudovanie.FirstOrDefault(o => o.Id == device.OborudovanieId);

                results.Add(new NetworkDeviceStatus
                {
                    DeviceId = device.OborudovanieId,
                    DeviceName = oborudovanie?.Nazvanie ?? "Неизвестно",
                    IpAddress = device.IpAdres,
                    IsOnline = checkResult.IsValid,
                    CheckMethod = checkResult.Method,
                    ResponseTime = checkResult.ResponseTime,
                    MacAddress = checkResult.MacAddress,
                    LastChecked = DateTime.Now,
                    Error = checkResult.Error
                });
            }

            return results;
        }

        // МЕТОД 1: TCP ПРОВЕРКА
        private TcpCheckResult CheckViaTcp(string ip, int port, int timeout)
        {
            try
            {
                using var client = new TcpClient();
                var startTime = DateTime.Now;

                var task = client.ConnectAsync(ip, port);
                if (task.Wait(timeout))
                {
                    var responseTime = (DateTime.Now - startTime).TotalMilliseconds;
                    return new TcpCheckResult { IsAvailable = true, ResponseTime = (int)responseTime };
                }
                else
                {
                    return new TcpCheckResult { IsAvailable = false };
                }
            }
            catch
            {
                return new TcpCheckResult { IsAvailable = false };
            }
        }

        // МЕТОД 2: ARP ПРОВЕРКА (для Windows/Linux)
        private ArpCheckResult CheckViaArp(string ip, int timeout)
        {
            try
            {
                // Для Windows: arp -a
                // Для Linux: arp -n
                // Здесь упрощенная реализация

                // В реальном проекте нужно вызывать системные команды
                // или использовать библиотеки типа SharpPcap

                return new ArpCheckResult { IsAvailable = false };
            }
            catch
            {
                return new ArpCheckResult { IsAvailable = false };
            }
        }

        // МЕТОД 3: UDP ПРОВЕРКА
        private UdpCheckResult CheckViaUdp(string ip, int port, int timeout)
        {
            try
            {
                using var client = new UdpClient();
                client.Client.ReceiveTimeout = timeout;

                var startTime = DateTime.Now;
                client.Connect(ip, port);

                // Отправляем пустой пакет
                var sendBytes = new byte[1] { 0x00 };
                client.Send(sendBytes, sendBytes.Length);

                // Пытаемся получить ответ (для некоторых служб)
                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var receiveBytes = client.Receive(ref remoteEP);

                var responseTime = (DateTime.Now - startTime).TotalMilliseconds;
                return new UdpCheckResult { IsAvailable = true, ResponseTime = (int)responseTime };
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                return new UdpCheckResult { IsAvailable = false };
            }
            catch
            {
                return new UdpCheckResult { IsAvailable = false };
            }
        }

        // Классы для результатов
        public class NetworkCheckResult
        {
            public bool IsValid { get; set; }
            public string? Method { get; set; }
            public int? ResponseTime { get; set; } // в мс
            public string? MacAddress { get; set; }
            public string? Error { get; set; }
        }

        public class NetworkDeviceStatus
        {
            public int DeviceId { get; set; }
            public string DeviceName { get; set; } = string.Empty;
            public string IpAddress { get; set; } = string.Empty;
            public bool IsOnline { get; set; }
            public string? CheckMethod { get; set; }
            public int? ResponseTime { get; set; }
            public string? MacAddress { get; set; }
            public DateTime LastChecked { get; set; }
            public string? Error { get; set; }
        }

        private class TcpCheckResult
        {
            public bool IsAvailable { get; set; }
            public int? ResponseTime { get; set; }
        }

        private class ArpCheckResult
        {
            public bool IsAvailable { get; set; }
            public string? MacAddress { get; set; }
        }

        private class UdpCheckResult
        {
            public bool IsAvailable { get; set; }
            public int? ResponseTime { get; set; }
        }
    }
}