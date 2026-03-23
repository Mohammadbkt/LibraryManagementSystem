using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Publisher;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class PublisherService : IPublisherService
    {

        private readonly AppDbContext _context;

        public PublisherService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto dto)
        {
            var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == dto.Name && !p.IsDeleted);
            if (existingPublisher != null)
                throw new InvalidOperationException("Publisher already exists");

            var PublisherToAdd = dto.ToEntity();

            await _context.Publishers.AddAsync(PublisherToAdd);

            await _context.SaveChangesAsync();

            return PublisherToAdd.ToDto();
        }

        public async Task DeletePublisherAsync(int id)
        {
            var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPublisher == null)
                throw new InvalidOperationException("Publisher does not exists");

            var hasBooks = await _context.Books.AnyAsync(b => b.PublisherId == id);
            if (hasBooks)
                throw new InvalidOperationException("Cannot delete publisher that has books");

            existingPublisher.IsDeleted = true;
            existingPublisher.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<PublisherDto>> GetAllPublisherAsync(PublisherQueryParam queryParams)
        {
            var publishersQuery = _context.Publishers
                            .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
                publishersQuery = publishersQuery
                    .Where(c => c.Name.Contains(queryParams.Search));

            var totalCount = await publishersQuery.CountAsync();

            var publishers = await publishersQuery
                .OrderBy(p => p.Name)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .AsNoTracking()
                .ToListAsync();

            var publisherDtos = publishers.Select(c => c.ToDto()).ToList();

            return new PagedResult<PublisherDto>
            {
                Items = publisherDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
        {
            var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPublisher == null)
                return null;

            return existingPublisher.ToDto();
        }

        public async Task<PublisherDto> UpdatePublisherAsync(int id, PublisherUpdateDto dto)
        {
            var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPublisher == null)
                throw new InvalidOperationException("Publisher does not exists");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                existingPublisher.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Website))
                existingPublisher.Website = dto.Website;

            await _context.SaveChangesAsync();

            return existingPublisher.ToDto();
        }
    }
}