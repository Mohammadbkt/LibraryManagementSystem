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
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == dto.Name && !c.IsDeleted);

            if (existingCategory != null)
                throw new InvalidOperationException("Category already exists");

            if (dto.ParentId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.ParentId.Value && !c.IsDeleted);

                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found");
            }

            var categoryToAdd = dto.ToEntity();

            if (categoryToAdd.SortOrder == 0)
            {
                var maxSortOrder = await _context.Categories
                    .Where(c => c.ParentId == dto.ParentId && !c.IsDeleted)
                    .MaxAsync(c => (int?)c.SortOrder) ?? 0;
                categoryToAdd.SortOrder = maxSortOrder + 1;
            }

            await _context.Categories.AddAsync(categoryToAdd);
            await _context.SaveChangesAsync();

            return categoryToAdd.ToDto();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null)
                throw new KeyNotFoundException("Category does not exist");

            var hasChildren = await _context.Categories
                .AnyAsync(c => c.ParentId == id && !c.IsDeleted);

            if (hasChildren)
                throw new InvalidOperationException("Cannot delete category with subcategories. Please delete or reassign subcategories first.");

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;

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
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null)
                return null;

            return category.ToDto();
        }

        public async Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync()
        {
            var allCategories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();

            return BuildTree(allCategories, null);
        }

        private static IEnumerable<CategoryTreeDto> BuildTree(
            List<Category> allCategories,
            int? parentId)
        {
            return allCategories
                .Where(c => c.ParentId == parentId)
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
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null)
                throw new KeyNotFoundException("Category does not exist");

            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId.Value == id)
                    throw new InvalidOperationException("Category cannot be its own parent");

                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.ParentId.Value && !c.IsDeleted);

                if (!parentExists)
                    throw new InvalidOperationException("Parent category not found");

                if (await IsCircularParentAsync(id, dto.ParentId.Value))
                    throw new InvalidOperationException("Circular parent relationship detected");

                category.ParentId = dto.ParentId;
            }
            else if (dto.ParentId == null)
            {
                category.ParentId = null;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != category.Name)
            {
                var nameExists = await _context.Categories
                    .AnyAsync(c => c.Name == dto.Name && c.Id != id && !c.IsDeleted);

                if (nameExists)
                    throw new InvalidOperationException("Category name already exists");

                category.Name = dto.Name;
            }

            if (dto.Description != null)
                category.Description = dto.Description;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return category.ToDto();
        }

        private async Task<bool> IsCircularParentAsync(int categoryId, int newParentId)
        {
            var allCategories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToDictionaryAsync(c => c.Id);

            var currentId = newParentId;
            while (allCategories.TryGetValue(currentId, out var current))
            {
                if (current.Id == categoryId)
                    return true;

                if (!current.ParentId.HasValue)
                    break;

                currentId = current.ParentId.Value;
            }

            return false;
        }
    }
}