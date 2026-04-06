using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Circulation.Reservation;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class ReservationMappers
    {
        public static Reservation ToEntity(this CreateReservationDto reservationDto, int queuePosition)
        {
            return new Reservation()
            {
                BookId = reservationDto.BookId,
                UserId = reservationDto.UserId,
                ReservationDate = DateTime.UtcNow,
                ExpiryDate = null,
                NotificationSentAt = null,
                PickupDate = null,
                Status = ReservationStatus.Waiting,
                QueuePosition = queuePosition,
                IsDeleted = false,
                DeletedAt = null,
                ItemId = null
            };
        }

        public static Expression<Func<Reservation, ReservationDto>> ToDto()
        {
            return r=> new ReservationDto()
            {
                Id = r.Id,
                BookId = r.BookId,
                UserId = r.UserId,
                UserFullName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : string.Empty,
                BookTitle = r.Book.Title, 
                ItemId = r.ItemId,
                ReservationDate = r.ReservationDate,
                ExpiryDate = r.ExpiryDate,
                Status = r.Status.ToString(), 
                QueuePosition = r.QueuePosition
            };
        }
    }
}