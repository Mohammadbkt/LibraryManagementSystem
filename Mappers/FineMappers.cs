using System;
using System.Collections.Generic;
using System.Linq;
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

        public static FineDto ToDto(this Fine fine)
        {
            return new FineDto()
            {
                Id = fine.Id,
                LoanId = fine.LoanId,
                BookTitle = fine.Loan.Edition.Book.Title,
                Amount = fine.Amount,
                IssuedDate = fine.IssuedDate,
                PaidDate = fine.PaidDate,
                Status = fine.Status,
                Reason = fine.Reason
            }
            ;
        }

    }
}