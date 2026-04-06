using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Circulation.Bookmark;
using library.Dtos.Circulation.Review;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class UserActivityService : IUserActivityService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserActivityService(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task AddBookmarkAsync(string userId, AddBookmarkDto dto)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var bookExists = await _context.Books.AnyAsync(b => b.Id == dto.BookId);
            if (!bookExists)
                throw new KeyNotFoundException($"Book with ID {dto.BookId} not found");

            var exists = await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.BookId == dto.BookId);

            if (exists)
                throw new InvalidOperationException("Book already bookmarked");

            await _context.Bookmarks.AddAsync(dto.ToEntity(userId));
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBookmarkAsync(string userId, int bookId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);

            if (bookmark is null)
                throw new KeyNotFoundException("Bookmark not found");

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBookmarkedAsync(string userId, int bookId)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .AnyAsync(b => b.UserId == userId && b.BookId == bookId);
        }

        public async Task<PagedResult<BookmarkDto>> GetUserBookmarksAsync(string userId, BookmarkQueryParam query)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");


            var bookmarkQuery = _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt);

            var totalCount = await bookmarkQuery.CountAsync();

            var items = await bookmarkQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(BookmarkMappers.ToDto())
                .ToListAsync();

            return new PagedResult<BookmarkDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ReviewDto> AddReviewAsync(string userId, AddReviewDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");


            if (!await _context.Books.AnyAsync(b => b.Id == dto.BookId))
                throw new KeyNotFoundException($"Book with ID {dto.BookId} not found");

            var loanExists = await _context.Loans
                .AnyAsync(l => l.Id == dto.LoanId && l.UserId == userId);

            if (!loanExists)
                throw new KeyNotFoundException("Loan not found");

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.LoanId == dto.LoanId && r.UserId == userId && !r.IsDeleted);

            if (alreadyReviewed)
                throw new InvalidOperationException("Already reviewed");

            var review = dto.ToEntity(userId);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.Id == review.Id)
                .Select(ReviewMappers.ToDto())
                .FirstAsync();
        }

        public async Task DeleteReviewAsync(string userId, int reviewId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

            if (review is null)
                throw new KeyNotFoundException("Review not found");

            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDto> UpdateReviewAsync(string userId, int reviewId, UpdateReviewDto dto)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");


            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

            if (review is null)
                throw new KeyNotFoundException("Review not found");

            if (dto.Rating.HasValue)
                review.Rating = dto.Rating.Value;

            if (!string.IsNullOrWhiteSpace(dto.Comment))
                review.Comment = dto.Comment;

            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.Id == reviewId)
                .Select(ReviewMappers.ToDto())
                .FirstAsync();

        }

        public async Task<PagedResult<ReviewDto>> GetBookReviewsAsync(string userId, int bookId, ReviewQueryParam query)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                throw new KeyNotFoundException($"User with ID {userId} not found");


            var reviewQuery = _context.Reviews
                .AsNoTracking()
                .Where(r => r.BookId == bookId && !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                reviewQuery = reviewQuery.Where(r =>
                    r.Comment != null && r.Comment.Contains(query.Search));
            }

            var totalCount = await reviewQuery.CountAsync();

            var items = await reviewQuery
                .OrderBy(r => r.Rating)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(ReviewMappers.ToDto())
                .ToListAsync();

            return new PagedResult<ReviewDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<PagedResult<ReviewDto>> GetUserReviewsAsync(string userId, ReviewQueryParam query)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                throw new KeyNotFoundException($"User with ID {userId} not found");


            var reviewQuery = _context.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId && !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                reviewQuery = reviewQuery.Where(r =>
                    r.Comment != null && r.Comment.Contains(query.Search));
            }

            var totalCount = await reviewQuery.CountAsync();

            var items = await reviewQuery
                .OrderBy(r => r.Rating)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(ReviewMappers.ToDto())
                .ToListAsync();

            return new PagedResult<ReviewDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}