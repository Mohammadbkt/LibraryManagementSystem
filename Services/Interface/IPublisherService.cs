using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Publisher;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface IPublisherService
    {
        Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto dto);
        Task<PagedResult<PublisherDto>> GetAllPublisherAsync(PublisherQueryParam queryParam);
        Task<PublisherDto?> GetPublisherByIdAsync(int id);
        Task<PublisherDto> UpdatePublisherAsync(int id, PublisherUpdateDto dto);
        Task DeletePublisherAsync(int id);
    }
}