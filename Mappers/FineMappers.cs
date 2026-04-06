using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Circulation.Fine;
using library.Models.Entities;

namespace library.Mappers
{
    public static class FineMappers
    {
        public static Fine ToEntity()
        {
            return new Fine()
            {
                
            }
            ;
        }

        public static Expression<Func<Fine, FineDto>> ToDto()
        {
            return f => new FineDto()
            {
                Id = f.Id,
                LoanId = f.LoanId,
                BookTitle = f.Loan.Edition.Book.Title,
                Amount = f.Amount,
                IssuedDate = f.IssuedDate,
                PaidDate = f.PaidDate,
                Status = f.Status,
                Reason = f.Reason
            }
            ;
        }

    }
}