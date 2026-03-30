using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public int? ItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public DateTime? NotificationSentAt { get; set; }
        public DateTime? PickupDate { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Waiting;
        public int QueuePosition { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Item? Item { get; set; }
        public User User { get; set; } = null!;
        public Book Book { get; set; } = null!;


    }
}