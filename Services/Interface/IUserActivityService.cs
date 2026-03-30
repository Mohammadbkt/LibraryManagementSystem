using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Bookmark;
using library.Dtos.Circulation.Loan;
using library.Dtos.Circulation.Review;
using library.Dtos.Common;
using library.Models.Entities;

namespace library.Services.Interface
{
    public interface IUserActivityService
    {
        //Bookmark
        Task AddBookmarkAsync(string userId, AddBookmarkDto dto);
        Task RemoveBookmarkAsync(string userId, int bookId);
        Task<PagedResult<BookmarkDto>> GetUserBookmarksAsync(BookmarkQueryParam queryParam);
        Task<bool> IsBookmarkedAsync(string userId, int bookId);

        //Reviews
        Task<ReviewDto> AddReviewAsync(string userId, AddReviewDto dto);
        Task DeleteReviewAsync(string userId, int reviewId);
        Task<ReviewDto> UpdateReviewAsync(string userId, int reviewId, UpdateReviewDto dto);
        Task<PagedResult<ReviewDto>> GetUserReviewsAsync(string userId, ReviewQueryParam queryParam);
        Task<PagedResult<ReviewDto>> GetBookReviewsAsync(string userId,  int bookId,ReviewQueryParam queryParam);





    }
}