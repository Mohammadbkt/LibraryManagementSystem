using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Circulation.Loan;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class LoanMappers
    {
        public static Loan ToEntity(this CheckoutDto dto, string userId, int DefaultLoanDays)
        {
            return new Loan()
            {
                ItemId = dto.ItemId,
                UserId = userId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(dto.LoanDurationDays > 0 ? dto.LoanDurationDays : DefaultLoanDays),
                Status = LoanStatus.Active
            };
        }

        public static Expression<Func<Loan, LoanDto>> ToDto()
        {
            return loan => new LoanDto()
            {
                Id = loan.Id,
                UserId = loan.UserId,
                UserFullName = $"{loan.User.FirstName} {loan.User.LastName}".Trim(),
                ItemId = loan.ItemId,
                Barcode = loan.Item.Barcode ?? string.Empty,
                BookTitle = loan.Edition.Book.Title,
                BorrowDate = loan.BorrowDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                HasFine = loan.Fine != null && loan.Fine.Status != FineStatus.Paid,
                FineAmount = loan.Fine != null && loan.Fine.Status != FineStatus.Paid ? loan.Fine.Amount : 0
            };
        }
    }
}