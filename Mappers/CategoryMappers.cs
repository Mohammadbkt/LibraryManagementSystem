using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Catalog.Category;
using library.Models.Entities;

namespace library.Mappers
{
    public static class CategoryMappers
    {
        public static Expression<Func<Category, CategoryDto>> ToDto() => category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder
        };

        public static Expression<Func<Category, CategoryTreeDto>> ToTreeDto() => category => new CategoryTreeDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            SortOrder = category.SortOrder,
            Children = new List<CategoryTreeDto>()
        };
        

        public static Category ToEntity(this CategoryCreateDto dto) => new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder
        };
    }
}