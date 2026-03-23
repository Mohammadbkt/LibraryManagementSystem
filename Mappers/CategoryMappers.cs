using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Category;
using library.Models.Entities;

namespace library.Mappers
{
    public static class CategoryMappers
    {
        public static CategoryDto ToDto(this Category category) => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder
        };

        public static CategoryTreeDto ToTreeDto(this Category category,
    List<Category> allCategories)
        {
            return new CategoryTreeDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                SortOrder = category.SortOrder,
                Children = allCategories
                    .Where(c => c.ParentId == category.Id)
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.Name)
                    .Select(c => c.ToTreeDto(allCategories))
            };
        }

        public static Category ToEntity(this CategoryCreateDto dto) => new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder
        };
    }
}