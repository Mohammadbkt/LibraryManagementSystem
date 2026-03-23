using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Category;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;
using library.Mappers;
using library.Dtos.Common;

namespace library.Services.Implementation
{
    public class CategoryService : ICategoryService
    {

        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == dto.Name && !c.IsDeleted);
            if (existingCategory != null)
                throw new InvalidOperationException("category already exisits");

            if (dto.ParentId != null)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.ParentId && !c.IsDeleted);

                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found");
            }

            var categorytoAdd = dto.ToEntity();

            await _context.Categories.AddAsync(categorytoAdd);

            await _context.SaveChangesAsync();

            return categorytoAdd.ToDto();

        }

        public async Task DeleteCategoryAsync(int id)
        {
            var exisitingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (exisitingCategory == null)
                throw new KeyNotFoundException("category does not exists");

            var hasChildren = await _context.Categories
                            .AnyAsync(c => c.ParentId == id && !c.IsDeleted);

            if (hasChildren)
                throw new InvalidOperationException("Category has subcategories");

            exisitingCategory.IsDeleted = true;
            exisitingCategory.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }

        public async Task<PagedResult<CategoryDto>> GetAllCategoriesAsync(CategoryQueryParams queryParams)
        {
            var categoriesQuery = _context.Categories
                                    .Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
                categoriesQuery = categoriesQuery
                    .Where(c => c.Name.Contains(queryParams.Search));

            var totalCount = await categoriesQuery.CountAsync();

            var categories = await categoriesQuery
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .AsNoTracking()
                .ToListAsync();

            var categoryDtos = categories.Select(c => c.ToDto()).ToList();

            return new PagedResult<CategoryDto>
            {
                Items = categoryDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (existingCategory == null)
                return null;

            return existingCategory.ToDto();

        }

        public async Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync()
        {
            var allCategories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return BuildTree(allCategories, null);
        }

        private static IEnumerable<CategoryTreeDto> BuildTree(
            List<Category> allCategories,
            int? parentId)
        {
            return allCategories
                .Where(c => c.ParentId == parentId && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryTreeDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    SortOrder = c.SortOrder,
                    Children = BuildTree(allCategories, c.Id)
                });
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (existingCategory == null)
                throw new InvalidOperationException("category does not exisits");

            if (dto.ParentId == id)
                throw new InvalidOperationException("Category cannot be its own parent");

            if (dto.ParentId != null)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.ParentId && !c.IsDeleted);

                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found");

                if (await IsCircularParentAsync(id, dto.ParentId.Value))
                    throw new InvalidOperationException("Circular parent relationship detected");


                existingCategory.ParentId = dto.ParentId;
            }

            if (dto.Name != null)
            {
                var exists = await _context.Categories.AnyAsync(c =>
                                    c.Name == dto.Name &&
                                    c.Id != id &&
                                    !c.IsDeleted);

                if (exists)
                    throw new InvalidOperationException("Category name already exists");

                existingCategory.Name = dto.Name;
            }

            if (dto.Description != null)
                existingCategory.Description = dto.Description;

            await _context.SaveChangesAsync();

            return existingCategory.ToDto();


        }

        private async Task<bool> IsCircularParentAsync(int categoryId, int newParentId)
        {
            var parent = await _context.Categories.FirstOrDefaultAsync(c => c.Id == newParentId && !c.IsDeleted);

            while (parent != null)
            {
                if (parent.Id == categoryId)
                    return true;

                if (parent.ParentId == null)
                    break;

                parent = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Id == parent.ParentId && !c.IsDeleted);
            }

            return false;
        }

    }
}