using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace EquipmentManagement.Client.Services
{
    public static class OborudovanieMapper
    {
        // Конвертация из DTO в локальную модель
        public static Models.Oborudovanie ToLocalModel(this OborudovanieDto dto)
        {
            return new Models.Oborudovanie
            {
                Id = dto.Id,
                Name = dto.Nazvanie ?? string.Empty,
                InventNumber = dto.InventarnyiNomer ?? string.Empty,
                PriceObor = dto.Stoimost.ToString("F2"),
                IdResponUser = dto.OtvetstvennyiPolzovatelId ?? 0,
                IdTimeResponUser = dto.VremennoOtvetstvennyiPolzovatelId ?? 0,
                IdClassroom = dto.AuditoriaId ?? 0,
                IdNapravObor = dto.NapravlenieId ?? 0,
                IdStatusObor = dto.StatusId ?? 0,
                IdModelObor = dto.VidModeliId ?? 0,
                Comments = dto.Kommentarii ?? string.Empty,
                Photo = dto.Photo
            };
        }

        // Конвертация из списка DTO в список локальных моделей
        public static List<Models.Oborudovanie> ToLocalModels(this IEnumerable<OborudovanieDto> dtos)
        {
            return dtos.Select(dto => dto.ToLocalModel()).ToList();
        }

        // Конвертация из локальной модели в DTO для создания
        public static CreateOborudovanieRequest ToCreateRequest(this Models.Oborudovanie model)
        {
            return new CreateOborudovanieRequest
            {
                Nazvanie = model.Name ?? string.Empty,
                InventarnyiNomer = model.InventNumber ?? string.Empty,
                Stoimost = ParsePrice(model.PriceObor),
                AuditoriaId = model.IdClassroom > 0 ? model.IdClassroom : null,
                OtvetstvennyiPolzovatelId = model.IdResponUser > 0 ? model.IdResponUser : null,
                VremennoOtvetstvennyiPolzovatelId = model.IdTimeResponUser > 0 ? model.IdTimeResponUser : null,
                NapravlenieId = model.IdNapravObor > 0 ? model.IdNapravObor : null,
                StatusId = model.IdStatusObor > 0 ? model.IdStatusObor : null,
                VidModeliId = model.IdModelObor > 0 ? model.IdModelObor : null,
                Kommentarii = model.Comments,
                Photo = model.Photo
            };
        }

        // Конвертация из локальной модели в DTO для обновления
        public static UpdateOborudovanieRequest ToUpdateRequest(this Models.Oborudovanie model)
        {
            return new UpdateOborudovanieRequest
            {
                Id = model.Id,
                Nazvanie = model.Name ?? string.Empty,
                InventarnyiNomer = model.InventNumber ?? string.Empty,
                Stoimost = ParsePrice(model.PriceObor),
                AuditoriaId = model.IdClassroom > 0 ? model.IdClassroom : null,
                OtvetstvennyiPolzovatelId = model.IdResponUser > 0 ? model.IdResponUser : null,
                VremennoOtvetstvennyiPolzovatelId = model.IdTimeResponUser > 0 ? model.IdTimeResponUser : null,
                NapravlenieId = model.IdNapravObor > 0 ? model.IdNapravObor : null,
                StatusId = model.IdStatusObor > 0 ? model.IdStatusObor : null,
                VidModeliId = model.IdModelObor > 0 ? model.IdModelObor : null,
                Kommentarii = model.Comments,
                Photo = model.Photo
            };
        }

        private static decimal ParsePrice(string price)
        {
            if (decimal.TryParse(price, out var result))
                return result;
            return 0;
        }
    }
}