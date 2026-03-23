using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Category;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface ICategoryService
    {

        Task<PagedResult<CategoryDto>> GetAllCategoriesAsync(CategoryQueryParams queryParams);
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync();
        Task DeleteCategoryAsync(int id);
    }
}