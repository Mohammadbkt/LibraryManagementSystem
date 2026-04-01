using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Fine;
using library.Dtos.Circulation.Loan;
using library.Dtos.Circulation.Reservation;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface ICirculationService
    {
        
        // ─── Loans ───────────────────────────────────────────────────────────
        Task<CheckoutResponseDto> CheckoutItemAsync(string userId, CheckoutDto dto);
        Task<LoanDto> ReturnItemAsync(int loanId, ReturnDto dto);
        Task<LoanDto> ExtendLoanAsync(int loanId);
        Task<PagedResult<LoanDto>> GetUserLoansAsync(string userId, LoanQueryParam queryParams);
        Task<LoanDto?> GetLoanByIdAsync(int loanId);

        // ─── Reservations ────────────────────────────────────────────────────
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task CancelReservationAsync(string userId, int reservationId);
        Task<PagedResult<ReservationDto>> GetUserReservationsAsync(string userId, ReservationQueryParams queryParams);
        Task<ReservationDto?> GetReservationByIdAsync(int reservationId);

        // ─── Fines ───────────────────────────────────────────────────────────
        Task<PagedResult<FineDto>> GetUserFinesAsync(string userId, FineQueryParam queryParams);
        Task<PagedResult<FineDto>> GetAllFinesAsync(FineQueryParam queryParams);
        Task<FineDto> PayFineAsync(string userId, int fineId);

        // ─── Admin/Librarian ─────────────────────────────────────────────────
        Task<PagedResult<LoanDto>> GetAllActiveLoansAsync(LoanQueryParam queryParams);
        Task<PagedResult<ReservationDto>> GetAllReservationsAsync(ReservationQueryParams queryParams);
        Task<LoanDto> FulfillReservationAsync(int reservationId);
    }
}