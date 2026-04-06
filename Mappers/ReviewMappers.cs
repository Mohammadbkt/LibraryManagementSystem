using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Circulation.Review;
using library.Models.Entities;

namespace library.Mappers
{
    public static class ReviewMappers
    {
        public static Expression<Func<Review, ReviewDto>> ToDto() => review => new ReviewDto
        {
                Id = review.Id,
                BookId = review.BookId,
                BookTitle = review.Book.Title,
                UserId = review.UserId,
                LoanId = review.LoanId,
                UserFullName = review.User.FirstName + " " + review.User.LastName,
                Rating = review.Rating,
                Comment = review.Comment,

        };
        

        public static Review ToEntity(this AddReviewDto dto, string userId)
        {
            return new Review()
            {
                LoanId = dto.LoanId,
                BookId = dto.BookId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
        }
    }
}