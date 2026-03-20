using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
using library.Dtos.Catalog.Publisher;

namespace library.Services.Interface
{
    public interface IEditionService
    {
        
    // Editions
    Task<IEnumerable<EditionDto>> GetEditionsByBookAsync(int bookId);
    Task<EditionDto?> GetEditionByIdAsync(int id);
    Task<EditionDto> CreateEditionAsync(EditionCreateDto dto);
    Task<EditionDto> UpdateEditionAsync(int id, EditionUpdateDto dto);
    Task<bool> DeleteEditionAsync(int id);

    // Items
    Task<IEnumerable<ItemDto>> GetItemsByEditionAsync(int editionId);
    Task<IEnumerable<ItemDto>> GetItemsByBookAsync(int bookId);
    Task<ItemDto?> GetItemByIdAsync(int id);
    Task<ItemDto> CreateItemAsync(ItemCreateDto dto);
    Task<ItemDto> UpdateItemAsync(int id, ItemUpdateDto dto);
    Task<bool> DeleteItemAsync(int id);
    Task<IEnumerable<ItemDto>> GetAvailableItemsAsync(int editionId);
    }
}