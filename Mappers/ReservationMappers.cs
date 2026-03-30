using System;
using System.Collections.Generic;
using System.Linq;
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

        public static ReservationDto ToDto(this Reservation reservation)
        {
            return new ReservationDto()
            {
                Id = reservation.Id,
                BookId = reservation.BookId,
                UserId = reservation.UserId,
                UserFullName = reservation.User != null ? $"{reservation.User.FirstName} {reservation.User.LastName}" : string.Empty,
                BookTitle = reservation.Book?.Title ?? string.Empty, 
                ItemId = reservation.ItemId,
                ReservationDate = reservation.ReservationDate,
                ExpiryDate = reservation.ExpiryDate,
                Status = reservation.Status.ToString(), 
                QueuePosition = reservation.QueuePosition
            };
        }
    }
}