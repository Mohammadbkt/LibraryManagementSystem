using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Item;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class ItemMappers
    {
        public static ItemDto ToDto(this Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                EditionId = item.EditionId,
                Barcode = item.Barcode,
                ItemStatus = item.ItemStatus.ToString(),
                Price = item.Price,
                Notes = item.Notes,
                Location = item.Location,
                AcquisitionDate = item.AcquisitionDate
            };
        }

        public static Item ToEntity(this ItemCreateDto dto)
        {
            return new Item
            {
                EditionId = dto.EditionId,
                Barcode = dto.Barcode,
                AcquisitionDate = dto.AcquisitionDate,
                Price = dto.Price,
                Notes = dto.Notes,
                Location = dto.Location,
                ItemStatus = ItemStatus.Available
            };
        }
    }
}