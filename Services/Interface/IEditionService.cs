using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
using library.Dtos.Catalog.Publisher;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface IEditionService
    {
        
    // Editions
    Task<PagedResult<EditionDto>> GetEditionsByBookAsync(int bookId, EditionQueryParams queryParams);
    Task<EditionDto?> GetEditionByIdAsync(int id);
    Task<EditionDto> CreateEditionAsync(EditionCreateDto dto);
    Task<EditionDto> UpdateEditionAsync(int id, EditionUpdateDto dto);
    Task DeleteEditionAsync(int id);

    // Items
    Task<PagedResult<ItemDto>> GetItemsByEditionAsync(int editionId, ItemQueryParams queryParams);
    Task<PagedResult<ItemDto>> GetItemsByBookAsync(int bookId, ItemQueryParams queryParams);
    Task<ItemDto?> GetItemByIdAsync(int id);
    Task<ItemDto> CreateItemAsync(ItemCreateDto dto);
    Task<ItemDto> UpdateItemAsync(int id, ItemUpdateDto dto);
    Task DeleteItemAsync(int id);
    Task<PagedResult<ItemDto>> GetAvailableItemsAsync(int editionId, ItemQueryParams queryParams);
    }
}