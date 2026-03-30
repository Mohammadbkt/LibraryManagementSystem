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
            var userExists = await _userManager.FindByIdAsync(dto.UserId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found");

            var bookExists = await _context.Books.AnyAsync(b => b.Id == dto.BookId);
            if (!bookExists)
                throw new KeyNotFoundException($"Book with ID {dto.BookId} not found");

            var alreadyBookmarked = await _context.Bookmarks
                .AnyAsync(b => b.UserId == dto.UserId && b.BookId == dto.BookId);

            if (alreadyBookmarked)
                throw new OperationCanceledException("Book already bookmarked");                

            var bookmark = dto.ToEntity(userId);

            var addedBookmark = await _context.Bookmarks.AddAsync(bookmark);
            if (addedBookmark == null)
                throw new OperationCanceledException("Error while Bookmarking the book");

            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDto> AddReviewAsync(string userId, AddReviewDto dto)
        {
            var bookExists = await _context.Books
                                .AnyAsync(b => b.Id == dto.BookId);

            if (!bookExists)
                throw new KeyNotFoundException("Book not found");

            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var loanExists = await _context.Loans.FirstOrDefaultAsync(l => l.Id == dto.LoanId && l.UserId == userId);

            if (loanExists == null)
                throw new KeyNotFoundException("Loan not found");

            var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.LoanId == dto.LoanId && r.UserId == userId && !r.IsDeleted);
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this loan");

            var review = dto.ToEntity(userId);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return review.ToDto();
        }

        public async Task DeleteReviewAsync(string userId, int reviewId)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var reviewExists = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (reviewExists == null)
                throw new InvalidOperationException($"Review With ID {reviewId} not found");

            reviewExists.IsDeleted = true;
            reviewExists.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }

        public async Task<PagedResult<ReviewDto>> GetBookReviewsAsync(string userId, int bookId, ReviewQueryParam queryParam)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var bookExists = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (bookExists == null)
                throw new KeyNotFoundException($"Book With ID {bookId} not found");

            var reviewQuery = _context.Reviews
                                    .Where(r => r.BookId == bookId && !r.IsDeleted)
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParam.Search))
            {
                reviewQuery = reviewQuery.Where(r => r.Comment != null && r.Comment.Contains(queryParam.Search));
            }

            var totalCount = await reviewQuery.CountAsync();

            var reviews = await reviewQuery
                                    .Include(r => r.Book)
                                    .Include(r => r.User)
                                    .OrderBy(r => r.Rating)
                                    .Skip((queryParam.Page - 1) * queryParam.PageSize)
                                    .Take(queryParam.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();

            var reviewsDto = reviews.Select(r => r.ToDto());

            return new PagedResult<ReviewDto>()
            {
                Items = reviewsDto,
                TotalCount = totalCount,
                Page = queryParam.Page,
                PageSize = queryParam.PageSize
            };
        }


        public async Task<PagedResult<BookmarkDto>> GetUserBookmarksAsync(BookmarkQueryParam queryParams)
        {
            var bookmarks = await _context.Bookmarks
                                            .AsNoTracking()
                                            .OrderByDescending(b => b.CreatedAt)
                                            .Include(bm => bm.Book)
                                            .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                            .Take(queryParams.PageSize)
                                            .ToListAsync();

            var totalCount = bookmarks.Count();

            var bookmarkDto = bookmarks.Select(bm => bm.ToDto());

            return new PagedResult<BookmarkDto>
            {
                Items = bookmarkDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<PagedResult<ReviewDto>> GetUserReviewsAsync(string userId, ReviewQueryParam queryParam)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var reviewQuery = _context.Reviews
                                    .Where(r => r.UserId == userId && !r.IsDeleted)
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParam.Search))
            {
                reviewQuery = reviewQuery.Where(r => r.Comment != null && r.Comment.Contains(queryParam.Search));
            }

            var totalCount = await reviewQuery.CountAsync();

            var reviews = await reviewQuery
                                    .Include(r => r.Book)
                                    .Include(r => r.User)
                                    .OrderBy(r => r.Rating)
                                    .Skip((queryParam.Page - 1) * queryParam.PageSize)
                                    .Take(queryParam.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();

            var reviewsDto = reviews.Select(r => r.ToDto());

            return new PagedResult<ReviewDto>()
            {
                Items = reviewsDto,
                TotalCount = totalCount,
                Page = queryParam.Page,
                PageSize = queryParam.PageSize
            };
        }

        public async Task<bool> IsBookmarkedAsync(string userId, int bookId)
        {
            return await _context.Bookmarks.AnyAsync(bm => bm.BookId == bookId && bm.UserId == userId);
        }

        public async Task RemoveBookmarkAsync(string userId, int bookId)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            var alreadyBookmarked = await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.BookId == bookId);

            if (alreadyBookmarked)
                throw new OperationCanceledException("Book already bookmarked");

            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(bm => bm.BookId == bookId && bm.UserId == userId);
            if (bookmark == null)
                throw new OperationCanceledException("cant delete, already bookmarked");

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDto> UpdateReviewAsync(string userId, int reviewId, UpdateReviewDto dto)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var reviewExists = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (reviewExists == null)
                throw new InvalidOperationException($"Review With ID {reviewId} not found");

            if (dto.Rating.HasValue)
                reviewExists.Rating = dto.Rating.Value;

            if (dto.Comment != null)
                reviewExists.Comment = dto.Comment;

            reviewExists.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return reviewExists.ToDto();
        }
    }
}