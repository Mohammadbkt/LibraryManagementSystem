using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Category;

namespace library.Services.Interface
{
    public interface ICategoryService
    {
        
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto);
    Task<CategoryDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
    Task<bool> DeleteCategoryAsync(int id);
    }
}