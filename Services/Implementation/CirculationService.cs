using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Circulation.Fine;
using library.Dtos.Circulation.Loan;
using library.Dtos.Circulation.Reservation;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Models.Enums;
using library.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class CirculationService : ICirculationService
    {

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private const int _DefaultLoanDays = 14;
        private const decimal _FinePerDay = 0.50m;
        private const int _ReservationExpiryDays = 3;
        private const int _MaxActiveLoans = 5;

        public CirculationService(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task CancelReservationAsync(string userId, int reservationId)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);
                if (reservation == null)
                    throw new KeyNotFoundException("Reservation not found");

                if (reservation.Status == ReservationStatus.Fulfilled)
                    throw new InvalidOperationException(
                        "Cannot cancel a fulfilled reservation");


                var remainingReservations = await _context.Reservations
                    .Where(r => r.BookId == reservation.BookId &&
                                r.Status == ReservationStatus.Waiting &&
                                r.QueuePosition > reservation.QueuePosition &&
                                !r.IsDeleted)
                    .ExecuteUpdateAsync(setters =>
                                setters.SetProperty(r => r.QueuePosition, r => r.QueuePosition - 1));

                reservation.Status = ReservationStatus.Cancelled;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<CheckoutResponseDto> CheckoutItemAsync(string userId, CheckoutDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                if (!user.IsActive)
                    throw new InvalidOperationException("Account is deactivated");

                var hasUnpaidFines = await _context.Fines.AnyAsync(f => f.UserId == userId && f.Status == FineStatus.Unpaid);
                if (hasUnpaidFines)
                    throw new InvalidOperationException("Cannot checkout — you have unpaid fines");


                var item = await _context.Items
                                            .Include(i => i.Edition)
                                                .ThenInclude(e => e.Book)
                                            .FirstOrDefaultAsync(i => i.Id == dto.ItemId);
                if (item == null)
                    throw new KeyNotFoundException($"item not found");

                if (item.ItemStatus != ItemStatus.Available)
                    throw new InvalidOperationException($"Item is not available — current status: {item.ItemStatus}");

                var alreadyBorrowed = await _context.Loans.AnyAsync(l => l.UserId == userId && l.Edition.BookId == item.Edition.BookId && l.Status == LoanStatus.Active);
                if (alreadyBorrowed)
                    throw new InvalidOperationException("You already have this book checked out");

                var activeLoansCount = await _context.Loans
                        .CountAsync(l => l.UserId == userId && l.Status == LoanStatus.Active);

                if (activeLoansCount >= _MaxActiveLoans)
                    throw new InvalidOperationException("You have reached the maximum number of active loans");

                var loan = dto.ToEntity(userId, _DefaultLoanDays);

                item.ItemStatus = ItemStatus.Borrowed;

                await _context.Loans.AddAsync(loan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CheckoutResponseDto
                {
                    Loan = loan.ToDto(),
                    RemainingLoans = _MaxActiveLoans - (activeLoansCount + 1)
                };

            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!user.IsActive)
                throw new InvalidOperationException("Account is deactivated");

            var bookExists = await _context.Books.AnyAsync(b => b.Id == dto.BookId);

            if (!bookExists)
                throw new KeyNotFoundException("Book not found");

            var isAvailable = await _context.Items
                .AnyAsync(i => i.Edition.BookId == dto.BookId && i.ItemStatus == ItemStatus.Available);

            if (isAvailable)
                throw new InvalidOperationException("Book is currently available — no need to reserve");

            var alreadyReserved = await _context.Reservations.AnyAsync(r => r.UserId == dto.UserId && r.BookId == dto.BookId && r.Status == ReservationStatus.Waiting);
            if (alreadyReserved)
                throw new InvalidOperationException("You already have an active reservation for this book");

            using var transaction = await _context.Database.BeginTransactionAsync();

            for (int i = 0; i < 3; i++) // retry mechanism
            {
                try
                {
                    var queuePosition = await _context.Reservations
                        .Where(r => r.BookId == dto.BookId && r.Status == ReservationStatus.Waiting)
                        .MaxAsync(r => (int?)r.QueuePosition) ?? 0;

                    var reservation = dto.ToEntity(queuePosition + 1);

                    await _context.Reservations.AddAsync(reservation);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return reservation.ToDto();
                }
                catch (DbUpdateException)
                {

                }
            }

            throw new Exception("Failed to create reservation due to concurrency conflict");
        }

        public async Task<LoanDto> ExtendLoanAsync(int loanId)
        {
            var loan = await _context.Loans
                                    .Include(l => l.Edition)
                                    .FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null)
                throw new KeyNotFoundException("Loan not found");

            if (loan.Status != LoanStatus.Active || loan.ReturnDate != null)
                throw new InvalidOperationException("Cannot extend — item already returned");

            if (DateTime.UtcNow > loan.DueDate)
                throw new InvalidOperationException("Cannot extend — loan is overdue");

            var hasReservations = await _context.Reservations.AnyAsync(r => r.BookId == loan.Edition.BookId && r.Status == ReservationStatus.Waiting);
            if (hasReservations)
                throw new InvalidOperationException("Cannot extend this loan because there are pending reservations for this book. Please return the book by the due date.");

            loan.DueDate = loan.DueDate.AddDays(_DefaultLoanDays);
            loan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return loan.ToDto();
        }

        public async Task<LoanDto> FulfillReservationAsync(int reservationId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Item)
                        .ThenInclude(i => i.Edition)
                            .ThenInclude(e => e.Book)
                    .FirstOrDefaultAsync(r => r.Id == reservationId);

                if (reservation == null)
                    throw new KeyNotFoundException("Reservation not found");

                if (reservation.Status != ReservationStatus.Ready)
                    throw new InvalidOperationException("Reservation is not ready");

                if (reservation.ExpiryDate < DateTime.UtcNow)
                    throw new InvalidOperationException("Reservation has expired");

                if (reservation.ItemId == null || reservation.Item == null)
                    throw new InvalidOperationException(
                        "No item assigned to this reservation");

                var loan = new Loan
                {
                    UserId = reservation.UserId,
                    ItemId = reservation.ItemId.Value,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(_DefaultLoanDays),
                    Status = LoanStatus.Active
                };

                await _context.Loans.AddAsync(loan);

                reservation.Status = ReservationStatus.Fulfilled;
                reservation.PickupDate = DateTime.UtcNow;
                reservation.UpdatedAt = DateTime.UtcNow;
                reservation.Item.ItemStatus = ItemStatus.Borrowed;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return loan.ToDto();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<ReservationDto>> GetAllReservationsAsync(ReservationQueryParams queryParams)
        {
            var reservationQuery = _context.Reservations
                                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Status) && Enum.TryParse<ReservationStatus>(queryParams.Status, out var status))
                reservationQuery = reservationQuery.Where(r => r.Status == status);

            var totalCount = await reservationQuery.CountAsync();

            var reservations = await reservationQuery
                                                .Include(r => r.Book)
                                                .Include(r => r.User)
                                                .AsNoTracking()
                                                .OrderByDescending(r => r.ReservationDate)
                                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                                .Take(queryParams.PageSize)
                                                .ToListAsync();

            var reservationsDto = reservations.Select(r => r.ToDto());

            return new PagedResult<ReservationDto>()
            {
                Items = reservationsDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
        public async Task<LoanDto?> GetLoanByIdAsync(int loanId)
        {
            var loan = await _context.Loans
                                    .Include(l => l.Item)
                                        .ThenInclude(i => i.Edition)
                                            .ThenInclude(e => e.Book)
                                    .Include(l => l.Fine)
                                    .Include(l => l.User)
                                    .FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null)
                throw new KeyNotFoundException("Loan not found");

            return loan.ToDto();
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                                        .Include(r => r.Book)
                                        .Include(r => r.User)
                                        .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found");

            return reservation.ToDto();
        }

        public async Task<PagedResult<FineDto>> GetUserFinesAsync(string userId, FineQueryParam queryParams)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var fineQuery = _context.Fines.Where(f => f.UserId == userId).AsQueryable();

            if (queryParams.Amount != 0)
                fineQuery = fineQuery.Where(f => f.Amount == queryParams.Amount);

            if (!string.IsNullOrWhiteSpace(queryParams.Status) && Enum.TryParse<FineStatus>(queryParams.Status, out var status))
                fineQuery = fineQuery.Where(f => f.Status == status);

            var totalCount = await fineQuery.CountAsync();

            var fines = await fineQuery
                                .Include(f => f.Loan)
                                    .ThenInclude(l => l.Edition)
                                        .ThenInclude(e => e.Book)
                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                .Take(queryParams.PageSize)
                                .AsNoTracking()
                                .OrderByDescending(f => f.Amount)
                                .ToListAsync();

            var finesDto = fines.Select(f => f.ToDto());

            return new PagedResult<FineDto>()
            {
                Items = finesDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<PagedResult<LoanDto>> GetUserLoansAsync(string userId, LoanQueryParam queryParams)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var loanQuery = _context.Loans.Where(l => l.UserId == userId).AsQueryable();

            if (queryParams.ActiveOnly)
                loanQuery = loanQuery.Where(l => l.Status == LoanStatus.Active);

            if (!string.IsNullOrWhiteSpace(queryParams.Status) && Enum.TryParse<LoanStatus>(queryParams.Status, out var status))
                loanQuery = loanQuery.Where(l => l.Status == status);

            var totalCount = await loanQuery.CountAsync();

            var loans = await loanQuery
                                .AsNoTracking()
                                .Include(l => l.Item)
                                    .ThenInclude(i => i.Edition)
                                        .ThenInclude(e => e.Book)
                                .Include(l => l.Fine)
                                .Include(l => l.User)
                                .OrderByDescending(l => l.BorrowDate)
                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                .Take(queryParams.PageSize)
                                .ToListAsync();

            var loansDto = loans.Select(l => l.ToDto());

            return new PagedResult<LoanDto>()
            {
                Items = loansDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<PagedResult<ReservationDto>> GetUserReservationsAsync(string userId, ReservationQueryParams queryParams)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var reservationQuery = _context.Reservations
                                            .Where(r => r.UserId == userId)
                                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Status) && Enum.TryParse<ReservationStatus>(queryParams.Status, out var status))
                reservationQuery = reservationQuery.Where(r => r.Status == status);

            var totalCount = await reservationQuery.CountAsync();

            var reservations = await reservationQuery
                                                .Include(r => r.Book)
                                                .Include(r => r.User)
                                                .AsNoTracking()
                                                .OrderByDescending(r => r.ReservationDate)
                                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                                .Take(queryParams.PageSize)
                                                .ToListAsync();

            var reservationsDto = reservations.Select(r => r.ToDto());

            return new PagedResult<ReservationDto>()
            {
                Items = reservationsDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<FineDto> PayFineAsync(string userId, int fineId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");


            var fine = await _context.Fines
                .Include(f => f.Loan)
                    .ThenInclude(l => l.Item)
                        .ThenInclude(i => i.Edition)
                            .ThenInclude(e => e.Book)
                .FirstOrDefaultAsync(f => f.Id == fineId && f.UserId == userId);

            if (fine == null)
                throw new KeyNotFoundException("Fine not found");

            if (fine.Status == FineStatus.Paid)
                throw new InvalidOperationException("Fine already paid");

            fine.Status = FineStatus.Paid;
            fine.PaidDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return fine.ToDto();
        }

        public async Task<LoanDto> ReturnItemAsync(int loanId, ReturnDto dto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var loan = await _context.Loans
                                    .Include(l => l.Item)
                                        .ThenInclude(i => i.Edition)
                                            .ThenInclude(e => e.Book)
                                    .Include(l => l.Fine)
                                    .FirstOrDefaultAsync(l => l.Id == loanId);

                if (loan == null)
                    throw new KeyNotFoundException("Loan not found");

                if (loan.ReturnDate != null || loan.Status == LoanStatus.Returned)
                    throw new InvalidOperationException("Item already returned");

                loan.ReturnDate = DateTime.UtcNow;
                loan.Status = LoanStatus.Returned;
                loan.Notes = dto.Notes;

                if (DateTime.UtcNow > loan.DueDate)
                {
                    var daysOverdue = (DateTime.UtcNow.Date - loan.DueDate.Date).Days;//var daysOverdue = (int)Math.Ceiling((DateTime.UtcNow - loan.DueDate).TotalDays);
                    var fineAmount = daysOverdue * _FinePerDay;

                    var fine = new Fine
                    {
                        LoanId = loanId,
                        UserId = loan.UserId,
                        Amount = fineAmount,
                        Reason = $"Overdue by {daysOverdue} days",
                        Status = FineStatus.Unpaid
                    };

                    await _context.Fines.AddAsync(fine);
                }

                var nextReservation = await _context.Reservations
                                                    .Where(r => r.BookId == loan.Item.Edition.BookId &&
                                                                r.Status == ReservationStatus.Waiting)
                                                    .OrderBy(r => r.QueuePosition).FirstOrDefaultAsync();

                if (nextReservation != null)
                {
                    nextReservation.ItemId = loan.ItemId;
                    nextReservation.Status = ReservationStatus.Ready;
                    nextReservation.ExpiryDate = DateTime.UtcNow.AddDays(_ReservationExpiryDays);
                    nextReservation.NotificationSentAt = DateTime.UtcNow;

                    loan.Item.ItemStatus = ItemStatus.Reserved;
                }
                else
                {
                    loan.Item.ItemStatus = ItemStatus.Available;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return loan.ToDto();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<PagedResult<LoanDto>> GetAllActiveLoansAsync(LoanQueryParam queryParams)
        {
            var loanQuery = _context.Loans.Where(l => l.Status == LoanStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.UserId))
                loanQuery = loanQuery.Where(l => l.UserId == queryParams.UserId);

            if (!string.IsNullOrWhiteSpace(queryParams.BookTitle))
                loanQuery = loanQuery.Where(l => l.Edition.Book.Title.Contains(queryParams.BookTitle));

            var totalCount = await loanQuery.CountAsync();

            var loans = await loanQuery
                                .Include(l => l.Item)
                                    .ThenInclude(i => i.Edition)
                                        .ThenInclude(e => e.Book)
                                .Include(l => l.Fine)
                                .Include(l => l.User)
                                .AsNoTracking()
                                .OrderByDescending(l => l.BorrowDate)
                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                .Take(queryParams.PageSize)
                                .ToListAsync();

            var loansDto = loans.Select(l => l.ToDto());

            return new PagedResult<LoanDto>()
            {
                Items = loansDto,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
    }
}