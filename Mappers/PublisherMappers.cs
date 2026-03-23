using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Publisher;
using library.Models.Entities;

namespace library.Mappers
{
    public static class PublisherMappers
    {
        public static PublisherDto ToDto(this Publisher publisher) => new PublisherDto
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Website = publisher.Website
        };

        public static Publisher ToEntity(this PublisherCreateDto dto) => new Publisher()
        {
            Name = dto.Name,
            Website = dto.Website,
        };
    }
}