using System;
using System.Collections.Generic;

namespace EquipmentManagement.Client.Models.DTO
{
    // Модель для получения данных из API
    public class OborudovanieDto
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
        public string InventarnyiNomer { get; set; } = string.Empty;
        public decimal Stoimost { get; set; }
        public int? AuditoriaId { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
        public int? NapravlenieId { get; set; }
        public int? StatusId { get; set; }
        public int? VidModeliId { get; set; }
        public string? Kommentarii { get; set; }
        public byte[]? Photo { get; set; }
    }

    // Модель для отправки данных в API
    public class CreateOborudovanieRequest
    {
        public string Nazvanie { get; set; } = string.Empty;
        public string InventarnyiNomer { get; set; } = string.Empty;
        public decimal Stoimost { get; set; }
        public int? AuditoriaId { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
        public int? NapravlenieId { get; set; }
        public int? StatusId { get; set; }
        public int? VidModeliId { get; set; }
        public string? Kommentarii { get; set; }
        public byte[]? Photo { get; set; }
    }

    // Модель для обновления данных в API
    public class UpdateOborudovanieRequest : CreateOborudovanieRequest
    {
        public int Id { get; set; }
    }
}